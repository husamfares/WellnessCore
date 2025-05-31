import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { QuestionListComponent } from './question-list.component';
import { of } from 'rxjs';
import { Question } from '../_models/Question';
import { Profile } from '../_models/profile';
import { FormBuilder } from '@angular/forms';
import { QuestionService } from '../_services/question.service';
import { ProfileService } from '../_services/profile.service';
import { AccountService } from '../_services/account.service';
import { ActivatedRoute } from '@angular/router';

describe('QuestionListComponent', () => {
  let component: QuestionListComponent;
  let fixture: ComponentFixture<QuestionListComponent>;
  let mockQuestionService: jasmine.SpyObj<QuestionService>;
  let mockProfileService: jasmine.SpyObj<ProfileService>;
  let mockAccountService: jasmine.SpyObj<AccountService>;

  const dummyQuestion: Question = {
    id: 1,
    caption: 'What is Angular?',
    askedBy: 'john_doe',
    createdAt: new Date().toISOString(),
    answers: [
      {
        id: 1,
        answerText: 'A frontend framework.',
        answeredBy: 'trainer1',
        createdAt: new Date().toISOString(),
      }
    ]
  };

  const dummyProfile: Profile = {
    id: 1,
    username: 'john_doe',
    role: 'user',
    mobileNumber: '1234567890',
    location: 'Earth',
    gymName: 'FitGym',
    clinicName: 'WellClinic',
    profilePictureUrl: '',
    subscriptions: [],
    sessionPrices: [],
  };

  beforeEach(async () => {
    mockQuestionService = jasmine.createSpyObj('QuestionService', [
      'getQuestions', 'postAnswer', 'updateQuestion', 'deleteQuestion', 'deleteAnswer', 'updateAnswer'
    ]);
    mockQuestionService.getQuestions.and.returnValue(of([dummyQuestion]));
    mockQuestionService.postAnswer.and.returnValue(of({
      id: 99,
      answerText: 'This is a test answer',
      answeredBy: 'trainer1',
      createdAt: new Date().toISOString()
    }));
    mockQuestionService.updateQuestion.and.returnValue(of({ ...dummyQuestion, caption: 'Updated caption' }));
    mockQuestionService.deleteQuestion.and.returnValue(of({}));
    mockQuestionService.deleteAnswer.and.returnValue(of({}));
    mockQuestionService.updateAnswer.and.returnValue(of({
      id: 1,
      answerText: 'Updated answer',
      answeredBy: 'trainer1',
      createdAt: new Date().toISOString()
    }));
    mockQuestionService.newQuestion$ = of();

    mockProfileService = jasmine.createSpyObj('ProfileService', ['getProfile']);
    mockProfileService.getProfile.and.returnValue(of(dummyProfile));

    mockAccountService = jasmine.createSpyObj('AccountService', ['isTrainerOrTherapist']);
    mockAccountService.isTrainerOrTherapist.and.returnValue(true);

    await TestBed.configureTestingModule({
      imports: [QuestionListComponent],
      providers: [
        { provide: FormBuilder, useClass: FormBuilder },
        { provide: QuestionService, useValue: mockQuestionService },
        { provide: ProfileService, useValue: mockProfileService },
        { provide: AccountService, useValue: mockAccountService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {},
            params: of({}),
            queryParams: of({}),
            data: of({})
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load questions on init', () => {
    expect(mockQuestionService.getQuestions).toHaveBeenCalled();
    expect(component.questions.length).toBeGreaterThan(0);
    expect(component.answerForms[dummyQuestion.id]).toBeTruthy();
  });

  it('should submit an answer', () => {
    const form = component.answerForms[dummyQuestion.id];
    form.setValue({ answerText: 'My answer' });

    component.submitAnswer(dummyQuestion.id);

    expect(mockQuestionService.postAnswer).toHaveBeenCalledWith(dummyQuestion.id, { answerText: 'My answer' });
  });

  it('should enter and save edited question', () => {
    component.startEditQuestion(dummyQuestion);
    expect(component.editQuestionId).toBe(dummyQuestion.id);
    expect(component.editedCaption).toBe(dummyQuestion.caption);

    component.editedCaption = 'Updated caption';
    component.saveEditedQuestion(dummyQuestion);

    expect(mockQuestionService.updateQuestion).toHaveBeenCalledWith(dummyQuestion.id, jasmine.objectContaining({
      caption: 'Updated caption'
    }));
  });

  it('should delete a question after confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    component.questions = [dummyQuestion];
    component.deleteQuestion(dummyQuestion.id);

    expect(mockQuestionService.deleteQuestion).toHaveBeenCalledWith(dummyQuestion.id);
  });

 it('should cache and reuse profile data', fakeAsync(() => {
  // Clear any ngOnInit side effects
  mockProfileService.getProfile.calls.reset();
  component.profileCache = {}; // clear the cache manually

  component.loadProfile('john_doe'); // should call getProfile
  tick();

  component.loadProfile('john_doe'); // should NOT call getProfile again
  tick();

  expect(mockProfileService.getProfile).toHaveBeenCalledTimes(1);
}));
});
