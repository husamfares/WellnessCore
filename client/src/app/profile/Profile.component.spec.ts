import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfileComponent } from './profile.component';
import { ProfileService } from '../_services/profile.service';
import { AccountService } from '../_services/account.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Profile } from '../_models/profile';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let mockProfileService: jasmine.SpyObj<ProfileService>;
  let mockAccountService: jasmine.SpyObj<AccountService>;

  const mockProfile: Profile = {
    id: 1,
    username: 'john',
    role: 'Trainer',
    mobileNumber: '1234567890',
    location: 'NY',
    gymName: 'Gold Gym',
    clinicName: '',
    profilePictureUrl: 'http://image.com/pic.jpg',
    subscriptions: [],
    sessionPrices: []
  };

  beforeEach(async () => {
    mockProfileService = jasmine.createSpyObj('ProfileService', [
      'getProfile',
      'updateProfile',
      'uploadProfilePicture',
      'deleteProfilePicture',
      'addTrainerSubscription',
      'addTherapistSessionPrice',
      'deleteSubscription',
      'deleteSessionPrice'
    ]);

    mockAccountService = jasmine.createSpyObj('AccountService', ['currentUser']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, ProfileComponent],
      providers: [
        { provide: ProfileService, useValue: mockProfileService },
        { provide: AccountService, useValue: mockAccountService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => 'john'
              }
            }
          }
        }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
  });

  it('should create component and load profile', () => {
    mockProfileService.getProfile.and.returnValue(of(mockProfile));
    mockAccountService.currentUser.and.returnValue({
        username: 'john',
        token: '',
        roles: [],
        gender: '',
        id: 0
    });

    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(component.profile).toEqual(mockProfile);
    expect(component.isOwner).toBeTrue();
  });

  it('should update profile', () => {
    mockProfileService.getProfile.and.returnValue(of(mockProfile));
    mockProfileService.updateProfile.and.returnValue(of({}));
    mockAccountService.currentUser.and.returnValue({
        username: 'john',
        token: '',
        roles: [],
        gender: '',
        id: 0
    });

    fixture.detectChanges();
    component.form.controls['mobileNumber'].setValue('9876543210');
    component.form.controls['location'].setValue('LA');

    component.updateProfile();

    expect(mockProfileService.updateProfile).toHaveBeenCalled();
  });

  it('should call uploadProfilePicture when file is selected', () => {
    const mockFile = new File([''], 'test.png', { type: 'image/png' });
    const event = { target: { files: [mockFile] } } as unknown as Event;

    mockProfileService.uploadProfilePicture.and.returnValue(of({ imageUrl: 'http://new-image.com/pic.jpg' }));
    component.profile = { ...mockProfile };
    component.onFileSelected(event);

    expect(mockProfileService.uploadProfilePicture).toHaveBeenCalledWith(mockFile);
    expect(component.imageUrl).toBe('http://new-image.com/pic.jpg');
  });

  it('should call deleteProfilePicture and update profile', () => {
    mockProfileService.deleteProfilePicture.and.returnValue(of({}));
    component.profile = { ...mockProfile };

    component.deleteProfilePicture();

    expect(mockProfileService.deleteProfilePicture).toHaveBeenCalled();
    expect(component.imageUrl).toBe('');
    expect(component.profile.profilePictureUrl).toBe('');
  });
});
