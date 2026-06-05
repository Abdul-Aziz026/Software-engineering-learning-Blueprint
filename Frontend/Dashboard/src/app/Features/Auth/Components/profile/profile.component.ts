import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { UpdateProfileRequest } from '../../Models/auth.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  username = '';
  email = '';
  currentPassword = '';
  newPassword = '';

  isEditing = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(public authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (!user) {
      this.router.navigate(['']);
      return;
    }
    this.username = user.username;
    this.email = user.email;
  }

  startEdit(): void {
    this.isEditing = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelEdit(): void {
    const user = this.authService.currentUser();
    if (user) {
      this.username = user.username;
      this.email = user.email;
    }
    this.currentPassword = '';
    this.newPassword = '';
    this.isEditing = false;
    this.errorMessage = '';
  }

  save(form: NgForm): void {
    if (form.invalid || this.isSubmitting) return;
    const user = this.authService.currentUser();
    if (!user) return;

    const payload: UpdateProfileRequest = {};
    if (this.username.trim() && this.username.trim() !== user.username) {
      payload.username = this.username.trim();
    }
    if (this.email.trim() && this.email.trim() !== user.email) {
      payload.email = this.email.trim();
    }
    if (this.newPassword) {
      payload.currentPassword = this.currentPassword;
      payload.newPassword = this.newPassword;
    }

    if (Object.keys(payload).length === 0) {
      this.errorMessage = 'No changes to save.';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.isSubmitting = true;

    this.authService.updateProfile(user.userId, payload).subscribe({
      next: (updated) => {
        this.isSubmitting = false;
        this.isEditing = false;
        this.username = updated.username;
        this.email = updated.email;
        this.currentPassword = '';
        this.newPassword = '';
        this.successMessage = 'Profile updated successfully.';
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting = false;
        this.errorMessage = err.error?.message
          ?? err.error?.details
          ?? 'Failed to update profile.';
      }
    });
  }

  get initial(): string {
    const u = this.authService.currentUser();
    return u?.username?.charAt(0)?.toUpperCase() ?? '?';
  }
}
