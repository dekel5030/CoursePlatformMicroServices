import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { API } from "../../api/endpoints";
import CourseCard from "../Courses/CourseCard";
import styles from "./Home.module.css";

interface Course {
  id: string;
  title: string;
  description?: string;
  author?: string;
  category?: string;
}

export default function Home() {
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    fetch(`${API.COURSES}/list`)
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
      })
      .then((data) => setCourses(data))
      .catch((err) => setError(String(err)))
      .finally(() => setLoading(false));
  }, []);

  const categories = useMemo(() => {
    const set = new Set<string>();
    courses.forEach((c) => c.category && set.add(c.category));
    return Array.from(set).sort();
  }, [courses]);

  const filtered = selectedCategory
    ? courses.filter((c) => c.category === selectedCategory)
    : courses;

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <div>
          <h1 className={styles.title}>Learn anything, anywhere</h1>
          <p className={styles.subtitle}>Browse curated courses from local dev or your API.</p>
        </div>
        <div>
          <Link to="/login" className={styles.loginButton}>
            Log in
          </Link>
        </div>
      </header>

      <section className={styles.categories} aria-label="Categories">
        <button
          className={!selectedCategory ? styles.categoryActive : styles.category}
          onClick={() => setSelectedCategory(null)}
        >
          All
        </button>
        {categories.map((cat) => (
          <button
            key={cat}
            className={selectedCategory === cat ? styles.categoryActive : styles.category}
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
            {filtered.map((c) => (
              <CourseCard key={c.id} course={c} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
