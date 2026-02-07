export interface UpdateUserRequest {
  firstName?: string | null;
  lastName?: string | null;
  phoneNumber?: { countryCode: string; number: string } | null;
  dateOfBirth?: string | null;
  avatarUrl?: string | null;
  bio?: string | null;
  linkedInUrl?: string | null;
  gitHubUrl?: string | null;
  twitterUrl?: string | null;
  websiteUrl?: string | null;
}
