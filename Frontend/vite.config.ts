import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/auth": {
        target: "https://localhost:7233",
        changeOrigin: true,
        secure: false,
      },
      "/courseservice": {
        target: "https://localhost:7171",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/courseservice/, ""),
      },
      "/userservice": {
        target: "https://localhost:7079",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/userservice/, ""),
      },
    },
  },
});
