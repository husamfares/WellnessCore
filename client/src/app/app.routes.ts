import {  Routes } from '@angular/router';
import { RegisterComponent } from './registerInfo/register/register.component';
import { LoginComponent } from './login/login.component';
import { WellnessInfoComponent } from './registerInfo/wellness-info/wellness-info.component';
import { HomeComponent } from './home/home.component';

export const routes: Routes = 
[
    {path: '' , component: LoginComponent},
    {path: 'register' , component: RegisterComponent},
    {path: 'wellness-info' , component: WellnessInfoComponent},
    {path: 'login' , component: LoginComponent},
    {path: 'home' , component: HomeComponent},


];


