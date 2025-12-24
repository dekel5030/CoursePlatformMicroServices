import { BrowserRouter } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import AppRoutes from "./routes/AppRoutes.tsx";
import { AuthProvider } from "./providers/AuthProvider.tsx";
import { ToastProvider } from "./providers/ToastProvider.tsx";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ToastProvider>
          <Layout>
            <AppRoutes />
          </Layout>
        </ToastProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
