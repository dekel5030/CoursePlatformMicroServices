import { BrowserRouter } from "react-router-dom";
import AppRoutes from "@/routes/AppRoutes";
import { AuthProvider } from "@/providers/AuthProvider";
import { AxiosInterceptorProvider } from "@/providers/AxiosInterceptorProvider";
import { PermissionsProvider } from "@/providers/PermissionsProvider";
import { Toaster } from "@/components/ui";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AxiosInterceptorProvider>
          <PermissionsProvider>
            <AppRoutes />
            <Toaster />
          </PermissionsProvider>
        </AxiosInterceptorProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}