import { Injectable, signal } from '@angular/core';

export type AuthMode = 'login' | 'signup' | 'forgot';

@Injectable({ providedIn: 'root' })
export class AuthModalService {
  readonly isOpen = signal(false);
  readonly mode = signal<AuthMode>('login');

  open(mode: AuthMode = 'login'): void {
    this.mode.set(mode);
    this.isOpen.set(true);
  }

  close(): void {
    this.isOpen.set(false);
  }

  switchMode(mode: AuthMode): void {
    this.mode.set(mode);
  }
}
