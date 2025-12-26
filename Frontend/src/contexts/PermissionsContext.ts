import { createContext } from "react";
import type { CurrentUserDto, PermissionDto } from "@/services/currentUser.service";

export interface PermissionsContextValue {
  user: CurrentUserDto | null;
  permissions: PermissionDto[];
  isLoading: boolean;
  error: Error | null;
}

export const PermissionsContext = createContext<PermissionsContextValue | undefined>(undefined);
