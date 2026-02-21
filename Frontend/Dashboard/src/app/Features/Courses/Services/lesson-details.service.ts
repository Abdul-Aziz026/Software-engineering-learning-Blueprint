import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LessonDetails } from "../Models/lesson-details.model";
import { ConfigService } from "../../../Core/Services/config.service";

@Injectable({
    providedIn: 'root'
})

export class LessonDetailsService {
    private apiUrl: string;

    constructor(private http: HttpClient, private configService: ConfigService) {
        this.apiUrl = this.configService.baseUrl;
    }

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