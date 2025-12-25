export const keycloakConfig = {
  url: import.meta.env.VITE_KEYCLOAK_URL || 'http://localhost:8080',
  realm: import.meta.env.VITE_KEYCLOAK_REALM || 'course-platform',
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID || 'frontend',
  redirectUri: import.meta.env.VITE_REDIRECT_URI || 'http://localhost:5067',
};

export const apiConfig = {
  gatewayUrl: import.meta.env.VITE_API_GATEWAY_URL || '/api',
};
