import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class DashboardService {
    private baseUrl:string = 'http://localhost:5000/api/Home/dashboard';
    constructor(private http: HttpClient){}

    getGreetMessage(): Observable<string> {
        return this.http.get(this.baseUrl, { responseType: 'text' });
    }
}