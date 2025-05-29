import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../_services/user.service';
import { map } from 'rxjs';

export const fitnessCheckGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router)

  return userService.getUserFitnessInfo().pipe(
    map(user => {
      console.log('Fitness check guard:', user); // 🔥 Log the user
      // Check if user.fitnessLevel and user.traineeGoal are valid values
      if (user && user.fitnessLevel && user.traineeGoal) {
        console.log('Fitness level and goal are set, redirecting to workout plan');
        return true;
      } else {
        console.log('Fitness level or goal not set, redirecting to fitness-check');
        router.navigate(['/fitness-check']);
        return false;
      }
    })
  );
};

