import {  Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { RegisterComponent } from './register/register.component';
import { loggedInGuard } from './_guards/logged-in.guard';


export const routes: Routes = [
    { 
      path: '', 
      component: LoginComponent,
      canActivate: [loggedInGuard] 
    },
    { 
      path: 'register', 
      component: RegisterComponent,
      canActivate: [loggedInGuard] 
    },
    { 
      path: 'login', 
      component: LoginComponent,
      canActivate: [loggedInGuard] 
    },
    { 
      path: 'home', 
      component: HomeComponent,
      canActivate: [authGuard] 
    },
    { path: 'errors', component: TestErrorsComponent },
    { path: 'not-found', component: NotFoundComponent },
    { path: 'server-error', component: ServerErrorComponent },
    { path: '**', component: LoginComponent, canActivate: [loggedInGuard] }
  ];