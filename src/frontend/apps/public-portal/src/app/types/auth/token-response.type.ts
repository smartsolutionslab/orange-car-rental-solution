/**
 * OAuth token response from Keycloak
 */
export interface TokenResponse {
  readonly access_token: string;
  readonly refresh_token: string;
  readonly expires_in: number;
  readonly token_type: string;
}
