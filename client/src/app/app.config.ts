import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { importProvidersFrom } from '@angular/core';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { jwtInterceptor } from './_interceptors/jwt.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { loadingInterceptor } from './_interceptors/loading.interceptor';
import { ModalModule} from 'ngx-bootstrap/modal';



export const appConfig: ApplicationConfig = {
  providers:[
  provideZoneChangeDetection({ eventCoalescing: true }),
  provideRouter(routes),
  provideHttpClient(withInterceptors([errorInterceptor,jwtInterceptor, loadingInterceptor])),
  importProvidersFrom(ToastrModule.forRoot()),
  importProvidersFrom(NgxSpinnerModule, ModalModule.forRoot()),
  importProvidersFrom(BrowserAnimationsModule)
  ]
};
