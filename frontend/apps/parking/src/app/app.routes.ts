import { Route } from '@angular/router';

export const appRoutes: Route[] = [
    {
        path: '',
        loadComponent: () => import('./parking/parking.component').then(m => m.ParkingComponent),
    },
];
