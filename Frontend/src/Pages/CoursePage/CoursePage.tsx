import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { Course } from "../../types/course";
import Lesson from "../../features/lessons/components/Lesson";
import { fetchCourseById } from "../../services/CoursesAPI";
import styles from "./CoursePage.module.css";

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const [course, setCourse] = useState<Course | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    fetchCourseById(id)
      .then(setCourse)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <div className={styles.status}>Loading course...</div>;
  if (error) return <div className={styles.statusError}>Error: {error}</div>;
  if (!course)
    return <div className={styles.statusError}>Course not found</div>;

  return (
    <div className={styles.container}>
      <div className={styles.top}>
        {course.imageUrl && (
          <img
            src={course.imageUrl}
            alt={course.title}
            className={styles.courseImage}
          />
        )}
        <div className={styles.courseInfo}>
          <h1 className={styles.title}>{course.title}</h1>
          <p className={styles.instructor}>
            Instructor: {course.instructorUserId ?? "Unknown"}
          </p>
          <div className={styles.buttons}>
            <button className={styles.buyButton}>Buy</button>
            <button className={styles.cartButton}>Add to Cart</button>
          </div>
        </div>
      </div>
      <p className={styles.description}>{course.description}</p>
      <section className={styles.lessons}>
        <h2>Lessons</h2>
        {course.lessons && course.lessons.length > 0 ? (
          course.lessons
            .sort((a, b) => a.order - b.order)
            .map((lesson, index) => (
              <Lesson key={lesson.id.value} lesson={lesson} index={index} />
            ))
        ) : (
          <p>No lessons available.</p>
        )}
      </section>
    </div>
  );
}
