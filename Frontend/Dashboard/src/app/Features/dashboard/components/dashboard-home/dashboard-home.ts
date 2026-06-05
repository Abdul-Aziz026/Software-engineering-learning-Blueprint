import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService, GreetMessage } from '../../Services/dashboard.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dashboard-home',
  imports: [CommonModule],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss',
})
export class DashboardHome implements OnInit {
  greetMessage$!: Observable<GreetMessage>;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.greetMessage$ = this.dashboardService.getGreetMessage();
  }
}