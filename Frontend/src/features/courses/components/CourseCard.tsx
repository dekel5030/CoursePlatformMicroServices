import { Link } from "react-router-dom";
import type { Course } from "../../../types/course";
import styles from "./CourseCard.module.css";

interface Props {
  course: Course;
}

export default function CourseCard({ course }: Props) {
  return (
    <Link to={`/courses/${course.id.value}`} className={styles.cardLink}>
      <div className={styles.card}>
        {course.imageUrl && <img src={course.imageUrl} alt={course.title} />}
        <h3>{course.title}</h3>
        {course.description && <p>{course.description}</p>}
        <p>
          Price: {course.price.amount} {course.price.currency}
        </p>
        <p>{course.isPublished ? "Published" : "Draft"}</p>
        <p>Lessons: {course.lessons?.length || 0}</p>
        <p>
          Last updated: {new Date(course.updatedAtUtc).toLocaleDateString()}
        </p>
      </div>
    </Link>
  );
}
