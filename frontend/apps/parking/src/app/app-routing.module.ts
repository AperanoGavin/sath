import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ParkingComponent } from './parking/parking.component';
import { LogoutCallbackComponent } from './auth/logout-callback.component';

const routes: Routes = [
  { path: '', component: ParkingComponent },
  { path: 'logout-callback', component: LogoutCallbackComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
