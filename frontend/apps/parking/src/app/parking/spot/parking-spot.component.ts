import { Component, Inject, OnInit, PLATFORM_ID, TransferState, inject, input, linkedSignal, signal } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';

import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationKeys } from '../../keys.interface';
import { AuthStore } from '../../stores/auth.store';
import { ParkingService } from '../../services/parking.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-parking-parking-spot',
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
  ],
  templateUrl: './parking-spot.component.html',
  styleUrls: ['./parking-spot.component.scss'],
})
export class ParkingSpotComponent implements OnInit {
  constructor(
    private transferState: TransferState,
    private readonly router: Router,
    private translateService: TranslateService,
    private alertService: AlertService,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  readonly authStore = inject(AuthStore);

  translationKeys = TranslationKeys

  isDragOver = false;

  spotNumber = input.required<number>()
  car = signal<string | null>(null);

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
    console.log(event)
    console.log(event.dataTransfer)
    if (this.car() !== null) {
      this.alertService.error('This parking spot is already occupied!', 'error');
      return;
    }

    if (confirm('Voulez vous déposer la voiture ici ?')) {
      this.car.set("/assets/voiture1.png");
    }
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      // 
    }
  }

  spotClicked() {
    this.alertService.info('Parking spot clicked!', 'info');
  }
}
