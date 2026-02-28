import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from '../../Models/subject.model';
import { SubjectService } from '../../Services/subject.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-subjects-component',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './subjects-component.html',
  styleUrl: './subjects-component.scss',
})
export class SubjectsComponent implements OnInit {
  subjects: Subject[] = [];
  selectedSubject: Subject | null = null;
  isEditing = false;
  isCreating = false;
  editingSubjectId: string | null = null;

  newSubject: Subject = {
    id: '',
    name: '',
    description: ''
  };

  constructor(private subjectService: SubjectService,
    private cdr: ChangeDetectorRef,
    private router: Router) { }


  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects(): void {
    this.subjectService.getAllSubjects().subscribe({
      next: (data) => {
        this.subjects = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading subjects:', err);
      }
    })
  }

  startCreate(): void {
    this.isCreating = true;
    this.isEditing = false;
    this.editingSubjectId = null;
    this.newSubject = { id: '', name: '', description: '' };
    this.selectedSubject = null;
  }

  cancelCreate() {
    this.isCreating = false;
    this.newSubject = { id: '', name: '', description: '' };
  }

  createSubject(): void {
    this.subjectService.createSubject(this.newSubject).subscribe({
      next: () => {
        this.loadSubjects();
        this.isCreating = false;
        this.newSubject = { id: '', name: '', description: '' };
      },
      error: (err) => {
        console.error('Error creating subject:', err);
      }
    });
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.editingSubjectId = null;
    this.newSubject = { id: '', name: '', description: '' };
  }

  editSubject(subject: Subject): void {
    this.isEditing = true;
    this.isCreating = false;
    this.editingSubjectId = subject.id;
    this.newSubject = { ...subject };
  }

  updateSubject(): void {
    if (!this.editingSubjectId) return;
    this.subjectService.updateSubject(this.editingSubjectId, this.newSubject).subscribe({
      next: () => {
        this.loadSubjects();
        this.isEditing = false;
        this.editingSubjectId = null;
        this.newSubject = { id: '', name: '', description: '' };
      },
      error: (err) => {
        console.error('Error updating subject:', err);
      }
    });
  }

  deleteSubject(subjectId: string): void {
    if (confirm('Are you sure you want to delete this subject?')) {
      this.subjectService.deleteSubject(subjectId).subscribe({
        next: () => {
          this.loadSubjects();
          this.selectedSubject = null;
        },
        error: (err) => {
          console.error('Error deleting subject:', err);
        }
      });
    }
  }
}
