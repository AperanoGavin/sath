// libs/auth/src/lib/infrastructure/CognitoAuthService.ts
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AUTH_SERVICE, IAuthService, UserInfo } from '../domain/IAuthService';

import { cognitoEnv } from './cognito.env';

@Injectable()
export class CognitoAuthService implements IAuthService {
  // On reprend directement les valeurs de cognitoEnv
  private readonly domain   = cognitoEnv.domain;
  private readonly clientId = cognitoEnv.clientId;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  /** URI de callback telle que configurée dans cognitoEnv */
  private get redirectUri(): string {
    return isPlatformBrowser(this.platformId)
      ? cognitoEnv.redirectUri
      : '';
  }

  /** URI de déconnexion telle que configurée dans cognitoEnv */
  private get logoutUri(): string {
    return isPlatformBrowser(this.platformId)
      ? cognitoEnv.logoutUri
      : '';
  }

  login(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const params = new URLSearchParams({
      client_id:     this.clientId,
      response_type: 'code',
      scope:         'openid email',
      redirect_uri:  this.redirectUri,
    });


    window.location.href = `${this.domain}/login?${params.toString()}`;
  }

  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    if (!isPlatformBrowser(this.platformId)) {
      throw new Error('handleCallback uniquement en navigateur');
    }

    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) throw new Error('OAuth2 code manquant');

    const tokenEndpoint = `${this.domain}/oauth2/token`;
    const bodyParams = new HttpParams()
      .set('grant_type', 'authorization_code')
      .set('client_id', this.clientId)
      .set('redirect_uri', this.redirectUri)
      .set('code', code);

    const res = await this.http
      .post<{ id_token: string }>(tokenEndpoint, bodyParams.toString(), {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      })
      .toPromise();

    const idToken = res!.id_token;
    localStorage.setItem('id_token', idToken);

    const payload = JSON.parse(atob(idToken.split('.')[1]));
    return {
      username: payload['cognito:username'],
      email:    payload.email,
    };
  }

  logout(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const params = new URLSearchParams({
      client_id:  this.clientId,
      logout_uri: this.logoutUri,
    });

    window.location.href = `${this.domain}/logout?${params.toString()}`;
  }

  async getCurrentUser(): Promise<UserInfo | null> {
    if (!isPlatformBrowser(this.platformId)) return null;

    const idToken = localStorage.getItem('id_token');
    if (!idToken) return null;

    try {
      const payload = JSON.parse(atob(idToken.split('.')[1]));
      return {
        username: payload['cognito:username'],
        email:    payload.email,
      };
    } catch {
      return null;
    }
  }
}

export const COGNITO_AUTH_PROVIDER = [
  { provide: AUTH_SERVICE, useClass: CognitoAuthService },
];
