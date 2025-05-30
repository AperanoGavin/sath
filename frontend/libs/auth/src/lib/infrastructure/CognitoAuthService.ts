import { IAuthService, UserInfo } from '@auth/domain';
import { COGNITO } from '../../../../../apps/parking/src/app/env';

export class CognitoAuthService implements IAuthService {
  private issuer    = `https://cognito-idp.${COGNITO.REGION}.amazonaws.com/${COGNITO.USER_POOL_ID}`;
  private authUrl   = `${this.issuer}/oauth2/authorize`;
  private tokenUrl  = `${this.issuer}/oauth2/token`;
  private logoutUrl = `${this.issuer}/logout`;
  private clientId  = COGNITO.CLIENT_ID;

  login(): void {
    const params = new URLSearchParams({
      client_id:     this.clientId,
      response_type: 'code',
      scope:         'openid email phone',
      redirect_uri:  COGNITO.REDIRECT_URI,
    });
    window.location.href = `${this.authUrl}?${params}`;
  }

  async handleCallback(callbackUrl: string): Promise<UserInfo> {
    const url = new URL(callbackUrl);
    const code = url.searchParams.get('code');
    if (!code) throw new Error('Code manquant en callback');

    const body = new URLSearchParams({
      grant_type:   'authorization_code',
      client_id:    this.clientId,
      code,
      redirect_uri: COGNITO.REDIRECT_URI,
    });
    const tokenRes = await fetch(this.tokenUrl, {
      method:  'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body:    body.toString(),
    });
    const tokenSet = await tokenRes.json();

    const userRes = await fetch(`${this.issuer}/oauth2/userInfo`, {
      headers: { Authorization: `Bearer ${tokenSet.access_token}` },
    });
    const profile = await userRes.json();

    localStorage.setItem('auth_tokens', JSON.stringify(tokenSet));
    localStorage.setItem('user_info',  JSON.stringify(profile));

    return {
      username: profile.username || profile.sub,
      email:    profile.email,
      phone:    profile.phone_number,
    };
  }

  logout(): void {
    localStorage.removeItem('auth_tokens');
    localStorage.removeItem('user_info');
    const params = new URLSearchParams({
      client_id:  this.clientId,
      logout_uri: COGNITO.LOGOUT_URI,
    });
    window.location.href = `${this.logoutUrl}?${params}`;
  }

  async getCurrentUser(): Promise<UserInfo | null> {
    const stored = localStorage.getItem('user_info');
    return stored ? JSON.parse(stored) as UserInfo : null;
  }
}
