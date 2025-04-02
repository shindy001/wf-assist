import type {ErrorDetail} from "./ErrorDetail";

export type Result<T = void> = {
    isSuccessful: true;
    data: T;
    error: undefined;
} | {
    isSuccessful: false;
    data: undefined;
    error: ErrorDetail;
};

export const success = <T = void>(data: T): Result<T> => ({
    isSuccessful: true,
    data: data,
    error: undefined,
})

export const failure = (error: ErrorDetail): Result<never> => ({
    isSuccessful: false,
    data: undefined,
    error: error,
})