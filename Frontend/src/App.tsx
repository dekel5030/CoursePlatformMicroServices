import { BrowserRouter } from "react-router-dom";
import { Layout } from "@/components/layout";
import AppRoutes from "@/routes/AppRoutes";
import { AuthProvider } from "@/providers/AuthProvider";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Layout>
          <AppRoutes />
        </Layout>
      </AuthProvider>
    </BrowserRouter>
  );
}
