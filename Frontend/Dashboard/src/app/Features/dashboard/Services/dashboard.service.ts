import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ConfigService } from "../../../Core/Services/config.service";

export interface GreetMessage {
  message: string;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly baseUrl: string;

  constructor(private http: HttpClient, private configService: ConfigService) {
    this.baseUrl = this.configService.baseUrl + '/Home/dashboard';
  }

  getGreetMessage(): Observable<GreetMessage> {
    return this.http.get<GreetMessage>(this.baseUrl);
  }
}
