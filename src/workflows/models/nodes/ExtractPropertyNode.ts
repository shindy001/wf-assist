export interface ExtractPropertyNode extends Record<string, unknown> {
    path?: string;
    targetId?: string;
}