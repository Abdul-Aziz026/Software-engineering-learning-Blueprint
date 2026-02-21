import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LessonDetails } from "../Models/lesson-details.model";

@Injectable ({
    providedIn: 'root'
})

export class LessonDetailsService {
    private apiUrl = 'http://localhost:5000/api';

    constructor(private http: HttpClient) { }

    getLessonDetailsByLessonId(subjectId: string, lessonId: string): Observable<LessonDetails> {
        return this.http.get<LessonDetails>(`${this.apiUrl}/LessonDetails/${subjectId}/lesson/${lessonId}`);
    }

    getLessonDetailsById(id: string): Observable<LessonDetails> {
        return this.http.get<LessonDetails>(`${this.apiUrl}/${id}`);
    }

    createLessonDetails(lessonDetails: LessonDetails): Observable<LessonDetails> {
        return this.http.post<LessonDetails>(this.apiUrl, lessonDetails);
    }

    updateLessonDetails(id: string, lessonDetails: LessonDetails): Observable<LessonDetails> {
        return this.http.put<LessonDetails>(`${this.apiUrl}/${id}`, lessonDetails);
    }

    deleteLessonDetails(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}