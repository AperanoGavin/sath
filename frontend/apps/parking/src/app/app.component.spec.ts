import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { provideStore } from '@ngrx/store';
import { provideHttpClient, withFetch, withInterceptorsFromDi } from '@angular/common/http';
import { AuthStore } from './stores/auth.store';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        provideStore(),
        AuthStore,
        provideHttpClient(
          withInterceptorsFromDi(),
          withFetch()
        ),
      ],
      imports: [AppComponent, RouterModule.forRoot([]), TranslateModule.forRoot()],
    }).compileComponents();
  });

  it(`should have as title 'parking'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('parking');
  });
});
