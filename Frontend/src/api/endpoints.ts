const USERSERVICE_BASE_URL =
  import.meta.env.VITE_USERSERVICE_BASE_URL || "http://localhost:4001";
const COURSESERVICE_BASE_URL =
  import.meta.env.VITE_COURSESERVICE_BASE_URL || "http://localhost:4002";
const AUTHSERVICE_BASE_URL =
  import.meta.env.VITE_AUTHSERVICE_BASE_URL || "http://localhost:4003";

export const API = {
  USERS: `${USERSERVICE_BASE_URL}`,
  COURSES: `${COURSESERVICE_BASE_URL}`,
  AUTH: `${AUTHSERVICE_BASE_URL}`,
};
