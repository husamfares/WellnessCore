import {  Routes } from '@angular/router';
import { RegisterComponent } from './registerInfo/register/register.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';

export const routes: Routes = 
[
    {path: '' , component: LoginComponent},
    {path: 'register' , component: RegisterComponent},
    {path: 'login' , component: LoginComponent},
    {path: 'home' , component: HomeComponent},
    {path: 'errors', component: TestErrorsComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: 'server-error', component: ServerErrorComponent},
    {path: '**', component: LoginComponent},
    


];


