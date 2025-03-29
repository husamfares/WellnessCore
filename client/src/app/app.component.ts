import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from "./login/login.component";
import { AccountService } from './_services/account.service';
import { RegisterComponent } from "./registerInfo/register/register.component";

@Component({
  selector: 'app-root',
  imports: [  RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title = 'WellnessCore';
  http = inject(HttpClient);
  private accountService = inject(AccountService);
  users: any;
  registerMode = false;


  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser()
  {
    const userString = localStorage.getItem('user');
    if(!userString) return;

    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

  cancelRegisterMode(event: boolean)
  {
    this.registerMode = event;
  }

}
