import {  Routes } from '@angular/router';
import { RegisterComponent } from './registerInfo/register/register.component';
import { LoginComponent } from './login/login.component';
import { WellnessInfoComponent } from './registerInfo/wellness-info/wellness-info.component';

export const routes: Routes = 
[
    {path: '' , component: LoginComponent},
    {path: 'register' , component: RegisterComponent},
    {path: 'wellness-info' , component: WellnessInfoComponent},
    {path: 'login' , component: LoginComponent},


];


