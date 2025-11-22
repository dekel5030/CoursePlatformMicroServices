const API_USERS_URL = "/userservice/api";

export interface User {
  id: string;
  email: string;
  firstName?: string | null;
  lastName?: string | null;
  dateOfBirth?: string | null;
  phoneNumber?: string | null;
}

export async function fetchUserById(id: string): Promise<User> {
  const response = await fetch(`${API_USERS_URL}/users/${id}`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch user");

  return await response.json();
}

export async function updateUser(
  id: string,
  data: Partial<User>
): Promise<User> {
  const response = await fetch(`${API_USERS_URL}/users/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(data),
  });

  if (!response.ok) throw new Error("Failed to update user");

  return await response.json();
}
