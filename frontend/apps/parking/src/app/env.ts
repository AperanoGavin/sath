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

export const envStateKey = makeStateKey<Environment>('env');
