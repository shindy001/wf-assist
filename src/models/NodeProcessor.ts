export interface NodeProcessor<T> {
    process: (node: T) => void;
}