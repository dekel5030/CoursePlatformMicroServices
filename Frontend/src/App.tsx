import { BrowserRouter } from "react-router-dom";
import { Layout } from "@/components";
import AppRoutes from "@/routes/AppRoutes";
import { AuthProvider } from "@/providers/AuthProvider";
import { ToastProvider } from "@/providers/ToastProvider";
import { QueryProvider } from "@/providers/QueryProvider";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <QueryProvider>
          <ToastProvider>
            <Layout>
              <AppRoutes />
            </Layout>
          </ToastProvider>
        </QueryProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
