import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ConfigService {
  get baseUrl(): string {
    return environment.baseUrl;
  }

  get isProduction(): boolean {
    return environment.production;
  }
}
