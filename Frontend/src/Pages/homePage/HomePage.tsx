import { useEffect, useMemo, useState } from "react";
import { fetchFeaturedCourses } from "../../services/CoursesAPI";
import type { Course } from "../../types/course";
import CourseCard from "../../features/courses/components/CourseCard";
import { useAuth } from "react-oidc-context";
import styles from "./HomePage.module.css";

export default function HomePage() {
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  const auth = useAuth();
  const token = auth.user?.access_token;

  useEffect(() => {
    setLoading(true);
    fetchFeaturedCourses(token)
      .then(setCourses)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [token]);

  const categories = useMemo(() => {
    const set = new Set<string>();
    courses.forEach((c) => c.title && set.add(c.title));
    return Array.from(set).sort();
  }, [courses]);

  const filtered = selectedCategory
    ? courses.filter((c) => c.title === selectedCategory)
    : courses;

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <div>
          <h1 className={styles.title}>Learn anything, anywhere</h1>
          <p className={styles.subtitle}>
            Browse curated courses from local devs or your API.
          </p>
        </div>
        <div>
          {!auth.isAuthenticated && (
            <button
              className={styles.loginButton}
              onClick={() => void auth.signinRedirect()}
            >
              Log in
            </button>
          )}
        </div>
      </header>

      <section className={styles.categories} aria-label="Categories">
        <button
          className={
            !selectedCategory ? styles.categoryActive : styles.category
          }
          onClick={() => setSelectedCategory(null)}
        >
          All
        </button>
        {categories.map((cat) => (
          <button
            key={cat}
            className={
              selectedCategory === cat ? styles.categoryActive : styles.category
            }
            onClick={() => setSelectedCategory(cat)}
          >
            {cat}
          </button>
        ))}
      </section>

      <main>
        {loading && <div>Loading courses...</div>}
        {error && <div style={{ color: "red" }}>Error: {error}</div>}

        {!loading && !error && (
          <div className={styles.grid}>
            {filtered.map((course) => (
              <CourseCard key={course.id.value} course={course} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
