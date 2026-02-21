import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
// import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CourseService {

  private apiUrl = 'http://localhost:5000' + '/api/course';

  constructor(private http: HttpClient) {}

  getCourseStructure(courseId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/${courseId}/structure`);
  }

  getCourseById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}