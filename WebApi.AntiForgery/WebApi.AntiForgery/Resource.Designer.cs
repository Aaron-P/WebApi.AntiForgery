﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApi.AntiForgery {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WebApi.AntiForgery.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;&lt;html&gt;&lt;head&gt;&lt;meta charset=&quot;utf-8&quot;&gt;&lt;title&gt;&amp;#x1F512;&lt;/title&gt;&lt;/head&gt;&lt;input type=&quot;hidden&quot; id=&quot;{0}&quot; value=&quot;{1}&quot;&gt;&lt;/html&gt;&lt;!--{2}--&gt;.
        /// </summary>
        internal static string TokenTemplateHtml {
            get {
                return ResourceManager.GetString("TokenTemplateHtml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}{{&quot;{1}&quot;:&quot;{2}&quot;,&quot;{3}&quot;:0}}.
        /// </summary>
        internal static string TokenTemplateJson {
            get {
                return ResourceManager.GetString("TokenTemplateJson", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot; ?&gt;&lt;!DOCTYPE {0} [&lt;!ELEMENT VerificationToken (#PCDATA)&gt;]&gt;&lt;{0}&gt;&lt;![CDATA[{1}]]&gt;&lt;/{0}&gt;&lt;!--{2}--&gt;.
        /// </summary>
        internal static string TokenTemplateXml {
            get {
                return ResourceManager.GetString("TokenTemplateXml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unknown error occurred while attempting to generate the anti-forgery token..
        /// </summary>
        internal static string TokenUnknownError {
            get {
                return ResourceManager.GetString("TokenUnknownError", resourceCulture);
            }
        }
    }
}