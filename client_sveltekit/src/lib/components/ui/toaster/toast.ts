import { toast as toastlib } from "svelte-sonner";

export function info(message: string, data?: { description: string }) {
  toastlib.info(message, data);
}

export function warning(message: string) {
  toastlib.error(message);
}

export function error(message: string) {
  toastlib.error(message);
}
