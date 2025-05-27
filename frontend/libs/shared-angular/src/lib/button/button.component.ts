import { ChangeDetectionStrategy, Component, EventEmitter, input, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'lib-button',
  standalone: true,
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
  imports: [MatButtonModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ButtonComponent {
  type = 'button';

  text = input<string>('Button')
  disabled = input<boolean>(false)
  icon = input<string>('')

  @Output() out = new EventEmitter();

  onClick() {
    if (this.disabled()) {
      return;
    }

    this.out.emit();
  }
}
