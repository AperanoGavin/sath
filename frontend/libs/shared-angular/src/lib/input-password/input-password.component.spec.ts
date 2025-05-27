import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { TextFieldModule } from '@angular/cdk/text-field';
import { InputPasswordComponent } from './input-password.component';
import { CommonModule } from '@angular/common';

describe('InputPasswordComponent', () => {
  let component: InputPasswordComponent;
  let fixture: ComponentFixture<InputPasswordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        ReactiveFormsModule,
        MatInputModule,
        TextFieldModule,
        MatIconModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(InputPasswordComponent);
    component = fixture.componentInstance;

    component.placeholder = 'Password Placeholder';
    component.label = 'Password Label';
    component.formControl = new FormControl();

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the correct placeholder', () => {
    expect(component.placeholder).toBe('Password Placeholder');
  });

  it('should have the correct label', () => {
    expect(component.label).toBe('Password Label');
  });

  it('should have a required FormControl', () => {
    expect(component.formControl).toBeInstanceOf(FormControl);
  });

  it('should toggle password visibility', () => {
    expect(component.hidden).toBe(true);
    component.hidden = !component.hidden;
    expect(component.hidden).toBe(false);
  });

  it('should update the FormControl value', () => {
    const testValue = 'Test Password';
    component.formControl.setValue(testValue);
    expect(component.formControl.value).toBe(testValue);
  });
});
