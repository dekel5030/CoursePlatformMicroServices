import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(() => {
  const gatewayUrl =
    process.env.services__gateway__https__0 ||
    process.env.services__gateway__http__0;

  return {
    plugins: [react()],
    server: {
      host: true,
      port: process.env.PORT ? parseInt(process.env.PORT) : 5173,
      proxy: {
        "/api": {
          target: gatewayUrl,
          changeOrigin: true,
          secure: false,
        },
      },
    },
  };
});
