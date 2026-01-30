import type { CurrentUserDto } from "../types/CurrentUserDto";
import type { UserModel } from "../types/UserModel";

/**
 * Maps a CurrentUserDto to a UserModel
 */
export function mapToUserModel(dto: CurrentUserDto): UserModel {
  return {
    id: dto.id,
    email: dto.email,
    firstName: dto.firstName,
    lastName: dto.lastName,
    fullName: `${dto.firstName} ${dto.lastName}`,
    roles: dto.roles,
    permissions: dto.permissions,
  };
}
