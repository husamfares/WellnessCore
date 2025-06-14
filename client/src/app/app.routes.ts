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
import { FitnessCheckComponent } from './fitness-check/fitness-check.component';
import { QuestionPageComponent } from './question-page/question-page.component';
import { WorkoutPlanComponent } from './workout-plan/workout-plan.component';
import { fitnessCheckGuard } from './_guards/fitness-check.guard';
import { StandardWorkoutPlanComponent } from './standard-workout-plan/standard-workout-plan.component';
import { HomeWorkoutComponent } from './home-workout/home-workout.component';
import { TherapistExercisesComponent } from './therapist-exercises/therapist-exercises.component';
import { RecoveryComponent } from './Recoverycomponents/recovery/recovery.component';
import { ProfileComponent } from './profile/profile.component';
import { ProfileListComponent } from './profile-list/profile-list.component';
import { NutritionGuideComponent } from './nutrition-guide/nutrition-guide.component';
import { MealAnalyzerComponent } from './meal-analyzer/meal-analyzer.component';
import { ChatbotComponent } from './chatbot/chatbot.component';


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
      path: 'recovery', 
      component: RecoveryComponent,
     canActivate: [authGuard] 
    },
    {
      path : 'workout-plan',
      component: WorkoutPlanComponent,
      canActivate: [fitnessCheckGuard],
    },
    
    { 
      path: 'standard',
      component: StandardWorkoutPlanComponent,
      canActivate: [fitnessCheckGuard],
    },

    {
      path : "home-workout", 
      component : HomeWorkoutComponent,
      canActivate: [fitnessCheckGuard],
    },

    {
      path: 'therapist-exercises',
      component: TherapistExercisesComponent,
      canActivate: [fitnessCheckGuard],
    },

    
    { path : 'fitness-check',
      component: FitnessCheckComponent,
      canActivate: [authGuard]
    },
    
    {
      path : 'profile/:username',
      component: ProfileComponent,
      canActivate: [authGuard]
    },
    { 
      path: 'profiles',
      component: ProfileListComponent,
      canActivate: [authGuard] 
    },
    {
        path: 'nutrition-guide/:username',
        component: NutritionGuideComponent,
        canActivate: [authGuard , fitnessCheckGuard],

    },
    {
        path: 'meal-analyzer',
        component: MealAnalyzerComponent,
        canActivate: [authGuard]
    },
     {
        path: 'chatbot',
        component: ChatbotComponent,
        canActivate: [authGuard]
    },                
    { path: 'errors', component: TestErrorsComponent },
    { path: 'not-found', component: NotFoundComponent },
    { path: 'server-error', component: ServerErrorComponent },
    { path: '**', component: LoginComponent, canActivate: [loggedInGuard] }
  ];