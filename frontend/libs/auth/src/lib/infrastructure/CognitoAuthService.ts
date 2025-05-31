// libs/auth/src/lib/infrastructure/CognitoAuthService.ts
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { AUTH_SERVICE, IAuthService, UserInfo } from '../domain/IAuthService';
import { cognitoEnv } from './cognito.env';
import { generateCodeVerifier, generateCodeChallenge } from './pkce';

@Injectable()
export class CognitoAuthService implements IAuthService {
  // NE PAS ajouter de slash à la fin du domain
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

    // 1) Génère code_verifier + code_challenge
    const codeVerifier = generateCodeVerifier();
    const codeChallenge = await generateCodeChallenge(codeVerifier);
    localStorage.setItem(this.codeVerifierKey, codeVerifier);

    // 2) Prépare les paramètres PKCE + OAuth2
    const params = new URLSearchParams({
      client_id:             this.clientId,
      response_type:         'code',
      scope:                 'openid email',
      redirect_uri:          this.redirectUri,
      code_challenge:        codeChallenge,
      code_challenge_method: 'S256',
    });

    // 3) ← Utiliser impérativement /oauth2/authorize (et non /login)
    window.location.href = `${this.domain}/oauth2/authorize?${params.toString()}`;
  }

  /**
   * Gère le callback PKCE de Cognito, échange le code en id_token, et renvoie UserInfo.
   */
  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    if (!isPlatformBrowser(this.platformId)) {
      throw new Error('handleCallback doit être appelé en navigateur');
    }

    // 1) Extrait "code" de l’URL
    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) {
      throw new Error('OAuth2 code manquant dans l’URL');
    }

    // 2) Récupère le code_verifier en cache
    const codeVerifier = localStorage.getItem(this.codeVerifierKey);
    if (!codeVerifier) {
      throw new Error('PKCE code_verifier introuvable');
    }

    // 3) Prépare le corps form-urlencoded pour /oauth2/token
    const bodyParams = new HttpParams()
      .set('grant_type',    'authorization_code')
      .set('client_id',     this.clientId)
      .set('redirect_uri',  this.redirectUri)
      .set('code',          code)
      .set('code_verifier', codeVerifier);

    // 4) Envoie POST vers /oauth2/token
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
      console.error('Erreur POST /oauth2/token', err);
      throw err;
    }

    // 5) Supprime code_verifier
    localStorage.removeItem(this.codeVerifierKey);

    // 6) Stocke l’id_token dans localStorage
    const idToken = res.id_token;
    localStorage.setItem('id_token', idToken);

    // 7) Décode le JWT pour obtenir { username, email }
    try {
      const payload = JSON.parse(atob(idToken.split('.')[1]));
      return {
        username: payload['cognito:username'],
        email:    payload.email,
      };
    } catch {
      throw new Error('Erreur lors du décodage de l’id_token');
    }
  }

  /** Déconnecte localement puis redirige vers Cognito pour le logout */
  logout(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    // 1) Bascule vers l’URL de logout Cognito (pas de secret requis pour SPA)
    const params = new URLSearchParams({
      client_id:  this.clientId,
      logout_uri: this.logoutUri,
    });
    window.location.href = `${this.domain}/logout?${params.toString()}`;
  }

  /** Récupère l’utilisateur courant en décodant l’id_token déjà en localStorage */
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
