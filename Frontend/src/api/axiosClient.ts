import axios, { type AxiosError, type InternalAxiosRequestConfig } from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL || "/api";

export const axiosClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

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

    if (error.response?.status === 401) {
      apiError.message = "Session expired. Please log in again.";
    }

    return Promise.reject(apiError);
  }
);

// axiosClient.interceptors.response.use(
//   (response) => response,
//   (error: AxiosError<ProblemDetails>) => {
//     let errorMessage = "An unexpected error occurred";

//     if (error.response) {
//       const data = error.response.data;

//       if (data) {
//         if (data.errors) {
//           errorMessage = Object.values(data.errors).flat().join(", ");
//         } else if (data.detail) {
//           errorMessage = data.detail;
//         } else if (data.title) {
//           errorMessage = data.title;
//         }
//       }

//       switch (error.response.status) {
//         case 401:
//           console.error("Unauthorized - Redirecting to login...");
//           break;
//         case 403:
//           errorMessage = "You don't have permission to perform this action";
//           break;
//       }
//     } else if (error.request) {
//       errorMessage =
//         "No response from server. Please check your network connection.";
//     } else {
//       errorMessage = error.message;
//     }

//     return Promise.reject(errorMessage);
//   }
// );

// axiosClient.interceptors.response.use(
//   (response) => response,
//   (error: AxiosError) => {
//     if (error.response) {
//       const status = error.response.status;

//       switch (status) {
//         case 401:
//           console.error('Unauthorized access - please log in again');
//           break;
//         case 403:
//           console.error('Access forbidden - insufficient permissions');
//           break;
//         case 404:
//           console.error('Resource not found');
//           break;
//         case 500:
//           console.error('Server error - please try again later');
//           break;
//         default:
//           console.error(`HTTP Error ${status}:`, error.response.data);
//       }
//     } else if (error.request) {
//       console.error('Network error - please check your connection');
//     } else {
//       console.error('Request failed:', error.message);
//     }

//     return Promise.reject(error);
//   }
// );
