import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID, TransferState } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { CreateReservationDTO, CreateSpotDTO, ReservationDTO, SpotCapability, SpotDTO } from '../dtos';
import { map } from 'rxjs';

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
        return this.http.get<{data: SpotDTO[]}>(`${this.env.PARKING_URL}/api/v1/spots`)
            .pipe(map(response => response.data));
    }

    getCapabilities() {
        return this.http.get<{data: SpotCapability[]}>(`${this.env.PARKING_URL}/api/v1/spots/capabilities`)
        .pipe(map(response => response.data));
    }

    getById(id: string) {
        return this.http.get<{data: SpotDTO}>(`${this.env.PARKING_URL}/api/v1/spots/${id}`)
            .pipe(map(response => response.data));
    }

    createSpotReservation(dto: CreateReservationDTO) {
        return this.http.post<{data: ReservationDTO}>(`${this.env.PARKING_URL}/api/v1/reservations`, dto)
    }

    // create(dto: CreateSpotDTO) {
    //     return this.http.post<void>(`${this.env.PARKING_URL}/api/v1/spots`, dto, {
    //         responseType: undefined
    //     })
    // }

    delete(id: string) {
        return this.http.delete<void>(`${this.env.PARKING_URL}/api/v1/spots/${id}`, {
            responseType: undefined
        });
    }

    getSpotCalendar(id: string) {
        return this.http.get<{data: ReservationDTO[]}>(`${this.env.PARKING_URL}/api/v1/spots/${id}/calendar`)
        .pipe(map(response => response.data));
    }
}