export class ErrorDetail {
    name: string;
    message: string;

    constructor(name: string, message: string) {
        this.name = name;
        this.message = message;
    }
}

export class NotFoundError {
}

export class AlreadyExistsError {
}

export type Either<T0, T1> = T0 | T1;
