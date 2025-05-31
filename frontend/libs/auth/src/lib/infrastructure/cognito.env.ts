export interface CognitoEnv {
  domain: string;
  clientId: string;
  redirectUri: string;
  logoutUri: string;
}

export const cognitoEnv: CognitoEnv = {
  domain: 'https://eu-west-1czfshuw1u.auth.eu-west-1.amazoncognito.com',

  clientId: '622o4sqg2h9e4t4gu47fap5vi0',

  redirectUri: 'http://localhost:4200/',

  logoutUri: 'http://localhost:4200/logout-callback',
};
