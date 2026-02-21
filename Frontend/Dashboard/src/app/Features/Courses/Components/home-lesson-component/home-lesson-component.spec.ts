import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeLessonComponent } from './home-lesson-component';

describe('CourseComponent', () => {
  let component: HomeLessonComponent;
  let fixture: ComponentFixture<HomeLessonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeLessonComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomeLessonComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
