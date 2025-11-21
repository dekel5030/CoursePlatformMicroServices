import { createContext, useContext, useState, useEffect } from "react";
import type { ReactNode } from "react";

interface AuthUser {
  authUserId: string;
  userId: string;
  email: string;
  roles: string[];
  permissions: string[];
}

interface AuthContextType {
  currentUser: AuthUser | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  setCurrentUser: (user: AuthUser | null) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [currentUser, setCurrentUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Fetch current user on mount
  useEffect(() => {
    const fetchCurrentUser = async () => {
      try {
        const API_AUTH_URL = import.meta.env.VITE_API_AUTH_URL || "https://localhost:7233/auth";
        const response = await fetch(`${API_AUTH_URL}/me`, {
          credentials: "include", // Include cookies
        });

        if (response.ok) {
          const user = await response.json();
          setCurrentUser(user);
        } else {
          setCurrentUser(null);
        }
      } catch (error) {
        console.error("Failed to fetch current user:", error);
        setCurrentUser(null);
      } finally {
        setIsLoading(false);
      }
    };

    fetchCurrentUser();
  }, []);

  const logout = () => {
    setCurrentUser(null);
    // TODO: Call logout endpoint to clear cookies
  };

  const isAuthenticated = currentUser !== null;

  return (
    <AuthContext.Provider
      value={{
        currentUser,
        isLoading,
        isAuthenticated,
        setCurrentUser,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
