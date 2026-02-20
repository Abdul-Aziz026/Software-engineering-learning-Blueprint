export interface Lesson {
  id: string;
  lessonName: string;
}

export interface Chapter {
  id: string;
  subjectId: string;
  chapterName: string;
  lessons: Lesson[];
  expanded?: boolean;
}