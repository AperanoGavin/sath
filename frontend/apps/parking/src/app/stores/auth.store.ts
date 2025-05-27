import { signalStore, withState, withMethods, withComputed, patchState } from '@ngrx/signals';
import { debounceTime, distinct, exhaustMap, pipe, repeat, switchMap, tap } from 'rxjs';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { computed, inject, linkedSignal } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export interface AuthState {
    token: string | null;

}

export const AuthStore = signalStore(
    withState<AuthState>({
        token: null
    }),
    withComputed((store) => {
        return {
            isAuthenticated: computed(() => {
                return store.token() !== null;
            }),
        }
    }),
    withMethods((store, service = inject(AuthService), router = inject(Router)) => ({
        setToken: (token: string) => {
            console.log('Setting token:', token);
            patchState(store, { token });
        },
        logout: () => {
            localStorage.removeItem('authToken');
            patchState(store, { token: null });
            console.log('Logged out');
            router.navigate(['/']);
        },
        login: rxMethod<any> ( //TODO: replace w cognito
            pipe(
                exhaustMap((dto) => service.login(dto)),
                tapResponse({
                    next: (token) => {
                        patchState(store, {
                            token: token,
                        });
                        localStorage.setItem('authToken', token);
                        router.navigate(['/']);
                    },
                    error: function (error: unknown): void {
                        // alert.error(`Failed to login`)
                    }

                }),
                repeat()
            )
        )
    }))
);