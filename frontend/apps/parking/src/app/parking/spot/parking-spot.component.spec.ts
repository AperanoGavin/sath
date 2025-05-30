import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ParkingSpotComponent } from './parking-spot.component';
import { By } from '@angular/platform-browser';
import { TranslateModule, TranslateStore } from '@ngx-translate/core';
import { provideStore } from '@ngrx/store';
import { provideHttpClient, withFetch, withInterceptorsFromDi } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ParkingStore } from '../../stores/parking.store';
import { AlertService } from '../../services/alert.service';

describe('ParkingSpotComponent', () => {
  let component: ParkingSpotComponent;
  let fixture: ComponentFixture<ParkingSpotComponent>;

  beforeAll(() => {
    // https://github.com/jsdom/jsdom/issues/2527
    (window as any).DragEvent = MouseEvent;
    (window as any).confirm = () => true;
  });

  beforeEach(async () => {
    await TestBed.configureTestingModule({
        providers: [
            provideStore(),
            ParkingStore,
            provideHttpClient(
              withInterceptorsFromDi(),
              withFetch()
            ),
          ],
          imports: [ParkingSpotComponent, RouterModule.forRoot([]), TranslateModule.forRoot()],
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParkingSpotComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('spotNumber', 5);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display an image if car is defined', () => {
    component.car.set('/image.png');
    fixture.detectChanges();
    const img = fixture.debugElement.query(By.css('img'));
    expect(img).toBeTruthy();
    expect(img.nativeElement.src).toContain('/image.png');
  });


  it('should display a paragraph with spot number if car is not defined', () => {
    component.car.set(null);
    const spotNumber = 5;
  
    fixture.detectChanges();
  
    const p = fixture.debugElement.query(By.css('p'));
    expect(p).toBeTruthy();
    expect(p.nativeElement.textContent).toContain(`Place n°${spotNumber}`);
  });
  
  

  it('should call spotClicked when clicked', () => {
    jest.spyOn(component, 'spotClicked');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    div.triggerEventHandler('click', null);
    expect(component.spotClicked).toHaveBeenCalled();
  });

  it('should call spotClicked when a key is pressed', () => {
    jest.spyOn(component, 'spotClicked');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    const event = new KeyboardEvent('keypress', { key: 'Enter' });
    div.triggerEventHandler('keypress', event);
    expect(component.spotClicked).toHaveBeenCalled();
  });

  it('should call onDragOver on dragover event', () => {
    jest.spyOn(component, 'onDragOver');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    const event = new DragEvent('dragover');
    div.triggerEventHandler('dragover', event);
    expect(component.onDragOver).toHaveBeenCalledWith(event);
  });

  it('should call onDrop on drop event', () => {
    jest.spyOn(component, 'onDrop');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    const event = new DragEvent('drop');
    div.triggerEventHandler('drop', event);
    expect(component.onDrop).toHaveBeenCalledWith(event);
  });
});
