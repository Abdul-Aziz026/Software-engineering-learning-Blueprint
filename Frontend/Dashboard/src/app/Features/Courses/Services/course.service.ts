import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from "../../../Core/Services/config.service";

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private apiUrl: string;

  constructor(private http: HttpClient, private configService: ConfigService) {
    this.apiUrl = this.configService.baseUrl + '/course';
  }

  getCourseStructure(courseId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/${courseId}/structure`);
  }

  getCourseById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}