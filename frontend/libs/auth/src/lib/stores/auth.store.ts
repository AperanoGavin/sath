import { Injectable, signal, computed } from '@angular/core';
import { UserInfo } from '../domain/IAuthService';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  /** Le JWT id_token (ou null s’il n’est pas en cache) */
  token = signal<string | null>(null);

  /** Les infos décodées de l’utilisateur (username + email), ou null */
  userInfo = signal<UserInfo | null>(null);

  /** True si un token est présent */
  isAuthenticated = computed(() => !!this.token());

  /** Stocke le id_token JWT */
  setToken(token: string) {
    this.token.set(token);
  }

  /** Stocke l’objet UserInfo { username, email } */
  setUserInfo(user: UserInfo) {
    this.userInfo.set(user);
  }

  /** Déconnecte en local : vide token, vide userInfo et supprime le localStorage */
  logout() {
    this.token.set(null);
    this.userInfo.set(null);
    localStorage.removeItem('id_token');
  }
}
