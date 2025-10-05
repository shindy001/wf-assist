import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
    input: "./../server/server.host/wfAssist-api.json",
    output: {
        path: "src/api",
        lint: null,
    },
    plugins: [
        {
            name: "@tanstack/svelte-query",
            queryOptions: {
                name: "{{name}}Query",
            },
        },
    ],
});
