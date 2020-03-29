import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  // Property
  username: any;
  password: any;

  constructor(
    public authService: AuthService,
    private alertService: AlertifyService
  ) {}

  ngOnInit() {}

  login() {
    // Convert to a single object later one : (PENDING FIX)
    let model = {
      username: this.username,
      password: this.password
    };
    this.authService.login(model).subscribe(
      next => {
        this.alertService.success('Logged in successfully!');
      },
      error => {
        this.alertService.error(error);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertService.message('Logged out');
  }
}
