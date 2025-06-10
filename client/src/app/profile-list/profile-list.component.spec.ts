import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfileListComponent } from './profile-list.component';
import { ProfileService } from '../_services/profile.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { Profile } from '../_models/profile';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

describe('ProfileListComponent', () => {
  let component: ProfileListComponent;
  let fixture: ComponentFixture<ProfileListComponent>;
  let mockProfileService: jasmine.SpyObj<ProfileService>;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockProfiles: Profile[] = [
    {
      id: 1,
      username: 'john_doe',
      role: 'admin',
      mobileNumber: '1234567890',
      location: 'NY',
      gymName: 'Gym1',
      clinicName: 'Clinic1',
      profilePictureUrl: '',
      subscriptions: [],
      sessionPrices: [],
    },
    {
      id: 2,
      username: 'jane_doe',
      role: 'user',
      mobileNumber: '0987654321',
      location: 'LA',
      gymName: 'Gym2',
      clinicName: 'Clinic2',
      profilePictureUrl: '',
      subscriptions: [],
      sessionPrices: [],
    },
  ];

  beforeEach(async () => {
    mockProfileService = jasmine.createSpyObj('ProfileService', ['getAllProfiles']);
    mockProfileService.getAllProfiles.and.returnValue(of(mockProfiles));

    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        ProfileListComponent, // 👈 standalone component goes here
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
      ],
      providers: [
        { provide: ProfileService, useValue: mockProfileService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load profiles on init', () => {
    expect(mockProfileService.getAllProfiles).toHaveBeenCalledWith('');
    expect(component.profiles.length).toBe(2);
  });

  it('should filter profiles by search term', () => {
    component.searchTerm = 'john';
    component.search();
    expect(component.filteredProfiles.length).toBe(1);
    expect(component.filteredProfiles[0].username).toBe('john_doe');
  });

  it('should filter profiles by role', () => {
    mockProfileService.getAllProfiles.and.returnValue(of([mockProfiles[0]]));
    component.filterByRole('admin');
    expect(mockProfileService.getAllProfiles).toHaveBeenCalledWith('admin');
    expect(component.filteredProfiles.length).toBe(1);
    expect(component.filteredProfiles[0].role).toBe('admin');
  });

  it('should navigate to profile', () => {
    component.goToProfile('john_doe');
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/profile', 'john_doe']);
  });
});
