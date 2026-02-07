export interface User {
  id: string;
  email: string;
  firstName?: string | null;
  lastName?: string | null;
  dateOfBirth?: string | null;
  phoneNumber?: string | null;
  avatarUrl?: string | null;
  bio?: string | null;
  linkedInUrl?: string | null;
  gitHubUrl?: string | null;
  twitterUrl?: string | null;
  websiteUrl?: string | null;
  isLecturer?: boolean;
}
