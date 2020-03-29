import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  // Property
  username: any;
  password: any;

  constructor(private authService: AuthService) {}

  ngOnInit() {}

  login() {
    let model = {
      username: this.username,
      password: this.password
    };
    this.authService.login(model).subscribe(
      next => {
        console.log('Logged in');
      },
      error => {
        console.log(error);
      }
    );
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout() {
    localStorage.removeItem('token');
    console.log('Logged out!');
  }
}
