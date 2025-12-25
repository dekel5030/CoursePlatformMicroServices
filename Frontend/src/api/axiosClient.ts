import axios, { type AxiosError, type AxiosInstance, type InternalAxiosRequestConfig } from 'axios';
import { apiConfig } from '@/config/app.config';

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  public readonly status: number;
  public readonly problemDetails: ProblemDetails;

  constructor(status: number, problemDetails: ProblemDetails, message?: string) {
    super(message || problemDetails.title || 'API Error');
    this.name = 'ApiError';
    this.status = status;
    this.problemDetails = problemDetails;
  }
}

let getAccessToken: (() => string | undefined) | undefined;

export function setTokenProvider(provider: () => string | undefined) {
  getAccessToken = provider;
}

const axiosInstance: AxiosInstance = axios.create({
  baseURL: apiConfig.gatewayUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

axiosInstance.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = getAccessToken?.();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response) {
      const problemDetails: ProblemDetails = error.response.data as ProblemDetails || {
        status: error.response.status,
        title: error.response.statusText || 'API Error',
      };
      throw new ApiError(error.response.status, problemDetails);
    }
    throw error;
  }
);

export const apiClient = axiosInstance;
