import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SubjectService } from '../../Services/subject.service';
import { ChapterService } from '../../Services/chapter.service';
import { LessonDetailsService } from '../../Services/lesson-details.service';
import { Subject } from '../../Models/subject.model';
import { Chapter } from '../../Models/chapter.model';
import { LessonDetails } from '../../Models/lesson-details.model';

@Component({
    selector: 'app-lesson-create-component',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './lesson-create-component.html',
    styleUrl: './lesson-create-component.scss'
})
export class LessonCreateComponent implements OnInit {
    // Form Data
    formData = {
        subjectId: '',
        chapterId: '',
        newChapterName: '',
        lessonId: '',
        lessonName: '',
        title: '',
        description: '',
        referenceUrls: ['']
    };

    subjects: Subject[] = [];
    chapters: Chapter[] = [];
    isNewChapter: boolean = false;

    constructor(
        private subjectService: SubjectService,
        private chapterService: ChapterService,
        private lessonDetailsService: LessonDetailsService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.loadSubjects();

        this.route.queryParams.subscribe(params => {
            if (params['subjectId']) {
                this.formData.subjectId = params['subjectId'];
                this.onSubjectChange();
            }
        });
    }

    loadSubjects() {
        this.subjectService.getAllSubjects().subscribe(subjects => {
            this.subjects = subjects;
        });
    }

    onSubjectChange() {
        if (this.formData.subjectId) {
            this.loadChapters(this.formData.subjectId);
        } else {
            this.chapters = [];
        }
        this.formData.chapterId = '';
        this.isNewChapter = false;
    }

    onChapterChange() {
        this.isNewChapter = this.formData.chapterId === 'new';
    }

    loadChapters(subjectId: string) {
        this.chapterService.getChaptersBySubject(subjectId).subscribe(chapters => {
            this.chapters = chapters;
        });
    }

    addReferenceUrl() {
        this.formData.referenceUrls.push('');
    }

    removeReferenceUrl(index: number) {
        if (this.formData.referenceUrls.length > 1) {
            this.formData.referenceUrls.splice(index, 1);
        }
    }

    onSubmit() {
        if (this.isNewChapter) {
            const newChapter: Chapter = {
                id: '',
                subjectId: this.formData.subjectId,
                chapterName: this.formData.newChapterName,
                lessons: [{ id: this.formData.lessonId, lessonName: this.formData.lessonName }]
            };

            this.chapterService.createChapter(newChapter).subscribe({
                next: () => this.createLessonDetails(),
                error: (err) => console.error('Error creating chapter', err)
            });
        } else {
            const selectedChapter = this.chapters.find(c => c.id === this.formData.chapterId);
            if (selectedChapter) {
                const updatedChapter = { ...selectedChapter };
                updatedChapter.lessons = [...(updatedChapter.lessons || []), { id: this.formData.lessonId, lessonName: this.formData.lessonName }];

                this.chapterService.updateChapter(updatedChapter.id, updatedChapter).subscribe({
                    next: () => this.createLessonDetails(),
                    error: (err) => console.error('Error updating chapter', err)
                });
            }
        }
    }

    createLessonDetails() {
        const lessonDetails: LessonDetails = {
            id: '',
            lessonId: this.formData.lessonId,
            title: this.formData.title,
            description: this.formData.description,
            referenceUrls: this.formData.referenceUrls.filter(url => url.trim() !== '')
        };

        this.lessonDetailsService.createLessonDetails(lessonDetails).subscribe({
            next: () => {
                alert('Lesson created successfully!');
                this.router.navigate(['/course', this.formData.subjectId, 'lesson', this.formData.lessonId]);
            },
            error: (err) => console.error('Error creating lesson details', err)
        });
    }
}
