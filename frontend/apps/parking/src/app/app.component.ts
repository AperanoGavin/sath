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
    if (!isPlatformBrowser(this.platformId)) return;

    const params = new URLSearchParams(window.location.search);
    const code = params.get('code');
    if (code) {
      try {
        const user: UserInfo = await this.authService.handleCallback(window.location.href);

        const token = localStorage.getItem('id_token');
        if (token) {
          this.authStore.setToken(token);
        }

        this.authStore.setUserInfo(user);
      } catch (err) {
        console.error('Échec du callback Cognito', err);
      } finally {
        this.router.navigate([], { replaceUrl: true });
      }
      return;
    }

    if (!this.authStore.isAuthenticated()) {
      const existingToken = localStorage.getItem('id_token');
      if (existingToken) {
        this.authStore.setToken(existingToken);
        const user = await this.authService.getCurrentUser();
        if (user) {
          this.authStore.setUserInfo(user);
        }
      }
    }
  }
}
