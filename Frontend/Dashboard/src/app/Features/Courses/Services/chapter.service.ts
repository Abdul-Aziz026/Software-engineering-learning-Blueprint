import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Chapter } from "../Models/chapter.model";

@Injectable({
  providedIn: 'root'
})

export class ChapterService {
    private apiUrl = 'http://localhost:5000/api/Chapters';

    constructor(private http: HttpClient) { }

    getAllChapters(): Observable<Chapter[]> {
        return this.http.get<Chapter[]>(this.apiUrl);
    }

    getChaptersBySubject(subjectId: string): Observable<Chapter[]> {
        return this.http.get<Chapter[]>(`${this.apiUrl}/subject/${subjectId}`);
    }

    getChapterById(id: string): Observable<Chapter> {
        return this.http.get<Chapter>(`${this.apiUrl}/${id}`);
    }

    createChapter(chapter: Chapter): Observable<Chapter> {
        return this.http.post<Chapter>(this.apiUrl, chapter);
    }

    updateChapter(id: string, chapter: Chapter): Observable<Chapter> {
        return this.http.put<Chapter>(`${this.apiUrl}/${id}`, chapter);
    }

    deleteChapter(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}