import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';

/**
 * Axios client with automatic JWT token injection and global error handling
 */

// Base API URL from environment or use proxy
const baseURL = import.meta.env.VITE_API_BASE_URL || '/api';

// Create axios instance
export const axiosClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Request Interceptor: Automatically attach JWT token from sessionStorage/localStorage
 */
axiosClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Ensure OIDC environment variables are configured
    const oidcAuthority = import.meta.env.VITE_OIDC_AUTHORITY;
    const oidcClientId = import.meta.env.VITE_OIDC_CLIENT_ID;
    
    if (!oidcAuthority || !oidcClientId) {
      // OIDC not configured, skip token injection
      return config;
    }
    
    // Try to get token from sessionStorage (OIDC stores it there)
    const storageKey = `oidc.user:${oidcAuthority}:${oidcClientId}`;
    const oidcStorage = sessionStorage.getItem(storageKey);
    
    if (oidcStorage) {
      try {
        const userData = JSON.parse(oidcStorage);
        
        // Validate userData structure
        if (userData && typeof userData === 'object' && 'access_token' in userData) {
          const token = userData.access_token;
          
          if (typeof token === 'string' && token.length > 0) {
            config.headers.Authorization = `Bearer ${token}`;
          }
        }
      } catch (error) {
        console.error('Failed to parse OIDC user data:', error);
      }
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response Interceptor: Handle global errors
 */
axiosClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    // Handle different error scenarios
    if (error.response) {
      const status = error.response.status;
      
      switch (status) {
        case 401:
          // Unauthorized - token expired or invalid
          console.error('Unauthorized access - please log in again');
          // Could trigger a redirect to login here
          break;
        case 403:
          // Forbidden - user doesn't have permission
          console.error('Access forbidden - insufficient permissions');
          break;
        case 404:
          // Not found
          console.error('Resource not found');
          break;
        case 500:
          // Server error
          console.error('Server error - please try again later');
          break;
        default:
          console.error(`HTTP Error ${status}:`, error.response.data);
      }
    } else if (error.request) {
      // Network error
      console.error('Network error - please check your connection');
    } else {
      // Other errors
      console.error('Request failed:', error.message);
    }

    return Promise.reject(error);
  }
);
