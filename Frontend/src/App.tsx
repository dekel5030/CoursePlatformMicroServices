import { BrowserRouter } from "react-router-dom";
import AppRoutes from "@/routes/AppRoutes";
import { AuthProvider, AxiosInterceptorProvider } from "@/providers";
import { Toaster } from "@/components";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AxiosInterceptorProvider>
          <AppRoutes />
          <Toaster />
        </AxiosInterceptorProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
