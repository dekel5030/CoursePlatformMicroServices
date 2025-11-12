import { useEffect, useState } from "react";
import { fetchFeaturedCourses } from "../../services/api";
import type { Course } from "../../types/course";
import CourseCard from "../../components/CourseCard/CourseCard";
import styles from "./Home.module.css";

export default function Home() {
  const [courses, setCourses] = useState<Course[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchFeaturedCourses()
      .then(setCourses)
      .catch((err) => setError(err.message));
  }, []);

  return (
    <div className={styles.container}>
      <h2 className={styles.heading}>Available Courses</h2>
      {error && <p className={styles.error}>{error}</p>}
      <div className={styles.grid}>
        {courses.map((course) => (
          <CourseCard key={course.id.value} course={course} />
        ))}
      </div>
    </div>
  );
}
