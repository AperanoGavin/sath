// apps/parking/src/app/header/header.component.ts
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
import { AuthStore } from '@auth/stores';

import { AUTH_SERVICE, IAuthService } from '@auth/domain';
import { COGNITO_AUTH_PROVIDER } from '@auth/infrastructure/CognitoAuthService';

@Component({
  selector: 'app-parking-header',
  standalone: true,
  providers: [
    ...COGNITO_AUTH_PROVIDER
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

  // Injection du service Cognito PKCE
  private readonly authService = inject(AUTH_SERVICE) as IAuthService;

  // On récupère le store
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
  }

  get languages(): string[] {
    return this.translateService.getLangs();
  }

  get currentLanguage(): string {
    return this.translateService.currentLang;
  }

  /** Affiche le username depuis le store, ou “anonymous” si non connecté */
  get username(): string {
    const userInfo = this.authStore.userInfo();
    return userInfo ? userInfo.username : 'anonymous';
  }

  changeLang(lang: string): void {
    this.translateService.use(lang);
  }

  getFlag(lang: string): string {
    return lang === 'en' ? 'us' : lang;
  }

  /** Logout complet : déclenche la redirection Cognito + vide le store local */
  logout(): void {
    // 1) Redirige vers l’URL de logout Cognito
    this.authService.logout();
    // 2) Vide le store local (token + userInfo)
    this.authStore.logout();
  }

  /** Lance le flux PKCE vers Cognito Hosted UI */
  login(): void {
    this.authService.login();
  }
}
