/// <reference path="./interfaces/IAntiForgerySettings.d.ts" />
/// <reference path="./interfaces/IOrigin.d.ts" />
/// <reference path="./interfaces/JQueryAjaxSettings.d.ts" />
/// <reference path="./interfaces/JQueryStatic.d.ts" />
/// <reference path="./typings/jquery/jquery.d.ts" />

module AntiForgery {
    "use strict";

    export var Defaults: IAntiForgerySettings = {
        allow: ["static", "dynamic"],
        enable: true,
        header: "X-CSRF-Token",
        id: "VerificationToken",
        methods: ["DELETE", "POST", "PUT"],
        timeout: 0,
        url: "/token.axd"
    };

    export var Settings: IAntiForgerySettings = Defaults;

    var PromiseCache: { [index: string]: JQueryPromise<string> } = {};

    function HtmlEscape(value: string): string {
        return value.replace(/(?:&|<|>|\"|'|`)/g, (match: string): string => ({"&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#x27;", "`": "&#x60;"})[match]);
    }

    function GetHeader(token: string, name: string): any {
        var headers: { [header: string]: string } = {};
        headers[name] = token;
        return { headers: headers };
    }

    function GetOrigin(url: string): IOrigin {
        var a: HTMLAnchorElement = document.createElement("a");
        a.href = url;
        return {
            protocol: a.protocol,
            hostname: a.hostname,
            port: a.port
        };
    }

    function SameOrigin(url1: string, url2: string): boolean {
        //https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy
        //Is there a better origin check than this?
        var origin1: IOrigin = GetOrigin(url1);
        var origin2: IOrigin = GetOrigin(url2);

        return origin1 && origin2
            && origin1.protocol === origin2.protocol
            && origin1.hostname === origin2.hostname
            && origin1.port === origin2.port
            && origin1.protocol !== "file:";
    }

    function AllowedOrigin(url: string, allowedOrigins: string[] = []): boolean {
        if (SameOrigin(url, document.location.href)) {
            return true;
        }

        for (var i: number = 0; i < allowedOrigins.length; i++) {
            if (SameOrigin(url, allowedOrigins[i])) {
                return true;
            }
        }
    }

    export function GetToken(selector: string, url: string, id: string): string {
        var $element: JQuery = $(selector);
        var token: string =
            $element.is("input") ?
                $element.val() :
                $element.is("iframe") && $element.attr("src") === url ?
                    $element.contents().find("#" + id).val() :
                    void 0;

        if (token) {
            return token;
        } else {
            throw new Error("Error loading verification token.");
        }
    }

    export function GetTokenAsync(url: string, id: string, timeout?: number): JQueryPromise<string> {
        var promise: JQueryPromise<string> = PromiseCache[url];
        if (promise) {//TODO: Custom $.ajaxSetting value?
            return promise;
        }

        var dfd: JQueryDeferred<string> = $.Deferred();
        /* jshint bitwise: false */
        var src: string = url + (url.indexOf("?") > -1 ? "&" : "?") + "_=" + parseInt(new Date().getTime().toString() + ((Math.random() * 1e5) | 0), 10).toString(36);
        /* jshint bitwise: true */

        if ($.active++ === 0) {//from jquery/src/ajax.js
            $.event.trigger("ajaxStart");
        }

        var $iframe: JQuery = $("<iframe src='" + HtmlEscape(src) + "' style='height:0;width:0;border:0;padding:0;margin:0;position:absolute;top:0;left:0;' sandbox='allow-same-origin allow-scripts'>")
            .on("load", function (): void {
                var token: string = $(this).contents().find("#" + id).val();
                if (token) {
                    dfd.resolve(token);
                } else {
                    dfd.reject();
                }

                $iframe.remove();//This doesn't work without sandbox allow-scripts on both the iframe and the CSP header. It might be better to just not remove the iframe.
            })
            .appendTo("body");

        if (timeout > 0) {
            var timeoutHandle = setTimeout((): void => {
                if (dfd.state() === "pending") {
                    dfd.reject();//TODO: Some timeout value?
                    $iframe.remove();
                }
            }, timeout);
            dfd.always((): void => { clearTimeout(timeoutHandle); });
        }

        return PromiseCache[url] = dfd.promise()
            .fail((): void => {
                PromiseCache[url] = null;
                throw new Error("Error loading verification token.");
            })
            .always((): void => {
                if (!(--$.active)) {//from jquery/src/ajax.js //Will this call multiple times?
                    $.event.trigger("ajaxStop");
                }
            });
    };

    export function PreFilter(options: JQueryAjaxSettings, originalOptions: JQueryAjaxSettings, jqXHR: JQueryXHR): void {
        var settings: IAntiForgerySettings = $.extend(true, {}, AntiForgery.Defaults, AntiForgery.Settings, options.AntiForgery);
        if (!settings.enable) {
            return;
        }
        if (settings.methods.indexOf((options.type || "").toUpperCase()) === -1) {
            return;
        }
        if (options.headers && options.headers[settings.header] !== void 0) {
            return;
        }

        //Leaking tokens is bad, we want to make sure they are going to a good origin.
        if (!AllowedOrigin(options.url, settings.allowedOrigins)) {
            throw new Error("The requested url origin is not allowed to prevent token leaking.");
        }

        //This allows us to 'pre-load' a token/iframe.  Synchronous calls will work with this method.
        if (settings.allow.indexOf("static") > -1 && settings.staticSelector) {
            var token = AntiForgery.GetToken(settings.staticSelector, settings.url, settings.id);
            $.extend(true, options, GetHeader(token, settings.header));
            return;
        } else if (settings.allow.indexOf("dynamic")) {
            //The dynamic loading of a token is by its nature asynchronous.
            if (!options.async) {
                throw new Error("Synchronous AJAX calls do not work with dynamically loaded anti-forgery tokens.");
            }

            jqXHR.abort();//This causes us problems...

            var dfd: JQueryDeferred<{}> = $.Deferred();
            AntiForgery.GetTokenAsync(settings.url, settings.id, settings.timeout).always((token: string): void => {
                var ajax: JQueryXHR = $.ajax($.extend(true, {}, options, GetHeader(token, settings.header)));
                $.extend(true, jqXHR, ajax);

                //Why can't I chain .then calls in order for this? (Apparenly .then retuns a new promise, that's probably the issue.)
                ajax.done(function (): void {
                    $.extend(true, jqXHR, ajax);
                    dfd.resolveWith(this, <any>arguments);
                }).fail(function (): void {
                    $.extend(true, jqXHR, ajax);
                    dfd.rejectWith(this, <any>arguments);
                });
            });

            dfd.promise(jqXHR);
        }
    }
}

$.antiForgery = AntiForgery;
$.ajaxPrefilter(AntiForgery.PreFilter);