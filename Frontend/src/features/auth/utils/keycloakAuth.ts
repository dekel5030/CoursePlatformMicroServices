/**
 * Keycloak Authentication Utilities
 * Handles custom login/register flows with PKCE
 */

// Get the OIDC configuration from environment variables
export const KEYCLOAK_CONFIG = {
  authority: import.meta.env.VITE_OIDC_AUTHORITY || "http://localhost:8080/realms/course-platform",
  clientId: import.meta.env.VITE_OIDC_CLIENT_ID || "frontend",
  redirectUri: window.location.origin,
};

// Keycloak endpoints
export const KEYCLOAK_ENDPOINTS = {
  registration: `${KEYCLOAK_CONFIG.authority}/protocol/openid-connect/registrations`,
  token: `${KEYCLOAK_CONFIG.authority}/protocol/openid-connect/token`,
  auth: `${KEYCLOAK_CONFIG.authority}/protocol/openid-connect/auth`,
  logout: `${KEYCLOAK_CONFIG.authority}/protocol/openid-connect/logout`,
};

/**
 * Generate random string for PKCE code verifier
 */
function generateRandomString(length: number): string {
  const charset = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~';
  const randomValues = new Uint8Array(length);
  crypto.getRandomValues(randomValues);
  return Array.from(randomValues, (byte) => charset[byte % charset.length]).join('');
}

/**
 * Generate PKCE code challenge from verifier
 */
async function generateCodeChallenge(verifier: string): Promise<string> {
  const encoder = new TextEncoder();
  const data = encoder.encode(verifier);
  const hash = await crypto.subtle.digest('SHA-256', data);
  const base64 = btoa(String.fromCharCode(...new Uint8Array(hash)))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=/g, '');
  return base64;
}

/**
 * Generate PKCE pair (code_verifier and code_challenge)
 */
export async function generatePKCE(): Promise<{ verifier: string; challenge: string }> {
  const verifier = generateRandomString(128);
  const challenge = await generateCodeChallenge(verifier);
  return { verifier, challenge };
}

/**
 * Store PKCE verifier in session storage
 */
export function storePKCEVerifier(verifier: string): void {
  sessionStorage.setItem('pkce_code_verifier', verifier);
}

/**
 * Get PKCE verifier from session storage
 */
export function getPKCEVerifier(): string | null {
  return sessionStorage.getItem('pkce_code_verifier');
}

/**
 * Clear PKCE verifier from session storage
 */
export function clearPKCEVerifier(): void {
  sessionStorage.removeItem('pkce_code_verifier');
}

/**
 * Generate and store state parameter for CSRF protection
 */
export function generateAndStoreState(): string {
  const state = generateRandomString(32);
  sessionStorage.setItem('oauth_state', state);
  return state;
}

/**
 * Get stored state parameter
 */
export function getStoredState(): string | null {
  return sessionStorage.getItem('oauth_state');
}

/**
 * Clear stored state parameter
 */
export function clearStoredState(): void {
  sessionStorage.removeItem('oauth_state');
}

/**
 * Store the original URL before redirecting to login
 */
export function storeRedirectUrl(url?: string): void {
  const redirectUrl = url || window.location.pathname + window.location.search;
  sessionStorage.setItem('auth_redirect_url', redirectUrl);
}

/**
 * Get and clear the stored redirect URL
 */
export function getAndClearRedirectUrl(): string {
  const url = sessionStorage.getItem('auth_redirect_url') || '/catalog';
  sessionStorage.removeItem('auth_redirect_url');
  return url;
}

/**
 * Initiate login with PKCE flow
 */
export async function initiateLoginWithPKCE(options?: {
  idpHint?: string;
  rememberMe?: boolean;
}): Promise<void> {
  const { verifier, challenge } = await generatePKCE();
  storePKCEVerifier(verifier);
  
  const state = generateAndStoreState();
  
  const params = new URLSearchParams({
    client_id: KEYCLOAK_CONFIG.clientId,
    redirect_uri: KEYCLOAK_CONFIG.redirectUri,
    response_type: 'code',
    scope: 'openid profile email',
    code_challenge: challenge,
    code_challenge_method: 'S256',
    state,
  });

  if (options?.idpHint) {
    params.set('kc_idp_hint', options.idpHint);
  }

  // Note: Keycloak doesn't have a standard "remember_me" parameter in the auth request
  // This would need to be handled via session management settings in Keycloak

  window.location.href = `${KEYCLOAK_ENDPOINTS.auth}?${params.toString()}`;
}

/**
 * Initiate registration with PKCE flow
 */
export async function initiateRegistrationWithPKCE(options?: {
  idpHint?: string;
}): Promise<void> {
  const { verifier, challenge } = await generatePKCE();
  storePKCEVerifier(verifier);
  
  const state = generateAndStoreState();
  
  const params = new URLSearchParams({
    client_id: KEYCLOAK_CONFIG.clientId,
    redirect_uri: KEYCLOAK_CONFIG.redirectUri,
    response_type: 'code',
    scope: 'openid profile email',
    code_challenge: challenge,
    code_challenge_method: 'S256',
    state,
    kc_action: 'register',
  });

  if (options?.idpHint) {
    params.set('kc_idp_hint', options.idpHint);
  }

  window.location.href = `${KEYCLOAK_ENDPOINTS.auth}?${params.toString()}`;
}
