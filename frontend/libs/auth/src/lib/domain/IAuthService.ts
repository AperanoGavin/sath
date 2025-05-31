import { InjectionToken } from '@angular/core';

export interface UserInfo {
  username: string;
  email: string;
}

export interface IAuthService {
  login(): Promise<void>;
  handleCallback(callbackUrl: string): Promise<UserInfo>;
  logout(): void;
  getCurrentUser(): Promise<UserInfo | null>;
}

export const AUTH_SERVICE = new InjectionToken<IAuthService>('AUTH_SERVICE');
