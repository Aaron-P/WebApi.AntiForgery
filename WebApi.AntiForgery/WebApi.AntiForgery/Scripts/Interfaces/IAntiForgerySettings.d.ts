interface IAntiForgerySettings {
    allow?: string[],
    allowedOrigins?: string[],
    enable?: boolean,
    header?: string,
    id?: string,
    staticSelector?: string,
    timeout?: number,
    methods?: string[],
    url?: string
}