import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
// import { NgIf } from '@angular/common';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  model: any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => this.toastr.error(error.error),
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}

//angular template form
//reactive forms-contain inside our component

//what are observables?
//new standard for managing async data
//they are lazy collections of multiple values over time
//like newsletter only subscriber recieve the data
//no sub no printy

//promide vs observable
//single furture value vs multiple value over time
//not lazy vs lazy
//cannot cancel vs able to cancel
// can use with map, filter, reduce and other operators

//signals
//not asynchronous
//signal is a wrapper around a value that notifies interested
//consumers when the value changes
//deal with state management
