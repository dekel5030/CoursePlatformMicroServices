import { useQuery } from "@tanstack/react-query";
import { fetchAllUsers } from "../api/Iam";

export function useUsers() {
  return useQuery({
    queryKey: ["auth", "users"],
    queryFn: () => fetchAllUsers(),
  });
}
