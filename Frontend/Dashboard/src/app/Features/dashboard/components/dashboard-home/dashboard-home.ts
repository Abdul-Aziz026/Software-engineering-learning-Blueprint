import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../Services/dashboard.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dashboard-home',
  imports: [CommonModule],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss',
})
export class DashboardHome implements OnInit {
  greetMessage$!: Observable<any>;

  constructor(private dashboardservice: DashboardService) {}

  ngOnInit(): void {
    this.greetMessage$ = this.dashboardservice.getGreetMessage();
  }
}