import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  type ReactNode,
} from "react";
import {
  login as apiLogin,
  logout as apiLogout,
  refreshAccessToken as apiRefreshAccessToken,
  type AuthResponse,
} from "../services/AuthAPI";
import { initializeAuthenticatedFetch } from "../utils/authenticatedFetch";

export type AuthUser = AuthResponse;

interface AuthContextType {
  currentUser: AuthUser | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<AuthResponse>;
  logout: () => Promise<void>;
  refreshAccessToken: () => Promise<string>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface Props {
  children: ReactNode;
}

export function AuthProvider({ children }: Props) {
  const [currentUser, setCurrentUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const logout = useCallback(async () => {
    await apiLogout();
    localStorage.removeItem("currentUser");
    setCurrentUser(null);
  }, []);

  const refreshAccessToken = useCallback(async (): Promise<string> => {
    const newToken = await apiRefreshAccessToken();

    setCurrentUser((prevUser) => {
      if (!prevUser) return prevUser;

      const updatedUser = { ...prevUser, accessToken: newToken };
      localStorage.setItem("currentUser", JSON.stringify(updatedUser));
      return updatedUser;
    });

    return newToken;
  }, []);

  // Initialize authenticated fetch on mount and when dependencies change
  useEffect(() => {
    initializeAuthenticatedFetch(
      () => currentUser?.accessToken,
      refreshAccessToken,
      logout
    );
  }, [currentUser, refreshAccessToken, logout]);

  useEffect(() => {
    const storedUser = localStorage.getItem("currentUser");
    setCurrentUser(storedUser ? (JSON.parse(storedUser) as AuthUser) : null);
    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    try {
      const user = await apiLogin({ email, password });

      setCurrentUser(user);
      localStorage.setItem("currentUser", JSON.stringify(user));

      return user;
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        currentUser,
        isLoading,
        isAuthenticated: currentUser !== null,
        login,
        logout,
        refreshAccessToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
};
