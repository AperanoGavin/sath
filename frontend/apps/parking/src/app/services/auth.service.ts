import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID, TransferState } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
    private env: Environment = defaultEnv
    
    constructor(
        private http: HttpClient,
        private transferState: TransferState,
        @Inject(PLATFORM_ID) private platformId: object
    ) {
        if (isPlatformBrowser(this.platformId)) {
            this.env = this.transferState.get<Environment>(envStateKey, defaultEnv);
        }
    }

    login(dto: any) {
        return of("token")
    }
}