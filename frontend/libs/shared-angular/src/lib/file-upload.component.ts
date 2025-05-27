import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'lib-file-upload',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
        class="drop-zone"
        (dragover)="onDragOver($event)"
        (dragleave)="onDragLeave($event)"
        (drop)="onDrop($event)"
        (click)="fileInput.click()"
        >
        <input
            multiple
            type="file"
            #fileInput
            style="display: none;"
            (change)="onFileSelected($event)"
        />
        <p class="hover:underline underline-offset-2 hover:text-accent duration-100">Drag and drop files here or click to upload</p>
        </div>

        <div
        *ngIf="isDragOver"
        class="drag-over"
        >
        <div class="plus pointer-events-none">+</div>
        </div>
  `,
  styles: [
    `
      .drop-zone {
        border: 2px dashed #ccc;
        border-radius: 10px;
        padding: 20px;
        text-align: center;
        cursor: pointer;
      }
      
      .drag-over {
        background-color: rgba(0, 0, 0, 0.5);
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 1000;
        display: flex;
        justify-content: center;
        align-items: center;
      }

      .drag-over .plus {
        font-size: 10rem;
        color: white;
      }
    `,
  ]
})
export class FileUploadComponent {
  isDragOver = false;

  @Output() filesUploaded = new EventEmitter<FileList>();

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.filesUploaded.emit(files);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.filesUploaded.emit(input.files);
    }
  }
}