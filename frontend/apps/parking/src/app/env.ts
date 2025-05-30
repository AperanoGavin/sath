import { makeStateKey } from "@angular/core";

export interface Environment {
    AUTH_URL: string;
    PROJECT_URL: string;
    SOCIAL_URL: string;
}

export const defaultEnv: Environment = {
    AUTH_URL: "default",
    PROJECT_URL: "default",
    SOCIAL_URL: "default",
}
export const COGNITO = {
  REGION:        'eu-west-1',
  USER_POOL_ID:  'eu-west-1_CzFShuW1u',
  CLIENT_ID:     '622o4sqg2h9e4t4gu47fap5vi0',
  REDIRECT_URI:  'https://d84l1y8p4kdic.cloudfront.net',  // ou localhost:4200 pour dev
  LOGOUT_URI:    'https://d84l1y8p4kdic.cloudfront.net/logout-callback'
};


export const envStateKey = makeStateKey<Environment>('env');
