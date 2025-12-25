import { BaseService } from './BaseService';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string | null;
  bio: string | null;
  profilePictureUrl: string | null;
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  bio?: string;
  profilePictureUrl?: string;
}

class UserServiceClass extends BaseService {
  async getUserById(userId: string): Promise<User> {
    return await this.get<User>(`/users/${userId}`);
  }

  async updateUser(userId: string, request: UpdateUserRequest): Promise<User> {
    return await this.put<User>(`/users/${userId}`, request);
  }
}

export const UserService = new UserServiceClass();
