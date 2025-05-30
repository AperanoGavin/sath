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
    if (this.car() !== null) {
      this.alertService.error('Emplacement déja utilisé', 'error');
      return;
    }

    const html = event.dataTransfer?.getData('text/html') || '';
    const parsedDocument = new DOMParser().parseFromString(html, "text/html");
    const imgElement = parsedDocument.querySelector('img');
    const image = imgElement ? imgElement.src : '';

    if (confirm('Voulez vous déposer la voiture ici ?')) {
      this.car.set(image);
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
}
