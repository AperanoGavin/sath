import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AUTH_SERVICE, IAuthService } from '@auth/domain';
import { COGNITO_AUTH_PROVIDER } from '@auth/infrastructure';

@Component({
  selector: 'app-callback',
  standalone: true,
  template: `<p>Authentification en cours…</p>`,
  providers: [
    ...COGNITO_AUTH_PROVIDER
  ]
})
export class CallbackComponent implements OnInit {
  constructor(
    private router: Router,
    @Inject(AUTH_SERVICE) private authService: IAuthService
  ) {}

  async ngOnInit() {
    try {
      await this.authService.handleCallback(window.location.href);
    } catch (err) {
      console.error('Échec du callback Cognito', err);
    } finally {
      this.router.navigate(['/']);
    }
  }
}
