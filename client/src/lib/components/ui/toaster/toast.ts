import { toast as toastlib } from "svelte-sonner";

export function info(message: string) {
  toastlib.info(message);
}

export function warning(message: string) {
  toastlib.error(message);
}

export function error(message: string) {
  toastlib.error(message);
}
