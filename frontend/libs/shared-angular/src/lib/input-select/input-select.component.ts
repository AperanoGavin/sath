/* eslint-disable @angular-eslint/no-input-rename */
import { ChangeDetectionStrategy, Component, input, Input, linkedSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { TextFieldModule } from '@angular/cdk/text-field';
import { MatSelectModule } from '@angular/material/select';
import { ReactiveFormsModule, FormControl, AbstractControl } from '@angular/forms';

@Component({
  selector: 'lib-input-select',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    TextFieldModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './input-select.component.html',
  styleUrls: ['./input-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InputSelectComponent {
  value = '';

  items = input.required<any[]>()
  label = input.required<string>()
  _formControl = input.required<FormControl>({alias: 'control'})
  required = input.required<boolean>()
  type = input<string>('text')
}

