import { FitnessCheckComponent } from './fitness-check.component';
import { Member } from '../_models/member';

import { UserService } from '../_services/user.service';
import { Router } from '@angular/router';

describe('FitnessCheckComponent', () => {
  let component: FitnessCheckComponent;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockUserService = jasmine.createSpyObj('UserService', ['someMethod']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    component = new FitnessCheckComponent(mockUserService, mockRouter);
  });

  describe('getFitnessLevel()', () => {
    it('should return "Beginner" for age 25 and score 50', () => {
      component.totalScore = 50;
      const mockMember: Member = {
        id: 1,
        username: 'test',
        age: 25,
        weight: 70,
        height: 170,
        gender: 'M',
        fitnessLevel: '',
        traineeGoal: ''
      };

      const result = component.getFitnessLevel(mockMember);
      expect(result).toBe('Beginner');
    });

    it('should return "Advanced" for age 25 and score 130', () => {
      component.totalScore = 130;
      const mockMember: Member = {
        id: 1,
        username: 'test',
        age: 25,
        weight: 70,
        height: 170,
        gender: 'M',
        fitnessLevel: '',
        traineeGoal: ''
      };

      const result = component.getFitnessLevel(mockMember);
      expect(result).toBe('Advanced');
    });
  });
});
