import { makeStateKey } from "@angular/core";

export interface Environment {
    PARKING_URL: string;
}

export const defaultEnv: Environment = {
    PARKING_URL: "default",
}

export const envStateKey = makeStateKey<Environment>('env');
