import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../Services/dashboard.service';

@Component({
  selector: 'app-dashboard-home',
  imports: [CommonModule],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss',
})
export class DashboardHome implements OnInit {
  greetMessage: string = '';

  constructor(
    private dashboardservice: DashboardService,
    private cdr: ChangeDetectorRef  // ← Add this
  ) {}

  ngOnInit(): void {
    this.dashboardservice.getGreetMessage().subscribe({
      next: (res: any) => {
        console.log("Response:", res);
        this.greetMessage = res.message; // adjust to your API response shape
        this.cdr.detectChanges();  // ← Force UI update
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}