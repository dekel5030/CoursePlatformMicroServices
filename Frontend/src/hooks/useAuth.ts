import { createContext, useContext } from 'react';

export interface AuthContextValue {
  user: {
    profile: {
      preferred_username?: string;
      email?: string;
      name?: string;
    };
    access_token?: string;
  } | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: Error | null;
  login: () => Promise<void>;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
