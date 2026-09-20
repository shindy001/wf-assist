import type { CreateClientConfig } from "$lib/api/client.gen";

export const createClientConfig: CreateClientConfig = (config) => ({
  ...config,
  baseUrl: import.meta.env.VITE_API_ADDRESS,
});
