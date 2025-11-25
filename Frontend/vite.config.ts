import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/

export default defineConfig({
  plugins: [react()],
  server: {
    host: true, // מאפשר גישה מחוץ ל-localhost (קריטי לדוקר/aspire)
    port: process.env.PORT ? parseInt(process.env.PORT) : 5173, // שימוש בפורט ש-Aspire מקצה
    strictPort: true, // אם הפורט תפוס - אל תחליף פורט, אלא תכשיל (כדי ש-Aspire ידע שיש בעיה)
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
