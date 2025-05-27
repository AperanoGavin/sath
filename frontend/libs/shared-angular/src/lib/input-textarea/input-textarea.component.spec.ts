import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { TextFieldModule } from '@angular/cdk/text-field';
import { InputTextareaComponent } from './input-textarea.component';
import { CommonModule } from '@angular/common';

describe('InputTextareaComponent', () => {
  let component: InputTextareaComponent;
  let fixture: ComponentFixture<InputTextareaComponent>;

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

    fixture = TestBed.createComponent(InputTextareaComponent);
    component = fixture.componentInstance;

    component.placeholder = 'Test Placeholder';
    component.label = 'Test Label';
    component.formControl = new FormControl();
    component.required = true;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the correct placeholder', () => {
    expect(component.placeholder).toBe('Test Placeholder');
  });

  it('should have the correct label', () => {
    expect(component.label).toBe('Test Label');
  });

  it('should have a required FormControl', () => {
    expect(component.formControl).toBeInstanceOf(FormControl);
  });

  it('should have the correct required attribute', () => {
    expect(component.required).toBe(true);
  });

  it('should have the correct type attribute', () => {
    expect(component.type).toBe('text');
  });

  it('should update the FormControl value', () => {
    const testValue = 'Test Value';
    component.formControl.setValue(testValue);
    expect(component.formControl.value).toBe(testValue);
  });
});
