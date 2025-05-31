import { Component, Inject, OnInit, PLATFORM_ID, TransferState, inject, linkedSignal } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';

import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationKeys } from '../keys.interface';
import { AuthStore } from '../stores/auth.store';
import { ParkingService } from '../services/parking.service';
import { ParkingSpotComponent } from './spot/parking-spot.component';
import { ImagesViewerComponent } from '@parking/shared-angular';
import { ParkingStore } from '../stores/parking.store';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

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
    ImagesViewerComponent,
    MatFormFieldModule,
    MatDatepickerModule,
    ReactiveFormsModule
  ],
  templateUrl: './parking.component.html',
})
export class ParkingComponent implements OnInit {
  private env: Environment = defaultEnv;

  constructor(
    private transferState: TransferState,
    private readonly router: Router,
    private translateService: TranslateService,
    @Inject(PLATFORM_ID) private platformId: object,
    private fb: FormBuilder
  ) {
    this.range = this.fb.group({
      start: [null],
      end: [null]
    });
  }

  readonly authStore = inject(AuthStore);
  readonly parkingStore = inject(ParkingStore);

  translationKeys = TranslationKeys

  isDragOver = false;

  cars = ['/assets/voiture1.png', '/assets/voiture2.png', '/assets/voiture3.png', '/assets/voiture4.png', '/assets/voiture5.png'];
  range: FormGroup;

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
        this.env = this.transferState.get<Environment>(envStateKey, defaultEnv);
        this.parkingStore.loadSpots();
        this.parkingStore.loadCapabilities();

        this.range.valueChanges.subscribe(() => {
          this.onDateChange();
        });
    }
  }

  parkingSpotClicked() {
    // 
  }

  onDateChange() {
    const startDate = this.range.get('start')?.value;
    const endDate = this.range.get('end')?.value;
    console.log('Selected date range:', startDate, endDate);

    this.parkingStore.setStartDate(startDate);
    this.parkingStore.setEndDate(endDate);
  }
}
