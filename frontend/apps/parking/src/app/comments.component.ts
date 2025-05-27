
import { Component, Inject, inject, input, linkedSignal, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';

import { TranslateModule } from '@ngx-translate/core';
import { TranslationKeys } from './keys.interface';
import { ButtonComponent, InputTextareaComponent } from '@parking/shared-angular';
import { RouterModule } from '@angular/router';
import { CommentService } from './services/comment.service';
import { CommentStore } from './stores/comment.store';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { TimeAgoPipe } from './pipes/date.pipe';

@Component({
  selector: 'app-parking-comments',
  standalone: true,
  providers: [CommentService],
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    MatButtonModule,
    TranslateModule,
    ButtonComponent,
    RouterModule,
    InputTextareaComponent,
    TimeAgoPipe,
  ],
  templateUrl: './comments.component.html',
})
export class CommentsComponent  {
  translationKeys = TranslationKeys

  readonly commentStore = inject(CommentStore);
  section = input.required<string>()
  comments = linkedSignal(() => Array.from(
    this.commentStore.comments()
    .values()
  ).sort((a, b) => new Date(b.created_at).getTime() - new Date(a.created_at).getTime()))
  form;

  constructor(
    private readonly fb: FormBuilder,
    @Inject(PLATFORM_ID) private platformId: object,
  ) {

    if (isPlatformBrowser(this.platformId)) {
      this.commentStore.getAllFrom(this.section)
    }

    this.form = this.fb.group({
      comment: ['', Validators.required],
    });
  }

  get commentControl() {
    return this.form.get('comment') as FormControl;
  }

  submitComment() {
    if (!this.commentControl.valid) return

    this.commentStore.create({
      dto: {
        content: this.commentControl.value,
        section: this.section(),
        parent: null
      }
    })
  }

  deleteComment(id: string) {
    if (confirm("Confirmer la suppression du message ?")) {
      this.commentStore.delete({id})
    }
  }

  like(id: string) {
    this.commentStore.like(id)
  }

  unlike(id: string) {
    this.commentStore.unlike(id)
  }

  isLiked(comments: string[]) {
    const likes = new Set(comments)
    console.log(likes)
    return likes.has("idtest")
  }
}
