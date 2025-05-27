import { Component, inject, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { HeaderComponent } from './header/header.component';
import { isPlatformBrowser } from '@angular/common';
import { AuthStore } from './stores/auth.store';

@Component({
  imports: [RouterModule, HeaderComponent, TranslateModule],
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'parking';
  readonly authStore = inject(AuthStore);

  constructor(
    translate: TranslateService,
    @Inject(PLATFORM_ID) private platformId: object,
  ) {
    translate.addLangs(['en', 'fr']);
    translate.setDefaultLang('en');
    translate.use('en');
  }

  ngOnInit() {
    if (isPlatformBrowser(this.platformId) && !this.authStore.isAuthenticated()) {
      const token = localStorage.getItem('authToken');
      if (token) {
        this.authStore.setToken(token);
      }
    }
  }
}
