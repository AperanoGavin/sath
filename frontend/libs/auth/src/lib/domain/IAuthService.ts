import { InjectionToken } from '@angular/core';


export interface UserInfo {
  username: string;
  email:    string;
}


export interface IAuthService {
  login(): void;


  handleCallback(callbackUrl: string): Promise<UserInfo>;

  logout(): void;

  getCurrentUser(): Promise<UserInfo | null>;
}

/**
 * InjectionToken pour permettre l’injection par abstraction
 * du service d’authentification (Clean Architecture / DIP).
 */
export const AUTH_SERVICE = new InjectionToken<IAuthService>('AUTH_SERVICE');
