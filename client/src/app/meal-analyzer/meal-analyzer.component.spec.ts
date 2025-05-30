import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MealAnalyzerComponent } from './meal-analyzer.component';
import { MealAnalyzerService } from '../_services/meal-analyzer.service';
import { of, throwError } from 'rxjs';
import { MealAnalysis } from '../_models/MealAnalysis';
import { By } from '@angular/platform-browser';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('MealAnalyzerComponent', () => {
  let component: MealAnalyzerComponent;
  let fixture: ComponentFixture<MealAnalyzerComponent>;
  let mockService: jasmine.SpyObj<MealAnalyzerService>;

  const mockHistory: MealAnalysis[] = [
    {
      imageUrl: 'http://img.com/apple.jpg',
      food: 'Apple',
      calories: 95,
      protein: 0.5,
      createdAt: '2024-05-30T12:00:00Z'
    }
  ];

  beforeEach(async () => {
    mockService = jasmine.createSpyObj('MealAnalyzerService', ['analyzeImage', 'getHistory']);

    await TestBed.configureTestingModule({
      imports: [MealAnalyzerComponent], 
      providers: [{ provide: MealAnalyzerService, useValue: mockService }],
      schemas: [NO_ERRORS_SCHEMA] 
    }).compileComponents();

    fixture = TestBed.createComponent(MealAnalyzerComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    mockService.getHistory.and.returnValue(of([])); 
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load history on init', () => {
    mockService.getHistory.and.returnValue(of(mockHistory));
    fixture.detectChanges();
    expect(mockService.getHistory).toHaveBeenCalled();
    expect(component.history.length).toBe(1);
  });

  it('should update selectedFile on file select', () => {
    const file = new File(['image'], 'meal.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [file] } };
    component.onFileSelected(event);
    expect(component.selectedFile).toBe(file);
  });

  it('should call analyzeImage and set result', () => {
    const file = new File(['meal'], 'meal.jpg', { type: 'image/jpeg' });
    const mockResult: MealAnalysis = {
      imageUrl: 'http://img.com/burger.jpg',
      food: 'Burger',
      calories: 250,
      protein: 15,
      createdAt: '2024-05-30T12:00:00Z'
    };

    component.selectedFile = file;
    mockService.analyzeImage.and.returnValue(of(mockResult));
    mockService.getHistory.and.returnValue(of(mockHistory)); 

    component.analyze();

    expect(mockService.analyzeImage).toHaveBeenCalledWith(file);
    expect(mockService.getHistory).toHaveBeenCalled();
    expect(component.result).toEqual(mockResult);
    expect(component.loading).toBeFalse();
  });

  it('should handle analyzeImage error gracefully', () => {
    component.selectedFile = new File(['bad'], 'bad.jpg', { type: 'image/jpeg' });
    mockService.analyzeImage.and.returnValue(throwError(() => new Error('Upload failed')));

    spyOn(console, 'error');
    component.analyze();

    expect(console.error).toHaveBeenCalledWith(jasmine.any(Error));
    expect(component.result).toBeNull();
    expect(component.loading).toBeFalse();
  });

  it('should disable analyze button when no file is selected or loading is true', () => {
    mockService.getHistory.and.returnValue(of([])); 
    fixture.detectChanges(); 

    const button = fixture.debugElement.query(By.css('button')).nativeElement;

    // No file selected
    expect(button.disabled).toBeTrue();

    // File selected, loading
    component.selectedFile = new File(['data'], 'image.jpg');
    component.loading = true;
    fixture.detectChanges();
    expect(button.disabled).toBeTrue();

    // File selected, not loading
    component.loading = false;
    fixture.detectChanges();
    expect(button.disabled).toBeFalse();
  });
});
