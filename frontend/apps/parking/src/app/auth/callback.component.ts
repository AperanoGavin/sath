// apps/parking/src/app/auth/callback.component.ts
import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AUTH_SERVICE, IAuthService } from '@auth/domain';
import { COGNITO_AUTH_PROVIDER } from '@auth/infrastructure';

@Component({
  selector: 'app-callback',
  standalone: true,
  template: `<p>Authentification en cours…</p>`,
  providers: [
    ...COGNITO_AUTH_PROVIDER // fournit CognitoAuthService sous AUTH_SERVICE
  ]
})
export class CallbackComponent implements OnInit {
  constructor(
    private router: Router,
    @Inject(AUTH_SERVICE) private authService: IAuthService
  ) {}

  async ngOnInit() {
    try {
      // Extrait le code, échange-le contre un token et le stocke en localStorage
      await this.authService.handleCallback(window.location.href);
    } catch (err) {
      console.error('Échec du callback Cognito', err);
    } finally {
      // Quoi qu'il arrive, on revient à la racine
      this.router.navigate(['/']);
    }
  }
}
