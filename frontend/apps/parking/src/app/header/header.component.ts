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
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-parking-header',
  standalone: true,
  providers: [AuthService],
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
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {
  private env: Environment = defaultEnv;

  constructor(
    private transferState: TransferState,
    private readonly router: Router,
    private translateService: TranslateService,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  readonly authStore = inject(AuthStore);

  translationKeys = TranslationKeys

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
        this.env = this.transferState.get<Environment>(envStateKey, defaultEnv);
    }

    console.log('Environment:', this.env);
  }

  get languages(): string[] {
    return this.translateService.getLangs();
  }

  get currentLanguage(): string {
    return this.translateService.currentLang;
  }

  get username() {
    return 'anonymous'
  }

  changeLang(lang: string): void {
    this.translateService.use(lang)
  }

  getFlag(lang: string): string {
    switch (lang) {
      case 'en':
        return 'us'
      default:
        return lang
    }
  }

  logout() {
    this.authStore.logout()
  }
}
