import { BrowserRouter } from "react-router-dom";
import { AppRoutes } from "@/app/routes";
import { AuthProvider, AxiosInterceptorProvider } from "@/app/providers";
import { Toaster } from "@/shared/ui";

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
