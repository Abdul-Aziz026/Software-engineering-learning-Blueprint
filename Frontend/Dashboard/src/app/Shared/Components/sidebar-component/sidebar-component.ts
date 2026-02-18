import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
// import { CourseService } from 'src/app/services/course.service';

@Component({
  selector: 'app-sidebar-component',
  templateUrl: './sidebar-component.html',
  styleUrls: ['./sidebar-component.scss']
})
export class SidebarComponent implements OnInit {
  courseData: any;
  courseId: string = '';

  constructor(
    private route: ActivatedRoute,
    // private courseService: CourseService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.courseId = params['id'];
      this.loadCourseData();
    });
  }

  loadCourseData() {
    // this.courseService.getCourseStructure(this.courseId).subscribe(data => {
    //   this.courseData = data;
    // });
  }
}