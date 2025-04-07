import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const loggedInGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  const router = inject(Router);

  if (accountService.currentUser()) {
    toastr.warning('You are already logged in!');
    router.navigateByUrl('/home'); // Redirect to home or dashboard
    return false;
  }
  return true;
};