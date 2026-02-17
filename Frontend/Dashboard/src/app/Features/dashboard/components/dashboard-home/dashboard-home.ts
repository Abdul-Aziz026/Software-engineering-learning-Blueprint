import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../Services/dashboard.service';

@Component({
  selector: 'app-dashboard-home',
  imports: [],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss',
})
export class DashboardHome implements OnInit {
  greetMessage:string = '';
  constructor(private dashboardservice: DashboardService){}

  ngOnInit(): void {
    this.dashboardservice.getGreetMessage().subscribe({
      next: (res) => {
        console.log("Response:", res);
        this.greetMessage = res;
      },
      error: (err) => {
        console.error(err);
      }
    })
  }

}
