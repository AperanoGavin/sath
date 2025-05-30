import { Component, Inject, OnInit, PLATFORM_ID, TransferState, inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';

import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationKeys } from '../keys.interface';
import { AuthStore } from '../stores/auth.store';
import { ParkingService } from '../services/parking.service';
import { ParkingSpotComponent } from './spot/parking-spot.component';
import { ImagesViewerComponent } from '@parking/shared-angular';

@Component({
  selector: 'app-parking-parking',
  standalone: true,
  providers: [ParkingService],
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    MatButtonModule,
    RouterModule,
    TranslateModule,
    ParkingSpotComponent,
    ImagesViewerComponent
  ],
  templateUrl: './parking.component.html',
})
export class ParkingComponent implements OnInit {
  private env: Environment = defaultEnv;

  constructor(
    private transferState: TransferState,
    private readonly router: Router,
    private translateService: TranslateService,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  readonly authStore = inject(AuthStore);

  translationKeys = TranslationKeys

  isDragOver = false;

  firstLineSpots: number[] = [];
  secondLineSpots: number[] = [];
  cars = ['/assets/voiture1.png', '/assets/voiture2.png', '/assets/voiture3.png', '/assets/voiture4.png', '/assets/voiture5.png'];

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
        this.env = this.transferState.get<Environment>(envStateKey, defaultEnv);
    }

    const parkingSpots = Array.from({ length: 16 }, (_, i) => i + 1);
    this.firstLineSpots = parkingSpots.slice(0, 8);
    this.secondLineSpots = parkingSpots.slice(8, 16);
  }

  parkingSpotClicked() {
    alert('Parking spot clicked!');
  }
}
