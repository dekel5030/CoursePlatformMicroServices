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
  accessToken: string;
  message?: string;
}

export async function login(data: LoginData): Promise<AuthResponse> {
  const response = await fetch(`${API_AUTH_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
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
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch current user");

  return response.json();
}

export async function logout(refreshToken?: string): Promise<void> {
  await fetch(`${API_AUTH_URL}/logout`, {
    method: "POST",
    headers: refreshToken ? { Authorization: `Bearer ${refreshToken}` } : {},
    credentials: "include",
  });
}

export async function refreshAccessToken(): Promise<string> {
  const response = await fetch(`${API_AUTH_URL}/refresh-token`, {
    method: "POST",
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to refresh access token");

  const data = await response.json();
  return data.accessToken;
}
