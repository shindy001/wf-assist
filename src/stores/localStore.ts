import {isBrowser} from "../utils/platformUtils";

export function useLocalStore() {
    if (!isBrowser()) {
        throw new Error("LocalStore cannot be used outside of browser")
    }

    return {
        setItem: (key: string, data: any) => {
            localStorage.setItem(key, JSON.stringify(data));
        },
        getItem: <TResult>(key: string): TResult | undefined => {
            const json = localStorage.getItem(key);
            let data: TResult | undefined = undefined;
            if (json) {
                data = JSON.parse(json);
            }

            return data;
        }
    }
}