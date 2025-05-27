/* eslint-disable @angular-eslint/no-input-rename */
import { ChangeDetectionStrategy, Component, input, Input, linkedSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { TextFieldModule } from '@angular/cdk/text-field';
import { ReactiveFormsModule, FormControl, AbstractControl } from '@angular/forms';

@Component({
  selector: 'lib-input-textarea',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    TextFieldModule,
    MatIconModule,
  ],
  templateUrl: './input-textarea.component.html',
  styleUrls: ['./input-textarea.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InputTextareaComponent {
  value = '';

  placeholder = input.required<string>()
  label = input.required<string>()
  _formControl = input.required<FormControl>({alias: 'control'})
  required = input.required<boolean>()
  type = input<string>('text')
}

