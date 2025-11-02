export interface Result<T = never> {
  isSuccessful: boolean;
  error?: string;
  data?: T;
}

export function successful(): Result;
export function successful<T>(data: T): Result<T>;
export function successful<T>(data?: T): Result<T> {
  return {
    isSuccessful: true,
    data: data,
  };
}

export function failed(errorMessage: string): Result {
  return {
    isSuccessful: false,
    error: errorMessage,
  };
}
