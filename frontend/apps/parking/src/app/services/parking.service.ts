import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID, TransferState } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { CreateSpotDTO, SpotCapability, SpotDTO } from '../dtos';

@Injectable({
  providedIn: 'root',
})
export class ParkingService {
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
        return this.http.get<SpotDTO[]>(`${this.env.PARKING_URL}/api/v1/spots`)
    }

    getSpotCapabilities() {
        return this.http.get<SpotCapability[]>(`${this.env.PARKING_URL}/api/v1/spots/capabilities`);
    }

    getById(id: string) {
        return this.http.get<SpotCapability[]>(`${this.env.PARKING_URL}/api/v1/spots/${id}`);
    }

    create(dto: CreateSpotDTO) {
        return this.http.post<void>(`${this.env.PARKING_URL}/api/v1/spots`, dto, {
            responseType: undefined
        })
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.env.PARKING_URL}/api/v1/spots/${id}`, {
            responseType: undefined
        });
    }

    getSpotCalendar(id: string) {
        return this.http.get<void>(`${this.env.PARKING_URL}/api/v1/spots/${id}/calendar`);
    }
}