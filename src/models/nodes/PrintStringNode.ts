export interface PrintStringNode extends Record<string, unknown> {
    useLogger: boolean;
    targetId: string | undefined;
}