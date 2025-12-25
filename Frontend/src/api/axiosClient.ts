import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';

const baseURL = import.meta.env.VITE_API_BASE_URL || '/api';

export const axiosClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
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


axiosClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response) {
      const status = error.response.status;
      
      switch (status) {
        case 401:
          console.error('Unauthorized access - please log in again');
          break;
        case 403:
          console.error('Access forbidden - insufficient permissions');
          break;
        case 404:
          console.error('Resource not found');
          break;
        case 500:
          console.error('Server error - please try again later');
          break;
        default:
          console.error(`HTTP Error ${status}:`, error.response.data);
      }
    } else if (error.request) {
      console.error('Network error - please check your connection');
    } else {
      console.error('Request failed:', error.message);
    }

    return Promise.reject(error);
  }
);
