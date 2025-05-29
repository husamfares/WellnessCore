import { Component, inject} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Router} from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  model: any ={}; 
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr= inject(ToastrService);


  login()
{
  this.accountService.login(this.model).subscribe({
    next: _ => {
      this.router.navigateByUrl('/home')
    },
    error: error => {
      // If the backend sent a clear message, show it:
      if (error.error && typeof error.error === 'string') {
        this.toastr.error(error.error);
      } 
      // Otherwise, use a fallback (or don't show anything)
      else if (error.status === 401) {
        this.toastr.error('Unauthorized');
      } 
      else {
        this.toastr.error('An unknown error occurred.');
      }
    }
  })
}


}
