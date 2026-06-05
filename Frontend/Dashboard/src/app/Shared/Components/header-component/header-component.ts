import { AfterViewInit, Component, ElementRef, HostListener, OnDestroy, ViewChild } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { Subject } from '../../../Features/Courses/Models/subject.model';
import { selectAllSubjects } from '../../../Core/Store/selectors/subject.selectors';
import { AuthService } from '../../../Features/Auth/Services/auth.service';
import { AuthModalService, AuthMode } from '../../../Features/Auth/Services/auth-modal.service';

@Component({
  selector: 'app-header-component',
  standalone: true,
  imports: [RouterLink, AsyncPipe],
  templateUrl: './header-component.html',
  styleUrl: './header-component.scss',
})
export class HeaderComponent implements AfterViewInit, OnDestroy {
  @ViewChild('navScroll') navScroll?: ElementRef<HTMLElement>;
  @ViewChild('profileMenu') profileMenu?: ElementRef<HTMLElement>;

  readonly subjects$: Observable<Subject[]>;

  canScrollLeft = false;
  canScrollRight = false;
  profileMenuOpen = false;

  private resizeObserver?: ResizeObserver;

  constructor(
    private store: Store,
    private router: Router,
    public authService: AuthService,
    private authModal: AuthModalService
  ) {
    this.subjects$ = this.store.select(selectAllSubjects);
  }

  ngAfterViewInit(): void {
    queueMicrotask(() => this.updateScrollState());

    if (this.navScroll?.nativeElement && typeof ResizeObserver !== 'undefined') {
      this.resizeObserver = new ResizeObserver(() => this.updateScrollState());
      this.resizeObserver.observe(this.navScroll.nativeElement);
    }
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
  }

  onScroll(): void {
    this.updateScrollState();
  }

  scrollPrev(): void {
    this.scrollByStep(-1);
  }

  scrollNext(): void {
    this.scrollByStep(1);
  }

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

  private scrollByStep(direction: 1 | -1): void {
    const el = this.navScroll?.nativeElement;
    if (!el) return;

    const firstItem = el.querySelector<HTMLElement>('.tech-link');
    const gap = parseFloat(getComputedStyle(el).columnGap || getComputedStyle(el).gap || '0') || 0;
    const step = firstItem ? firstItem.offsetWidth + gap : el.clientWidth * 0.6;

    el.scrollBy({ left: direction * step, behavior: 'smooth' });
  }

  private updateScrollState(): void {
    const el = this.navScroll?.nativeElement;
    if (!el) return;

    const max = el.scrollWidth - el.clientWidth;
    this.canScrollLeft = el.scrollLeft > 1;
    this.canScrollRight = el.scrollLeft < max - 1;
  }
}
