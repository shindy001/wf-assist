export interface NodeExecutor<T> {
    execute: (node: T) => void;
}