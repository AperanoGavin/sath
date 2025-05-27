import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, input, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'lib-images-viewer',
  standalone: true,
  templateUrl: './images-viewer.component.html',
  imports: [MatButtonModule, MatIconModule, CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ImagesViewerComponent {
  images = input.required<string[]>()
  currentImageIndex = 0

  previousImage() {
    if (this.currentImageIndex > 0) {
      this.currentImageIndex--;
    }
  }

  nextImage() {
    if (this.currentImageIndex < (this.images().length) - 1) {
      this.currentImageIndex++;
    }
  }
}
