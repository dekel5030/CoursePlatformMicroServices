export interface UpdateUserRequest {
  firstName?: string | null;
  lastName?: string | null;
  phoneNumber?: { countryCode: string; number: string } | null;
  dateOfBirth?: string | null;
}
