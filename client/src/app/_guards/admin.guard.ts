import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  const roles = accountService.roles();

  if (roles.includes('Admin')) {
    return true;
  }

  toastr.error("Access Denied");
  return false;
};