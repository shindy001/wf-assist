import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
  input: "./../server/_AspNetCore.Host/wfAssist-api.json",
  output: {
    path: "src/api",
    postProcess: [],
  },
  plugins: [
    {
      name: "@hey-api/client-fetch",
      runtimeConfigPath: "./../apiClientConfig.ts",
    },
  ],
});
