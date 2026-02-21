import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ConfigService } from "../../../Core/Services/config.service";

@Injectable({
  providedIn: 'root'
})

export class DashboardService {
  private baseUrl: string;
  constructor(private http: HttpClient,
    private configService: ConfigService
  ) {
    this.baseUrl = this.configService.baseUrl + 'Home/dashboard';
  }

  getGreetMessage(): Observable<any> {
    return this.http.get<any>(this.baseUrl);
  }
}


