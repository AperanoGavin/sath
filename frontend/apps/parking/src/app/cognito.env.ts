export interface CognitoEnv {
  domain: string;
  clientId: string;
  redirectUri: string;
  logoutUri: string;
}

export const cognitoEnv: CognitoEnv = {
  // le sous-domaine exact de ton Hosted UI
  domain: 'https://czfshuw1u.auth.eu-west-1.amazoncognito.com',
  clientId: '622o4sqg2h9e4t4gu47fap5vi0',

  // doit être dans "Allowed callback URLs"
  redirectUri: 'https://d84l1y8p4kdic.cloudfront.net',

  // doit être dans "Allowed sign-out URLs"
  logoutUri: 'https://d84l1y8p4kdic.cloudfront.net/logout-callback',
};
