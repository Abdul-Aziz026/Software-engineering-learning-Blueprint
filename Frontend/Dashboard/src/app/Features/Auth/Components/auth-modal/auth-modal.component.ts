import { Component, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../Services/auth.service';
import { AuthModalService, AuthMode } from '../../Services/auth-modal.service';

@Component({
  selector: 'app-auth-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth-modal.component.html',
  styleUrl: './auth-modal.component.scss'
})
export class AuthModalComponent {
  isSubmitting = false;
  errorMessage = '';

  username = '';
  email = '';
  emailOrUsername = '';
  password = '';

  constructor(public modal: AuthModalService, private authService: AuthService) {
    effect(() => {
      if (this.modal.isOpen()) this.resetForm();
    });
  }

  switchMode(mode: AuthMode): void {
    if (this.modal.mode() === mode) return;
    this.modal.switchMode(mode);
    this.errorMessage = '';
  }

  close(): void {
    if (this.isSubmitting) return;
    this.modal.close();
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget) this.close();
  }

  submit(form: NgForm): void {
    if (form.invalid || this.isSubmitting) return;

    this.errorMessage = '';
    this.isSubmitting = true;

    const request$ = this.modal.mode() === 'signup'
      ? this.authService.signup({
          username: this.username.trim(),
          email: this.email.trim(),
          password: this.password
        })
      : this.authService.login({
          emailOrUsername: this.emailOrUsername.trim(),
          password: this.password
        });

    request$.subscribe({
      next: () => {
        this.isSubmitting = false;
        this.modal.close();
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting = false;
        this.errorMessage = err.error?.message
          ?? err.error?.details
          ?? 'Something went wrong. Please try again.';
      }
    });
  }

  private resetForm(): void {
    this.username = '';
    this.email = '';
    this.emailOrUsername = '';
    this.password = '';
    this.errorMessage = '';
    this.isSubmitting = false;
  }
}
