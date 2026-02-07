import apiClient from '@/lib/api-client';
import type {
  LecturerProfile,
  CreateLecturerProfileRequest,
} from '../types';

export const lecturerProfilesApi = {
  getLecturerProfile: async (userId: string): Promise<LecturerProfile> => {
    const response = await apiClient.get(`/users/${userId}/lecturer-profile`);
    return response.data;
  },

  createLecturerProfile: async (
    userId: string,
    data: CreateLecturerProfileRequest
  ): Promise<LecturerProfile> => {
    const response = await apiClient.post(
      `/users/${userId}/lecturer-profile`,
      data
    );
    return response.data;
  },
};
