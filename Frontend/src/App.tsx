import { BrowserRouter } from "react-router-dom";
import { Layout, TokenSync } from "@/components";
import AppRoutes from "@/routes/AppRoutes";
import { AuthProvider } from "@/providers/AuthProvider";
import { ToastProvider } from "@/providers/ToastProvider";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <TokenSync />
        <ToastProvider>
          <Layout>
            <AppRoutes />
          </Layout>
        </ToastProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
