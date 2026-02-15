import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
  input: "./../server/_server.host.openapi/wfAssist-api.json",
  output: {
    path: "src/api",
    lint: null,
  },
  plugins: [
    {
      name: "@hey-api/client-fetch",
      runtimeConfigPath: "./../apiClientConfig.ts",
    },
  ],
});
