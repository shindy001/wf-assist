import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
  input: "./../server/_AspNetCore.Host/wfAssist-api.json",
  output: {
    path: "src/lib/api",
    postProcess: [],
  },
  plugins: [
    {
      name: "@hey-api/client-fetch",
      runtimeConfigPath: "./src/apiClientConfig.js",
    },
  ],
});
