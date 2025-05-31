import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuestionComponent } from './question.component';
import { QuestionService } from '../_services/question.service';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { Question } from '../_models/Question';

describe('QuestionComponent', () => {
  let component: QuestionComponent;
  let fixture: ComponentFixture<QuestionComponent>;
  let mockQuestionService: jasmine.SpyObj<QuestionService>;

  beforeEach(async () => {
    mockQuestionService = jasmine.createSpyObj('QuestionService', ['postQuestion']);
    mockQuestionService.postQuestion.and.returnValue(of({id: 1,
  caption: 'Why is Angular awesome?',
  askedBy: 'test_user',
  createdAt: new Date().toISOString(),
  answers: []
} as Question));

    await TestBed.configureTestingModule({
      imports: [QuestionComponent, ReactiveFormsModule], // standalone component
      providers: [
        { provide: QuestionService, useValue: mockQuestionService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have an invalid form initially', () => {
    expect(component.form.valid).toBeFalse();
  });

  it('should validate the caption field as required', () => {
    const caption = component.form.get('caption');
    caption?.setValue('');
    expect(caption?.valid).toBeFalse();

    caption?.setValue('A sample question');
    expect(caption?.valid).toBeTrue();
  });

  it('should call postQuestion and reset the form on submit', () => {
    component.form.setValue({ caption: 'Why is Angular awesome?' });

    component.submit();

    expect(mockQuestionService.postQuestion).toHaveBeenCalledWith({ caption: 'Why is Angular awesome?' });
    expect(component.form.value.caption).toBeNull();
    expect(component.form.valid).toBeFalse(); // should be invalid again after reset
  });

  it('should not call postQuestion if form is invalid', () => {
    component.form.setValue({ caption: '' });
    component.submit();
    expect(mockQuestionService.postQuestion).not.toHaveBeenCalled();
  });
});
