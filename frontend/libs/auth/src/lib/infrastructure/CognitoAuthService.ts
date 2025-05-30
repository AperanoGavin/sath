// libs/auth/src/lib/infrastructure/CognitoAuthService.ts
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { AUTH_SERVICE, IAuthService, UserInfo } from '../domain/IAuthService';

/**
 * Implémentation du service d'authentification via Amazon Cognito
 */
@Injectable()
export class CognitoAuthService implements IAuthService {
  private readonly clientId = '622o4sqg2h9e4t4gu47fap5vi0';
  private readonly domain   = 'eu-west-1czfshuw1u.auth.eu-west-1.amazoncognito.com';

  constructor(
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  /** URL de callback calculée dynamiquement en navigateur */
  private get redirectUri(): string {
    if (isPlatformBrowser(this.platformId)) {
      return `${window.location.origin}/callback`;
    }
    return '';
  }

  login(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const params = new URLSearchParams({
      client_id: this.clientId,
      response_type: 'code',
      scope: 'openid profile email',
      redirect_uri: this.redirectUri
    });

    window.location.href = `https://${this.domain}/login?${params.toString()}`;
  }

  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    if (!isPlatformBrowser(this.platformId)) {
      throw new Error('handleCallback appelé en SSR');
    }
    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) throw new Error('Code OAuth2 manquant');

    const tokenEndpoint = `https://${this.domain}/oauth2/token`;
    const bodyParams = new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: this.clientId,
      code,
      redirect_uri: this.redirectUri
    });

    const response = await fetch(tokenEndpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: bodyParams.toString()
    });
    const { id_token } = await response.json();
    const payload = JSON.parse(atob(id_token.split('.')[1]));

    return {
      username: payload['cognito:username'],
      email: payload.email,
    };
  }

  logout(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const params = new URLSearchParams({
      client_id: this.clientId,
      logout_uri: window.location.origin
    });
    window.location.href = `https://${this.domain}/logout?${params.toString()}`;
  }

  async getCurrentUser(): Promise<UserInfo | null> {
    if (!isPlatformBrowser(this.platformId)) return null;

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

export const COGNITO_AUTH_PROVIDER = [
  { provide: AUTH_SERVICE, useClass: CognitoAuthService }
];
