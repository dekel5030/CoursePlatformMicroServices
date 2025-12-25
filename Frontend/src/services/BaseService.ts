import { apiClient } from '@/api/axiosClient';
import { type AxiosResponse } from 'axios';

export abstract class BaseService {
  protected async get<T>(endpoint: string): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.get(endpoint);
    return response.data;
  }

  protected async post<T>(endpoint: string, data?: unknown): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.post(endpoint, data);
    return response.data;
  }

  protected async put<T>(endpoint: string, data?: unknown): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.put(endpoint, data);
    return response.data;
  }

  protected async delete<T>(endpoint: string): Promise<T | void> {
    const response: AxiosResponse<T> = await apiClient.delete(endpoint);
    if (response.status === 204) {
      return undefined;
    }
    return response.data;
  }

  protected async patch<T>(endpoint: string, data?: unknown): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.patch(endpoint, data);
    return response.data;
  }
}
