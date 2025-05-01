export interface RequestNode extends Record<string, unknown> {
    url?: string,
    requestType?: string,
    requestBody?: string,
}