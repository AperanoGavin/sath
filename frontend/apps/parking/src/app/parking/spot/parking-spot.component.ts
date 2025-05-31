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
import { ParkingStore } from '../../stores/parking.store';
import { SpotCapability } from '../../dtos';

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

  readonly parkingStore = inject(ParkingStore);

  translationKeys = TranslationKeys

  isDragOver = false;

  id = input.required<string>();
  spotNumber = input.required<string>();
  capabilities = input.required<SpotCapability[]>();
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
    if (this.car() !== null) {
      this.alertService.error('Emplacement déja utilisé', 'error');
      return;
    }

    const html = event.dataTransfer?.getData('text/html') || '';
    const parsedDocument = new DOMParser().parseFromString(html, "text/html");
    const imgElement = parsedDocument.querySelector('img');
    const image = imgElement ? imgElement.src : '';

    const from = this.parkingStore.startDate();
    const to = this.parkingStore.endDate();

    if (from === null || to === null) {
      this.alertService.error('Veuillez sélectionner une date de début et de fin avant de déposer la voiture.', 'error');
      return;
    }

    if (confirm('Voulez vous déposer la voiture ici ?')) {
      if (this.isSlotAvailable()) {
        this.car.set(image);
        this.parkingStore.createSpotReservation({
          spotId: this.id(),
          userId: 'e903b35d-9dac-4eba-9e28-76912b62fb1c',
          from,
          to,
          isReservation: true,
        });
      }
    }
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      // 
    }
  }

  spotClicked() {
    // 
  }

  isSlotAvailable(): boolean {
    const reservations = this.parkingStore.reservations().get(this.id());
    const startDate = this.parkingStore.startDate();
    const endDate = this.parkingStore.endDate();

    if (!reservations || !startDate || !endDate) {
      return true;
    }

    return !reservations.some(reservation => {
      const reservationStart = new Date(reservation.from);
      const reservationEnd = new Date(reservation.to);
      return (startDate < reservationEnd && endDate > reservationStart);
    });
  }
}
