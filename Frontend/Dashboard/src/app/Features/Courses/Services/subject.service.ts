import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Subject } from "../Models/subject.model";

@Injectable({
  providedIn: 'root'
})

export class SubjectService {
    private apiUrl = 'http://localhost:5000/api/Course';

    constructor(private http: HttpClient) { }

    getAllSubjects(): Observable<Subject[]> {
        return this.http.get<Subject[]>(this.apiUrl);
    }

    getSubjectById(id: string): Observable<Subject> {
        return this.http.get<Subject>(`${this.apiUrl}/${id}`);
    }

    createSubject(subject: Subject): Observable<Subject> {
        return this.http.post<Subject>(this.apiUrl, subject);
    }

    updateSubject(id: string, subject: Subject): Observable<Subject> {
        return this.http.put<Subject>(`${this.apiUrl}/${id}`, subject);
    }

    deleteSubject(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

}