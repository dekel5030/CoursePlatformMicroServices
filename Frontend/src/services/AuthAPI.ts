export const API_AUTH_URL = "/auth";

export interface LoginData {
  email: string;
  password: string;
}

export interface RegisterData {
  email: string;
  password: string;
  fullname: string;
  userName?: string;
  phoneNumber?: string;
}

/**
 * User data returned from the backend (matches CurrentUserDto)
 * JWT tokens are NOT stored client-side - they are managed via HttpOnly cookies
 */
export interface AuthUser {
  id: string;
  email: string;
  userName?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  avatarUrl?: string | null;
}

export async function login(data: LoginData): Promise<AuthUser> {
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

export async function register(data: RegisterData): Promise<AuthUser> {
  const response = await fetch(`${API_AUTH_URL}/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Registration failed");
  }

  return response.json();
}

export async function getCurrentUser(): Promise<AuthUser> {
  const response = await fetch(`${API_AUTH_URL}/me`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch current user");

  return response.json();
}

export async function logout(): Promise<void> {
  await fetch(`${API_AUTH_URL}/logout`, {
    method: "POST",
    credentials: "include",
  });
}
