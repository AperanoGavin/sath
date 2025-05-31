import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout-callback',
  template: `
    <p>Déconnecté avec succès. Redirection vers l’accueil…</p>
  `
})
export class LogoutCallbackComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit() {
    setTimeout(() => {
      this.router.navigate(['/']);
    }, 1000);
  }
}
