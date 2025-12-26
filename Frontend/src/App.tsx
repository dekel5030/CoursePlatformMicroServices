import { BrowserRouter } from "react-router-dom";
import { Layout } from "@/components/layout";
import AppRoutes from "@/routes/AppRoutes";
import { AuthProvider } from "@/providers/AuthProvider";
import { Toaster } from "@/components/ui";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Layout>
          <AppRoutes />
        </Layout>
        <Toaster />
      </AuthProvider>
    </BrowserRouter>
  );
}
