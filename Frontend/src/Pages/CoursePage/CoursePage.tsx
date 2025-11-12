import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { fetchCourseById } from "../../services/api";
import type { Course } from "../../types/course";
import Lesson from "../../components/Lesson/Lesson";
import Button from "../../components/Button/Button";
import styles from "./CoursePage.module.css";

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const [course, setCourse] = useState<Course | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setError("Course ID is required");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    fetchCourseById(id)
      .then((data) => setCourse(data))
      .catch((err) => setError(err.message || "Failed to load course"))
      .finally(() => setLoading(false));
  }, [id]);

  const handleBuy = () => {
    // TODO: Implement buy functionality
    console.log("Buy course:", course?.id);
    alert(`Purchasing course: ${course?.title}`);
  };

  const handleAddToCart = () => {
    // TODO: Implement add to cart functionality
    console.log("Add to cart:", course?.id);
    alert(`Added to cart: ${course?.title}`);
  };

  if (loading) {
    return (
      <div className={styles.container}>
        <div className={styles.loading}>Loading course...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <h2>Error</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  if (!course) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <h2>Course not found</h2>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.courseHeader}>
        {course.imageUrl && (
          <div className={styles.imageContainer}>
            <img
              src={course.imageUrl}
              alt={course.title}
              className={styles.courseImage}
            />
          </div>
        )}
        
        <div className={styles.courseInfo}>
          <h1 className={styles.courseTitle}>{course.title}</h1>
          
          {course.instructorUserId && (
            <p className={styles.instructor}>
              Instructor: {course.instructorUserId}
            </p>
          )}
          
          <div className={styles.priceAndActions}>
            <div className={styles.price}>
              <span className={styles.amount}>
                {course.price.amount} {course.price.currency}
              </span>
            </div>
            
            <div className={styles.actions}>
              <Button variant="filled" onClick={handleBuy}>
                Buy Now
              </Button>
              <Button variant="outlined" onClick={handleAddToCart}>
                Add to Cart
              </Button>
            </div>
          </div>
        </div>
      </div>

      <div className={styles.courseDescription}>
        <h2 className={styles.sectionTitle}>About this course</h2>
        <p className={styles.description}>{course.description}</p>
        
        <div className={styles.metadata}>
          <div className={styles.metadataItem}>
            <span className={styles.metadataLabel}>Status:</span>
            <span className={course.isPublished ? styles.published : styles.draft}>
              {course.isPublished ? "Published" : "Draft"}
            </span>
          </div>
          <div className={styles.metadataItem}>
            <span className={styles.metadataLabel}>Last updated:</span>
            <span>{new Date(course.updatedAtUtc).toLocaleDateString()}</span>
          </div>
          {course.lessons && (
            <div className={styles.metadataItem}>
              <span className={styles.metadataLabel}>Lessons:</span>
              <span>{course.lessons.length}</span>
            </div>
          )}
        </div>
      </div>

      {course.lessons && course.lessons.length > 0 && (
        <div className={styles.lessonsSection}>
          <h2 className={styles.sectionTitle}>Course Content</h2>
          <div className={styles.lessonsList}>
            {course.lessons.map((lesson, index) => (
              <Lesson key={lesson.id} lesson={lesson} index={index} />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
