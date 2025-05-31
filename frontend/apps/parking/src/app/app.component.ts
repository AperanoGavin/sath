// apps/parking/src/app/app.component.ts
import {
  Component,
  OnInit,
  Inject,
  PLATFORM_ID,
  inject
} from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { HeaderComponent } from './header/header.component';
import { ParkingComponent } from './parking/parking.component';
import { AuthStore } from '@auth/stores';
import { AUTH_SERVICE, IAuthService, UserInfo } from '@auth/domain';
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
    // Seul en navigateur (SSR ignore)
    if (!isPlatformBrowser(this.platformId)) return;

    // 1) Si l’URL contient un ?code=… (après retour de Cognito)
    const params = new URLSearchParams(window.location.search);
    const code = params.get('code');
    if (code) {
      try {
        // Échange le code PKCE contre id_token + recupère userInfo
        const user: UserInfo = await this.authService.handleCallback(window.location.href);

        // Stocke l’id_token en localStorage + dans AuthStore
        const token = localStorage.getItem('id_token');
        if (token) {
          this.authStore.setToken(token);
        }

        // Stocke l’objet user (username + email) dans AuthStore
        this.authStore.setUserInfo(user);
      } catch (err) {
        console.error('Échec du callback Cognito', err);
      } finally {
        // On purge le paramètre ?code=… de l’URL pour ne pas le garder
        this.router.navigate([], { replaceUrl: true });
      }
      return;
    }

    // 2) Sinon, si pas encore authentifié *et* qu’un id_token est en localStorage
    if (!this.authStore.isAuthenticated()) {
      const existingToken = localStorage.getItem('id_token');
      if (existingToken) {
        // On remet le token dans le store
        this.authStore.setToken(existingToken);
        // On reconstruit le userInfo à partir du token
        const user = await this.authService.getCurrentUser();
        if (user) {
          this.authStore.setUserInfo(user);
        }
      }
    }
  }
}
