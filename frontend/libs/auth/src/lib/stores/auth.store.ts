import { Injectable, signal, computed } from '@angular/core';
import { UserInfo } from '../domain/IAuthService';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  // Signal pour l’id_token
  token = signal<string | null>(null);

  // Signal pour l’objet UserInfo
  userInfo = signal<UserInfo | null>(null);

  // Indique si on a un token (connecté)
  isAuthenticated = computed(() => !!this.token());

  setToken(token: string) {
    this.token.set(token);
  }

  setUserInfo(user: UserInfo) {
    this.userInfo.set(user);
  }

  logout() {
    this.token.set(null);
    this.userInfo.set(null);
    localStorage.removeItem('id_token');
  }
}
