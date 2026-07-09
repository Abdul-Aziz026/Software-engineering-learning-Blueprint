import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../Features/Auth/Services/auth.service';
import { AuthModalService, AuthMode } from '../../../Features/Auth/Services/auth-modal.service';

@Component({
  selector: 'app-header-component',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './header-component.html',
  styleUrl: './header-component.scss',
})
export class HeaderComponent {
  @ViewChild('profileMenu') profileMenu?: ElementRef<HTMLElement>;

  profileMenuOpen = false;

  constructor(
    private router: Router,
    public authService: AuthService,
    private authModal: AuthModalService
  ) {}

  openAuthModal(mode: AuthMode): void {
    this.authModal.open(mode);
  }

  toggleProfileMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  closeProfileMenu(): void {
    this.profileMenuOpen = false;
  }

  logout(): void {
    this.profileMenuOpen = false;
    this.authService.logout();
    this.router.navigateByUrl('/');
  }

  get userInitial(): string {
    const u = this.authService.currentUser();
    return u?.username?.charAt(0)?.toUpperCase() ?? '?';
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.profileMenuOpen) return;
    const target = event.target as Node | null;
    const root = this.profileMenu?.nativeElement;
    if (root && target && !root.contains(target)) {
      this.profileMenuOpen = false;
    }
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.profileMenuOpen = false;
  }
}
