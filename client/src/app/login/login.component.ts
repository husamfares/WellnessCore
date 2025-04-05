import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  imports: [FormsModule , RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  model: any ={}; 
  accountService = inject(AccountService);
  private router = inject(Router);
  openRegister = output<boolean>();
  private toastr= inject(ToastrService);


  login()
  {
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/home')
      },
      error: error => this.toastr.error(error.error)
    })
  }

  logout()
  {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  openRegisterForm()
  {
    this.openRegister.emit(true);
  }
}
