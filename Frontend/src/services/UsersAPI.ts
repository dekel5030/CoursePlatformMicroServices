const API_USERS_URL = "/api";

export type Fetcher = (
  input: RequestInfo | URL,
  init?: RequestInit
) => Promise<Response>;

export interface User {
  id: string;
  email: string;
  firstName?: string | null;
  lastName?: string | null;
  dateOfBirth?: string | null;
  phoneNumber?: string | null;
}

export interface UpdateUserRequest {
  firstName?: string | null;
  lastName?: string | null;
  phoneNumber?: { countryCode: string; number: string } | null;
  dateOfBirth?: string | null;
}

// קבלת fetcher כפרמטר שני
export async function fetchUserById(
  id: string,
  fetcher: Fetcher
): Promise<User> {
  const response = await fetcher(`${API_USERS_URL}/users/${id}`);
  if (!response.ok) throw new Error("Failed to fetch user");
  return await response.json();
}

export async function updateUser(
  id: string,
  data: UpdateUserRequest,
  fetcher: Fetcher
): Promise<User> {
  const response = await fetcher(`${API_USERS_URL}/users/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) throw new Error("Failed to update user");
  return await response.json();
}
