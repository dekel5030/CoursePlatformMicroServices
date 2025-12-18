import { BrowserRouter } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import AppRoutes from "./routes/AppRoutes.tsx";
import { AuthProvider } from "./providers/AuthProvider.tsx";

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
