import type { Course } from "../../types/course";
import "./CourseCard.css";

interface Props {
  course: Course;
}

export default function CourseCard({ course }: Props) {
  return (
    <div className="course-card">
      {course.imageUrl && <img src={course.imageUrl} alt={course.title} />}
      <h3>{course.title}</h3>
      <p>{course.description}</p>
      <p>
        Price: {course.price.amount} {course.price.currency}
      </p>
      <p>{course.isPublished ? "Published" : "Draft"}</p>
      <p>Lessons: {course.lessons?.length || 0}</p>
      <p>Last updated: {new Date(course.updatedAtUtc).toLocaleDateString()}</p>
    </div>
  );
}
