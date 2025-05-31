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

  /** Démarre le flux PKCE → redirige vers Cognito Hosted UI */
  async login(): Promise<void> {
    if (!isPlatformBrowser(this.platformId)) return;

    const codeVerifier = generateCodeVerifier();
    const codeChallenge = await generateCodeChallenge(codeVerifier);
    localStorage.setItem(this.codeVerifierKey, codeVerifier);

    const params = new URLSearchParams({
      client_id:             this.clientId,
      response_type:         'code',
      scope:                 'openid email',
      redirect_uri:          this.redirectUri,
      code_challenge:        codeChallenge,
      code_challenge_method: 'S256',
    });

    window.location.href = `${this.domain}/oauth2/authorize?${params.toString()}`;
  }

  /**
   * Gère le callback PKCE de Cognito, échange le code en id_token, et renvoie UserInfo.
   */
  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    if (!isPlatformBrowser(this.platformId)) {
      throw new Error('handleCallback doit être exécuté en navigateur');
    }

    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) {
      throw new Error('OAuth2 code manquant dans l’URL');
    }

    const codeVerifier = localStorage.getItem(this.codeVerifierKey);
    if (!codeVerifier) {
      throw new Error('PKCE code_verifier introuvable');
    }

    const bodyParams = new HttpParams()
      .set('grant_type',    'authorization_code')
      .set('client_id',     this.clientId)
      .set('redirect_uri',  this.redirectUri)
      .set('code',          code)
      .set('code_verifier', codeVerifier);

    let res: { id_token: string };
    try {
      res = await this.http.post<{ id_token: string }>(
        `${this.domain}/oauth2/token`,
        bodyParams.toString(),
        {
          headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
          withCredentials: false
        }
      ).toPromise() as { id_token: string };
    } catch (err) {
      console.error('Erreur lors du POST /oauth2/token', err);
      throw err;
    }

    localStorage.removeItem(this.codeVerifierKey);
    const idToken = res.id_token;
    localStorage.setItem('id_token', idToken);

    try {
      const payload = JSON.parse(atob(idToken.split('.')[1]));
      return {
        username: payload['cognito:username'],
        email:    payload.email,
      };
    } catch {
      throw new Error('Impossible de décoder l’id_token');
    }
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
