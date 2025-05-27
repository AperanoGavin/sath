/* eslint-disable @angular-eslint/no-input-rename */
import { ChangeDetectionStrategy, Component, input, Input, linkedSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { TextFieldModule } from '@angular/cdk/text-field';
import { ReactiveFormsModule, FormControl, AbstractControl } from '@angular/forms';

@Component({
  selector: 'lib-input-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    TextFieldModule,
    MatIconModule,
  ],
  templateUrl: './input-password.component.html',
  styleUrls: ['./input-password.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InputPasswordComponent {
  hidden = true;

  value = '';

  placeholder = input.required<string>()
  label = input.required<string>()
  _formControl = input.required<FormControl>({alias: 'control'})
  required = input<boolean>()
  type = input<string>('text')
}
