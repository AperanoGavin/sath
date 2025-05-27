import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID, TransferState } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { ExampleDTO } from '../dtos';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
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

    getAll() {
        return this.http.get<ExampleDTO[]>(`${this.env.PROJECT_URL}/parkings`)
    }

    getById(id: string) {
        return this.http.get<ExampleDTO>(`${this.env.PROJECT_URL}/parkings/${id}`)
    }

    create(dto: ExampleDTO) {
        return this.http.post<string>(`${this.env.PROJECT_URL}/parkings`, dto, {
            responseType: undefined
        })
    }
}