// Re-export auth hooks - these depend on context so they'll stay in features for now
// but can be accessed from shared for convenience
export { useAuth, useHasPermission, useHasRole } from "@/features/auth/hooks";
export { useHateoasLink } from "./useHateoasLink";
