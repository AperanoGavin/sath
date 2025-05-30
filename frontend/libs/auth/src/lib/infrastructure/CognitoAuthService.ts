// libs/auth/src/lib/infrastructure/CognitoAuthService.ts
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AUTH_SERVICE, IAuthService, UserInfo } from '../domain/IAuthService';
import { defaultEnv, envStateKey, Environment } from '../../../../../apps/parking/src/app/env';

@Injectable()
export class CognitoAuthService implements IAuthService {
  private env: Environment = defaultEnv;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  private get redirectUri() {
    return isPlatformBrowser(this.platformId)
      ? this.env.redirectUri
      : '';
  }

  login(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const { cognitoDomain, clientId } = this.env;
    const params = new URLSearchParams({
      client_id: clientId,
      response_type: 'code',
      scope: 'openid profile email',
      redirect_uri: this.redirectUri,
    });
    window.location.href = `${cognitoDomain}/login?${params.toString()}`;
  }

  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    if (!isPlatformBrowser(this.platformId)) {
      throw new Error('SSR: handleCallback interdit');
    }
    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) throw new Error('OAuth2 code manquant');

    const tokenEndpoint = `${this.env.cognitoDomain}/oauth2/token`;
    const body = new HttpParams()
      .set('grant_type', 'authorization_code')
      .set('client_id', this.env.clientId)
      .set('redirect_uri', this.redirectUri)
      .set('code', code);

    const res = await this.http
      .post<{ id_token: string }>(tokenEndpoint, body.toString(), {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      })
      .toPromise();

    const idToken = res!.id_token;
    localStorage.setItem('id_token', idToken);

    const payload = JSON.parse(atob(idToken.split('.')[1]));
    return {
      username: payload['cognito:username'],
      email: payload.email,
    };
  }

  logout(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const params = new URLSearchParams({
      client_id: this.env.clientId,
      logout_uri: window.location.origin,
    });
    window.location.href = `${this.env.cognitoDomain}/logout?${params.toString()}`;
  }

  async getCurrentUser(): Promise<UserInfo | null> {
    if (!isPlatformBrowser(this.platformId)) return null;
    const idToken = localStorage.getItem('id_token');
    if (!idToken) return null;
    try {
      const payload = JSON.parse(atob(idToken.split('.')[1]));
      return { username: payload['cognito:username'], email: payload.email };
    } catch {
      return null;
    }
  }
}

export const COGNITO_AUTH_PROVIDER = [
  { provide: AUTH_SERVICE, useClass: CognitoAuthService },
];
