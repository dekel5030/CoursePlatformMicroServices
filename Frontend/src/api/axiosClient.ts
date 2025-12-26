import axios, { type AxiosError, type InternalAxiosRequestConfig } from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL || "/api";

export const axiosClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Store the logout callback to be set by the AxiosInterceptorProvider
let logoutCallback: (() => void) | null = null;

export function setLogoutCallback(callback: () => void) {
  logoutCallback = callback;
}

axiosClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const oidcAuthority = import.meta.env.VITE_OIDC_AUTHORITY;
    const oidcClientId = import.meta.env.VITE_OIDC_CLIENT_ID;

    if (!oidcAuthority || !oidcClientId) {
      return config;
    }

    const storageKey = `oidc.user:${oidcAuthority}:${oidcClientId}`;
    const oidcStorage = sessionStorage.getItem(storageKey);

    if (oidcStorage) {
      try {
        const userData = JSON.parse(oidcStorage);

        if (
          userData &&
          typeof userData === "object" &&
          "access_token" in userData
        ) {
          const token = userData.access_token;

          if (typeof token === "string" && token.length > 0) {
            config.headers.Authorization = `Bearer ${token}`;
          }
        }
      } catch (error) {
        console.error("Failed to parse OIDC user data:", error);
      }
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

export interface ApiErrorResponse {
  message: string;
  errors?: Record<string, string[]>;
  code?: string;
}

axiosClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ProblemDetails>) => {
    const data = error.response?.data;

    const apiError: ApiErrorResponse = {
      message: "An unexpected error occurred",
    };

    if (data) {
      if (data.errors) {
        apiError.errors = data.errors;
        apiError.message = "Please fix the validation errors.";
      } else if (data.detail) {
        apiError.message = data.detail;
      } else if (data.title) {
        apiError.message = data.title;
      }
    }

    // Handle 401 Unauthorized - trigger logout and clear state
    if (error.response?.status === 401) {
      apiError.message = "Session expired. Please log in again.";
      
      // Trigger OIDC logout if callback is available
      if (logoutCallback) {
        logoutCallback();
      }
    }

    return Promise.reject(apiError);
  }
);
