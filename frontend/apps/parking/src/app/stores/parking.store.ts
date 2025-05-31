import { signalStore, withState, withMethods, withComputed, patchState } from '@ngrx/signals';
import { concatMap, debounceTime, distinctUntilChanged, exhaustMap, from, mergeMap, of, pipe, switchMap, tap } from 'rxjs';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { computed, inject } from '@angular/core';
import { ParkingService } from '../services/parking.service';
import { Router } from '@angular/router';
import { CreateSpotDTO, SpotDTO, SpotCapability, ErrorResponse, ReservationDTO, CreateReservationDTO } from '../dtos';
import { AlertService } from '../services/alert.service';

export interface ParkingState {
  spots: SpotDTO[];
  spotCapabilities: SpotCapability[];
  loading: boolean;
  error: string | null;
  reservations: Map<string, ReservationDTO[]>;
  startDate: Date | null;
  endDate: Date | null;
}

const initialState: ParkingState = {
  spots: [],
  spotCapabilities: [],
  loading: false,
  error: null,
  reservations: new Map<string, ReservationDTO[]>(),
  startDate: null,
  endDate: null,
};

export const ParkingStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((_store) => ({
  })),
  withMethods((store, parkingService = inject(ParkingService), _router = inject(Router), alertService = inject(AlertService)) => ({
    setStartDate: rxMethod<Date | null>(
      pipe(
        tap((date) => patchState(store, { startDate: date })),
        distinctUntilChanged(),
      )
    ),
    setEndDate: rxMethod<Date | null>(
      pipe(
        tap((date) => patchState(store, { endDate: date })),
        distinctUntilChanged(),
      )
    ),
    loadSpots: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          parkingService.getAll().pipe(
            tapResponse({
              next: (spots) => {
                console.log('Spots loaded:', spots);
                patchState(store, { spots, loading: false });
              },
              error: (error: ErrorResponse) => {
                patchState(store, { error: error.detail, loading: false });
                alertService.error(error.detail, 'error');
              },
            }),
          ),
        ),
        switchMap((spots) => {
          return from(spots).pipe(
            mergeMap((spot) =>
              parkingService.getSpotCalendar(spot.id).pipe(
                tapResponse({
                  next: (calendar) => {
                    const reservations = new Map(store.reservations());
                    reservations.set(spot.id, calendar);
                    patchState(store, { reservations });
                  },
                  error: (error: ErrorResponse) => {
                    console.error(`Error loading calendar for spot ${spot.id}:`);
                  },
                })
              )
            )
          )
        }),
      )
    ),
  })),
  withMethods((store, parkingService = inject(ParkingService), _router = inject(Router), alertService = inject(AlertService)) => ({
    loadCapabilities: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          parkingService.getCapabilities().pipe(
            tapResponse({
              next: (spotCapabilities) => {
                console.log('Spot capabilities loaded:', spotCapabilities);
                patchState(store, { spotCapabilities, loading: false })
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

    createSpotReservation: rxMethod<CreateReservationDTO>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((dto) =>
          parkingService.createSpotReservation(dto).pipe(
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
  }))
);
