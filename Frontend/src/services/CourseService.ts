import { BaseService } from './BaseService';
import type { Course, Lesson } from '@/types';

class CourseServiceClass extends BaseService {
  async getFeaturedCourses(): Promise<Course[]> {
    const data = await this.get<Course[] | { items: Course[] }>('/courses/featured');
    return Array.isArray(data) ? data : data.items;
  }

  async getCourseById(courseId: string): Promise<Course> {
    return await this.get<Course>(`/courses/${courseId}`);
  }

  async getLessonById(lessonId: string): Promise<Lesson> {
    return await this.get<Lesson>(`/lessons/${lessonId}`);
  }
}

export const CourseService = new CourseServiceClass();
