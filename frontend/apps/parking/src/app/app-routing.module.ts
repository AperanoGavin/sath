// apps/parking/src/app/app-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ParkingComponent } from './parking/parking.component';

const routes: Routes = [
  { path: '', component: ParkingComponent },
  { path: 'logout-callback', component: ParkingComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
