export const API_AUTH_URL = "/auth";

export interface LoginData {
  email: string;
  password: string;
}

export interface AuthResponse {
  authUserId: string;
  userId: string;
  email: string;
  roles: string[];
  permissions: string[];
  message?: string;
}

export async function loginUser(data: LoginData): Promise<AuthResponse> {
  const response = await fetch(`${API_AUTH_URL}/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Login failed");
  }

  return response.json();
}

export async function getCurrentUser(): Promise<AuthResponse> {
  const response = await fetch(`${API_AUTH_URL}/me`, {
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch current user");
  }

  return response.json();
}

export async function logout(): Promise<void> {
  const response = await fetch(`${API_AUTH_URL}/logout`, {
    method: "POST",
    credentials: "include",
  });

  if (!response.ok) {
    // Even if logout fails on server, we should clear client state
    console.error("Logout failed on server, but clearing client state");
  }
}
