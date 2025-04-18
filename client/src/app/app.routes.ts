import {  Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { RegisterComponent } from './register/register.component';
import { loggedInGuard } from './_guards/logged-in.guard';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
<<<<<<< HEAD
import { FitnessCheckComponent } from './fitness-check/fitness-check.component';
=======
import { QuestionPageComponent } from './question-page/question-page.component';
>>>>>>> fa3a3ca2062c0bc79f9d999908112b1e3ce63688


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
    {
      path: 'questions',
      component: QuestionPageComponent,
      canActivate: [authGuard]
    },
    { 
      path: 'admin', 
      component: AdminPanelComponent,
      canActivate: [authGuard,adminGuard] 
    },
    {
      path : 'fitness-check',
      component: FitnessCheckComponent
    },
    { path: 'errors', component: TestErrorsComponent },
    { path: 'not-found', component: NotFoundComponent },
    { path: 'server-error', component: ServerErrorComponent },
    { path: '**', component: LoginComponent, canActivate: [loggedInGuard] }
  ];