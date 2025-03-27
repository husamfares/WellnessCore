import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  model: any ={}; 
  accountService = inject(AccountService);
  openRegister = output<boolean>();


  login()
  {
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: error => console.log(error)
    })
  }

  logout()
  {
    this.accountService.logout();
  }

  openRegisterForm()
  {
    this.openRegister.emit(true);
  }
}
