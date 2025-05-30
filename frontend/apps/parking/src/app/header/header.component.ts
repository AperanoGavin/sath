// header.component.ts
import {
  Component,
  Inject,
  OnInit,
  PLATFORM_ID,
  TransferState,
  inject
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { defaultEnv, Environment, envStateKey } from '../env';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';

import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationKeys } from '../keys.interface';
import { AuthStore } from '../stores/auth.store';

// On importe l’abstraction IAuthService et son token
import { AUTH_SERVICE, IAuthService } from '@auth/domain';
// On importe la vraie classe pour le provider
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-parking-header',
  standalone: true,
  providers: [
    AuthService,
    { provide: AUTH_SERVICE, useExisting: AuthService }
  ],
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

  // on récupère l’implémentation via le token
  private readonly authService = inject(AUTH_SERVICE) as IAuthService;
  readonly authStore = inject(AuthStore);
  translationKeys = TranslationKeys;

  constructor(
    private transferState: TransferState,
    private translateService: TranslateService,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

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

  get username(): string {
    return 'anonymous';
  }

  changeLang(lang: string): void {
    this.translateService.use(lang);
  }

  getFlag(lang: string): string {
    return lang === 'en' ? 'us' : lang;
  }

  logout(): void {
    this.authStore.logout();
  }

  /** déclenche la Hosted UI via ton AuthService concret */
  login(): void {
    this.authService.login();
  }
}
