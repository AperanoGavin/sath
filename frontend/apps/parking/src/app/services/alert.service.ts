import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar'

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  constructor(
    private snackBar: MatSnackBar
  ) { }

  info(message: string, action = 'Close') {
    this.snackBar.open(message, action);
  }

  error(message: string, action = 'Close') {
    console.error(message);
    this.snackBar.open(message, action, {
      panelClass: ['error']
    });
  }
}
