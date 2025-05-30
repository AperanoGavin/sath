import { Injectable } from '@angular/core';
import { IAuthService, UserInfo, AUTH_SERVICE } from '../domain/IAuthService';

/**
 * Implémentation du service d'authentification via Amazon Cognito
 */
@Injectable()
export class CognitoAuthService implements IAuthService {
  private readonly clientId = '622o4sqg2h9e4t4gu47fap5vi0';
  private readonly domain   = 'eu-west-1czfshuw1u.auth.eu-west-1.amazoncognito.com';
  private readonly redirectUri = window.location.origin + '/callback';

  login(): void {
    const authUrl = `https://${this.domain}/login?` +
      new URLSearchParams({
        client_id: this.clientId,
        response_type: 'code',
        scope: 'openid profile email',
        redirect_uri: this.redirectUri
      }).toString();
    window.location.href = authUrl;
  }

  /**
   * Traite le callback : échange code contre tokens puis décode l'id_token
   * @param callbackUrl URL complète reçue de Cognito
   */
  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) throw new Error('Code OAuth2 manquant');

    const tokenEndpoint = `https://${this.domain}/oauth2/token`;
    const params = new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: this.clientId,
      code,
      redirect_uri: this.redirectUri
    });

    const response = await fetch(tokenEndpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: params.toString()
    });
    const { id_token } = await response.json();

    // Décodage basique du JWT (sans vérification) pour récupérer le profil
    const payload = JSON.parse(atob(id_token.split('.')[1]));
    return {
      username: payload['cognito:username'],
      email: payload.email,
    };
  }

  /** Déconnecte et redirige vers l’URI de logout Cognito */
  logout(): void {
    const logoutUrl = `https://${this.domain}/logout?` +
      new URLSearchParams({
        client_id: this.clientId,
        logout_uri: window.location.origin
      }).toString();
    window.location.href = logoutUrl;
  }

  /** Récupère l’utilisateur courant depuis un token stocké en localStorage */
  async getCurrentUser(): Promise<UserInfo | null> {
    const idToken = localStorage.getItem('id_token');
    if (!idToken) return null;
    try {
      const payload = JSON.parse(atob(idToken.split('.')[1]));
      return {
        username: payload['cognito:username'],
        email: payload.email,
      };
    } catch {
      return null;
    }
  }
}

/**
 * Fourniture de l'implémentation pour l'InjectionToken AUTH_SERVICE
 */
export const COGNITO_AUTH_PROVIDER = [
  { provide: AUTH_SERVICE, useClass: CognitoAuthService }
];
