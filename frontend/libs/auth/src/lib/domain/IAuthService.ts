export interface UserInfo {
  username: string;
  email:    string;
  phone?:   string;
}

export interface IAuthService {
  /** Redirige vers l’Hosted UI Cognito */
  login(): void;

  /** Traite le callback OIDC: échange code → tokens + profile */
  handleCallback(callbackUrl: string): Promise<UserInfo>;

  /** Déconnecte et redirige vers l’URI de logout */
  logout(): void;

  /** Récupère l’utilisateur courant (ou null) */
  getCurrentUser(): Promise<UserInfo | null>;
}
