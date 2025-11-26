import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  const authUrl =
    process.env.services__authservice__https__0 ||
    process.env.services__authservice__http__0;
  const usersUrl =
    process.env.services__userservice__https__0 ||
    process.env.services__userservice__http__0;
  const coursesUrl =
    process.env.services__courseservice__https__0 ||
    process.env.services__courseservice__http__0;

  return {
    plugins: [react()],
    server: {
      host: "0.0.0.0",
      port: process.env.PORT ? parseInt(process.env.PORT) : 5173,
      proxy: {
        "/auth": {
          target: authUrl,
          changeOrigin: true,
          secure: false,
        },
        "/userservice": {
          target: usersUrl,
          changeOrigin: true,
          secure: false,
          rewrite: (path) => path.replace(/^\/userservice/, ""),
        },
        "/courseservice": {
          target: coursesUrl,
          changeOrigin: true,
          secure: false,
          rewrite: (path) => path.replace(/^\/courseservice/, ""),
        },
      },
    },
  };
});
