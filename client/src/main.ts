import { mount } from "svelte";
import App from "./App.svelte";
import { client } from "$api/client.gen";

// api client config, client is available at /src/api
client.setConfig({
  baseUrl: import.meta.env.VITE_API_ADDRESS,
});

const app = mount(App, {
  target: document.getElementById("app")!,
});
