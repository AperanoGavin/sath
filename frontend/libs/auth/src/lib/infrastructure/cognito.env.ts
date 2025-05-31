export interface CognitoEnv {
  domain: string;
  clientId: string;
  redirectUri: string;
  logoutUri: string;
}

export const cognitoEnv: CognitoEnv = {
  domain: 'https://eu-west-1czfshuw1u.auth.eu-west-1.amazoncognito.com',

  clientId: 'rdi4g0c5qvmbfuf5kg0g5u31',

  redirectUri: 'http://localhost:4200/',

  logoutUri: 'http://localhost:4200/logout-callback',
};
