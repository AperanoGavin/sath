import { mergeApplicationConfig, ApplicationConfig, TransferState, makeStateKey, provideAppInitializer, inject, APP_INITIALIZER } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { provideServerRouting } from '@angular/ssr';
import * as dotenv from 'dotenv';
import { appConfig } from './app.config';
import { serverRoutes } from './app.routes.server';
import { Environment, envStateKey } from './env';

import { Injectable } from '@angular/core';

dotenv.config();

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  loadConfig(): Promise<void> {
    return Promise.resolve();
  }
}

const intializeAppFn = () => {
  const configService = inject(ConfigService);
  return configService.loadConfig();
};


export function transferStateFactory(transferState: TransferState) {
  return () => {
    const vars: Environment = {
      PARKING_URL: process.env['PARKING_URL'] || '',
    };
    transferState.set<Environment>(envStateKey, vars);
  };
}

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(),
    provideServerRouting(serverRoutes),
    provideAppInitializer(() => intializeAppFn()),
    {
      provide: APP_INITIALIZER,
      useFactory: transferStateFactory,
      deps: [TransferState],
      multi: true,
    },
  ],
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
