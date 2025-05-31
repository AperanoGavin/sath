import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { AUTH_SERVICE, IAuthService, UserInfo } from '../domain/IAuthService';
import { cognitoEnv } from './cognito.env';
import { generateCodeVerifier, generateCodeChallenge } from './pkce';


@Injectable()
export class CognitoAuthService implements IAuthService {
  private readonly domain   = cognitoEnv.domain;
  private readonly clientId = cognitoEnv.clientId;
  private readonly codeVerifierKey = 'pkce_code_verifier';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  private get redirectUri(): string {
    return isPlatformBrowser(this.platformId)
      ? cognitoEnv.redirectUri
      : '';
  }

  private get logoutUri(): string {
    return isPlatformBrowser(this.platformId)
      ? cognitoEnv.logoutUri
      : '';
  }

  async login(): Promise<void> {
    if (!isPlatformBrowser(this.platformId)) return;

    const codeVerifier = generateCodeVerifier();
    const codeChallenge = await generateCodeChallenge(codeVerifier);
    localStorage.setItem(this.codeVerifierKey, codeVerifier);

    const params = new URLSearchParams({
      client_id:             this.clientId,
      response_type:         'code',
      scope:                 'openid email', // ou 'openid email phone' si nécessaire
      redirect_uri:          this.redirectUri,
      code_challenge:        codeChallenge,
      code_challenge_method: 'S256',
    });

    // 5. Redirection vers Cognito Hosted UI
    window.location.href = `${this.domain}/oauth2/authorize?${params.toString()}`;
  }


  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    if (!isPlatformBrowser(this.platformId)) {
      throw new Error('handleCallback appelé en SSR');
    }

    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) {
      throw new Error('OAuth2 code manquant dans callback URL');
    }

    const codeVerifier = localStorage.getItem(this.codeVerifierKey);
    if (!codeVerifier) {
      throw new Error('PKCE code_verifier manquant');
    }

    const tokenEndpoint = `${this.domain}/oauth2/token`;
    const bodyParams = new HttpParams({
      fromObject: {
        grant_type:    'authorization_code',
        client_id:     this.clientId,
        redirect_uri:  this.redirectUri,
        code:          code,
        code_verifier: codeVerifier,
      },
    });

    const res = await this.http
      .post<{ id_token: string }>(
        tokenEndpoint,
        bodyParams.toString(),
        { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }
      )
      .toPromise();

    localStorage.removeItem(this.codeVerifierKey);

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

    // 1. Construire l’URL de sign-out
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
  { provide: AUTH_SERVICE, useClass: CognitoAuthService }
];
