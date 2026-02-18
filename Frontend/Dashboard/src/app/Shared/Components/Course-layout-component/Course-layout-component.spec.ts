import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseLayoutComponent } from './Course-layout-component';

describe('LayoutComponent', () => {
  let component: CourseLayoutComponent;
  let fixture: ComponentFixture<CourseLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CourseLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseLayoutComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
