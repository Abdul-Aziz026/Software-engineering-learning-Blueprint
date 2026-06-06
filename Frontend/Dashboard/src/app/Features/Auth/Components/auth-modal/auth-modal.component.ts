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
  infoMessage = '';

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
    this.infoMessage = '';
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
    this.infoMessage = '';
    this.isSubmitting = true;

    if (this.modal.mode() === 'forgot') {
      this.authService.forgotPassword({ email: this.email.trim() }).subscribe({
        next: (res) => {
          this.isSubmitting = false;
          // Generic success message — backend never reveals whether the email exists.
          this.infoMessage = res?.message
            ?? 'If an account exists for that email, a reset link has been sent.';
        },
        error: (err: HttpErrorResponse) => {
          this.isSubmitting = false;
          this.errorMessage = err.error?.message
            ?? err.error?.details
            ?? 'Something went wrong. Please try again.';
        }
      });
      return;
    }

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
    this.infoMessage = '';
    this.isSubmitting = false;
  }
}
