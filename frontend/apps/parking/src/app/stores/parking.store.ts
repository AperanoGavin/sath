import { signalStore, withState, withMethods, withComputed, patchState } from '@ngrx/signals';
import { debounceTime, distinctUntilChanged, exhaustMap, pipe, switchMap, tap } from 'rxjs';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { computed, inject } from '@angular/core';
import { ParkingService } from '../services/parking.service';
import { Router } from '@angular/router';
import { CreateSpotDTO, SpotDTO, SpotCapability, ErrorResponse } from '../dtos';
import { AlertService } from '../services/alert.service';

export interface ParkingState {
  spots: SpotDTO[];
  spotCapabilities: SpotCapability[];
  loading: boolean;
  error: string | null;
}

const initialState: ParkingState = {
  spots: [],
  spotCapabilities: [],
  loading: false,
  error: null,
};

export const ParkingStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((_store) => ({
  })),
  withMethods((store, parkingService = inject(ParkingService), _router = inject(Router), alertService = inject(AlertService)) => ({
    loadSpots: rxMethod<void>(
        pipe(
          tap(() => patchState(store, { loading: true, error: null })),
          switchMap(() =>
            parkingService.getAll().pipe(
              tapResponse({
                next: (spots) => patchState(store, { spots, loading: false }),
                error: (error: ErrorResponse) => {
                  patchState(store, { error: error.detail, loading: false });
                  alertService.error(error.detail, 'error');
                },
              })
            )
          )
        )
      ),
  })),
  withMethods((store, parkingService = inject(ParkingService), _router = inject(Router), alertService = inject(AlertService)) => ({
    loadSpotCapabilities: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          parkingService.getSpotCapabilities().pipe(
            tapResponse({
              next: (spotCapabilities) => patchState(store, { spotCapabilities, loading: false }),
              error: (error: ErrorResponse) => {
                patchState(store, { error: error.detail, loading: false });
                alertService.error(error.detail, 'error');
              },
            })
          )
        )
      )
    ),

    createSpot: rxMethod<CreateSpotDTO>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((dto) =>
          parkingService.create(dto).pipe(
            tapResponse({
              next: () => {
                patchState(store, { loading: false });
                // Optionally reload spots or navigate away
                store.loadSpots();
              },
              error: (error: ErrorResponse) => {
                patchState(store, { error: error.detail, loading: false });
                alertService.error(error.detail, 'error');
              },
            })
          )
        )
      )
    ),

    deleteSpot: rxMethod<string>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) =>
          parkingService.delete(id).pipe(
            tapResponse({
              next: () => {
                patchState(store, { loading: false });
                store.loadSpots();
              },
              error: (error: ErrorResponse) => {
                patchState(store, { error: error.detail, loading: false });
                alertService.error(error.detail, 'error');
              },
            })
          )
        )
      )
    ),

    getSpotCalendar: rxMethod<string>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) =>
          parkingService.getSpotCalendar(id).pipe(
            tapResponse({
              next: () => patchState(store, { loading: false }),
              error: (error: ErrorResponse) => {
                patchState(store, { error: error.detail, loading: false });
                alertService.error(error.detail, 'error');
              },
            })
          )
        )
      )
    ),
  }))
);
