import type { Course } from "../../types/course";
import styles from "./CourseCard.module.css";

interface Props {
  course: Course;
}

export default function CourseCard({ course }: Props) {
  return (
    <div className={styles.card}>
      {course.imageUrl && (
        <div className={styles.imageWrapper}>
          <img src={course.imageUrl} alt={course.title} />
        </div>
      )}

      <div className={styles.content}>
        <h3 className={styles.title}>{course.title}</h3>
        <p className={styles.description}>{course.description}</p>

        <div className={styles.meta}>
          <span className={styles.price}>
            💰 {course.price.amount} {course.price.currency}
          </span>
          <span
            className={`${styles.status} ${
              course.isPublished ? styles.published : styles.draft
            }`}
          >
            {course.isPublished ? "Published" : "Draft"}
          </span>
        </div>

        <div className={styles.details}>
          <span>📚 {course.lessons?.length || 0} lessons</span>
          <span>
            🕒 Updated: {new Date(course.updatedAtUtc).toLocaleDateString()}
          </span>
        </div>
      </div>
    </div>
  );
}
