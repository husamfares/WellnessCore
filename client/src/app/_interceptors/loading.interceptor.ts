import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../_services/busy.service';
import { delay, finalize, identity } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  
  const busyService = inject(BusyService);
  
  busyService.busy();

  return next(req).pipe(
    delay(400),
    finalize(() => {
      busyService.idle()
    })
  )
};