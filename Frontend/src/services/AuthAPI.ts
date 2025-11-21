const API_AUTH_URL =
  import.meta.env.VITE_API_AUTH_URL || "https://localhost:7233/auth";

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
  // TODO: Implement logout endpoint
  // For now, just clear local state
  return Promise.resolve();
}
