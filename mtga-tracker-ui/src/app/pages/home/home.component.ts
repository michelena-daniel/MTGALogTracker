import { Component, OnInit, inject } from '@angular/core';
import { ApiService, UserInfo } from '../../services/api.service';
import { AsyncPipe, JsonPipe, NgFor, DatePipe, NgIf, NgClass } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { Match } from '../../services/api.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, AsyncPipe, JsonPipe, DatePipe, MatFormFieldModule, MatSelectModule, ReactiveFormsModule, MatTableModule],
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  private apiService = inject(ApiService);

  users$ = this.apiService.getUsers();
  matches: Match[] = [];

  userControl = new FormControl('');
  users: UserInfo[] = [];

  displayedColumns: string[] = ['matchId', 'playerOne', 'playerTwo', 'timestamp', 'winner', 'winnerMtgArenaId', 'homeUser'];


  ngOnInit(): void {
    this.apiService.getUsers().subscribe(users => {
      this.users = users;
    });

    this.userControl.valueChanges.subscribe(mtgArenaId => {
      if(mtgArenaId) {
        this.apiService.getMatches(mtgArenaId).subscribe(matches => {
          console.log("Matches fetched:", matches);
          this.matches = matches;
        })
      }
    });
  }

  getRowClass(row: Match): string {
    return row.winnerMtgArenaId === row.homeUser ? 'win' : 'lose';
  }
}