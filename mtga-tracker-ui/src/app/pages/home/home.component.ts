import { Component, OnInit, inject } from '@angular/core';
import { ApiService, UserInfo } from '../../services/api.service';
import { AsyncPipe, JsonPipe, NgFor } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { ReactiveFormsModule, FormControl } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NgFor, AsyncPipe, JsonPipe, MatFormFieldModule, MatSelectModule, ReactiveFormsModule],
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  private apiService = inject(ApiService);

  users$ = this.apiService.getUsers();

  userControl = new FormControl('');
  users: UserInfo[] = [];

  ngOnInit(): void {
    this.apiService.getUsers().subscribe(users => {
      this.users = users;
    });
  }
}