import { Component, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  // Properties
  registerMode = false;

  constructor(private http: HttpClient) {}

  ngOnInit() {}

  registerToggle() {
    this.registerMode = true;
  }

  cancelRegisterMode(mode: boolean) {
    this.registerMode = mode;
  }
}
