// apps/parking/src/app/app.component.ts
import { Component, OnInit, Inject, PLATFORM_ID, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { HeaderComponent } from './header/header.component';
import { ParkingComponent } from './parking/parking.component';
import { AuthStore } from './stores/auth.store';
import { AUTH_SERVICE, IAuthService } from '@auth/domain';
import { COGNITO_AUTH_PROVIDER } from '@auth/infrastructure';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterModule,
    HeaderComponent,
    TranslateModule,
    ParkingComponent,
  ],
  providers: [
    ...COGNITO_AUTH_PROVIDER
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  readonly authStore = inject(AuthStore);
  private router = inject(Router);
  private authService = inject(AUTH_SERVICE) as IAuthService;

  constructor(
    translate: TranslateService,
    @Inject(PLATFORM_ID) private platformId: object,
  ) {
    translate.addLangs(['en', 'fr']);
    translate.setDefaultLang('en');
    translate.use('en');
  }

  async ngOnInit() {
    if (!isPlatformBrowser(this.platformId)) return;

    const params = new URLSearchParams(window.location.search);
    const code = params.get('code');
    if (code) {
      try {
        await this.authService.handleCallback(window.location.href);
        // ← on vient d'obtenir et stocker l'id_token dans localStorage
        const token = localStorage.getItem('id_token');
        if (token) {
          this.authStore.setToken(token);  // <-- remplace setUser() par setToken()
        }
      } catch (err) {
        console.error('Échec du callback Cognito', err);
      } finally {
        this.router.navigate([], { replaceUrl: true });
      }
    } else if (!this.authStore.isAuthenticated()) {
      const token = localStorage.getItem('id_token');
      if (token) {
        this.authStore.setToken(token);  // <-- remplace setUser() par setToken()
      }
    }
  }
}
