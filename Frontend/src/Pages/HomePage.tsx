import { useEffect, useState } from "react";
import { fetchFeaturedCourses } from "../services/api";
import type { Course } from "../types/course";
import CourseCard from "../components/CourseCard/CourseCard";
import Navbar from "../components/NavBar/Navbar";

export default function HomePage() {
  const [courses, setCourses] = useState<Course[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchFeaturedCourses()
      .then(setCourses)
      .catch((err) => setError(err.message));
  }, []);

  return (
    <>
      <Navbar />
      <main style={{ padding: "20px 40px" }}>
        <h2>Available Courses</h2>
        {error && <p style={{ color: "red" }}>{error}</p>}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))",
            gap: "20px",
            marginTop: "20px",
          }}
        >
          {courses.map((course) => (
            <CourseCard key={course.id.value} course={course} />
          ))}
        </div>
      </main>
    </>
  );
}
