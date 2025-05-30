import { signalStore, withState, withMethods, withComputed, patchState } from '@ngrx/signals';
import { debounceTime, distinct, exhaustMap, pipe, repeat, switchMap, tap } from 'rxjs';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { computed, inject, linkedSignal } from '@angular/core';
import { ParkingService } from '../services/parking.service';
import { Router } from '@angular/router';

export interface ParkingState {
}

export const ParkingStore = signalStore(
    withState<ParkingState>({
    }),
    // withComputed((store) => {
    // }),
    // withMethods((store, service = inject(ParkingService), router = inject(Router)) => ({
        
    // }))
);