import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ParkingSpotComponent } from './parking-spot.component';
import { By } from '@angular/platform-browser';
import { TranslateModule } from '@ngx-translate/core';
import { provideStore } from '@ngrx/store';
import { provideHttpClient, withFetch, withInterceptorsFromDi } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ParkingStore } from '../../stores/parking.store';
import { AlertService } from '../../services/alert.service';

describe('ParkingSpotComponent', () => {
  let component: ParkingSpotComponent;
  let fixture: ComponentFixture<ParkingSpotComponent>;
  let alertService: AlertService;

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
        provideHttpClient(withInterceptorsFromDi(), withFetch()),
        { provide: AlertService, useValue: { error: jest.fn() } }
      ],
      imports: [ParkingSpotComponent, RouterModule.forRoot([]), TranslateModule.forRoot()]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParkingSpotComponent);
    component = fixture.componentInstance;
    alertService = TestBed.inject(AlertService);
    fixture.componentRef.setInput('id', '1');
    fixture.componentRef.setInput('spotNumber', '5');
    fixture.componentRef.setInput('capabilities', ['ElectricCharger']);
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
    fixture.detectChanges();
    const p = fixture.debugElement.query(By.css('p'));
    expect(p).toBeTruthy();
    expect(p.nativeElement.textContent).toContain('Place n°5');
  });

  it('should call spotClicked when clicked', () => {
    const spy = jest.spyOn(component, 'spotClicked');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    div.triggerEventHandler('click', null);
    expect(spy).toHaveBeenCalled();
  });

  it('should call spotClicked when a key is pressed', () => {
    const spy = jest.spyOn(component, 'spotClicked');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    const event = new KeyboardEvent('keypress', { key: 'Enter' });
    div.triggerEventHandler('keypress', event);
    expect(spy).toHaveBeenCalled();
  });

  it('should call onDragOver on dragover event', () => {
    const spy = jest.spyOn(component, 'onDragOver');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    const event = new DragEvent('dragover');
    div.triggerEventHandler('dragover', event);
    expect(spy).toHaveBeenCalledWith(event);
  });

  // it('should call onDragLeave on dragleave event', () => {
  //   const spy = jest.spyOn(component, 'onDragLeave');
  //   const div = fixture.debugElement.query(By.css('.drop-zone'));
  //   const event = new DragEvent('dragleave');
  //   div.triggerEventHandler('dragleave', event);
  //   expect(spy).toHaveBeenCalledWith(event);
  // });

  it('should call onDrop on drop event', () => {
    const spy = jest.spyOn(component, 'onDrop');
    const div = fixture.debugElement.query(By.css('.drop-zone'));
    const event = new DragEvent('drop');
    Object.defineProperty(event, 'dataTransfer', {
      value: {
        getData: () => '<img src="/image.png" />'
      }
    });
    div.triggerEventHandler('drop', event);
    expect(spy).toHaveBeenCalledWith(event);
  });

  it('should show an error if spot is already used', () => {
    component.car.set('/image.png');
    const event = new DragEvent('drop');
    Object.defineProperty(event, 'dataTransfer', {
      value: {
        getData: () => '<img src="/another-image.png" />'
      }
    });
    component.onDrop(event);
    expect(alertService.error).toHaveBeenCalledWith('Emplacement déja utilisé', 'error');
  });

  it('should show an error if no date range is selected', () => {
    const event = new DragEvent('drop');
    Object.defineProperty(event, 'dataTransfer', {
      value: {
        getData: () => '<img src="/image.png" />'
      }
    });
    jest.spyOn(component.parkingStore, 'startDate').mockReturnValue(null);
    jest.spyOn(component.parkingStore, 'endDate').mockReturnValue(null);
    component.onDrop(event);
    expect(alertService.error).toHaveBeenCalledWith('Veuillez sélectionner une date de début et de fin avant de déposer la voiture.', 'error');
  });

  it('should create a spot reservation if slot is available and dates are selected', () => {
    const event = new DragEvent('drop');
    Object.defineProperty(event, 'dataTransfer', {
      value: {
        getData: () => '<img src="/image.png" />'
      }
    });
    const spy = jest.spyOn(component.parkingStore, 'createSpotReservation');
    jest.spyOn(component.parkingStore, 'startDate').mockReturnValue(new Date('2023-01-01'));
    jest.spyOn(component.parkingStore, 'endDate').mockReturnValue(new Date('2023-01-02'));
    component.onDrop(event);
    expect(spy).toHaveBeenCalled();
  });
});
