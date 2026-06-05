import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { ConfigService } from '../../../Core/Services/config.service';
import { AuthResponse, LoginRequest, SignupRequest, UpdateProfileRequest } from '../Models/auth.model';

const STORAGE_KEY = 'auth_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl: string;
  readonly currentUser = signal<AuthResponse | null>(AuthService.loadFromStorage());

  constructor(private http: HttpClient, private configService: ConfigService) {
    this.apiUrl = this.configService.baseUrl + '/auth';
  }

  signup(payload: SignupRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/signup`, payload).pipe(
      tap((user) => this.persistUser(user))
    );
  }

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap((user) => this.persistUser(user))
    );
  }

  getProfile(userId: string): Observable<AuthResponse> {
    return this.http.get<AuthResponse>(`${this.apiUrl}/users/${userId}`);
  }

  updateProfile(userId: string, payload: UpdateProfileRequest): Observable<AuthResponse> {
    return this.http.put<AuthResponse>(`${this.apiUrl}/users/${userId}`, payload).pipe(
      tap((user) => this.persistUser(user))
    );
  }

  logout(): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem(STORAGE_KEY);
    }
    this.currentUser.set(null);
  }

  private persistUser(user: AuthResponse): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    }
    this.currentUser.set(user);
  }

  private static loadFromStorage(): AuthResponse | null {
    if (typeof localStorage === 'undefined') return null;
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AuthResponse;
    } catch {
      return null;
    }
  }
}
