import { createContext } from "react";
import type { UserModel } from "../types/UserModel";
import type { PermissionDto } from "../types/PermissionDto";

export interface AuthContextValue {
  user: UserModel | null;
  permissions: PermissionDto[];
  isAuthenticated: boolean;
  isLoading: boolean;
  error: Error | null;

  signoutRedirect: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined
);
