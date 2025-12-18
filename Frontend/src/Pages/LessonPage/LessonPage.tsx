import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { Lesson } from "../../types/Lesson";
import { fetchLessonById } from "../../services/CoursesAPI";
import { useAuthenticatedFetch } from "../../utils/useAuthenticatedFetch";
import styles from "./LessonPage.module.css";

export default function LessonPage() {
  const { id } = useParams<{ id: string }>();
  const [lesson, setLesson] = useState<Lesson | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const authFetch = useAuthenticatedFetch();

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    fetchLessonById(id, authFetch)
      .then((data) => setLesson(data))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [id, authFetch]);

  if (loading) return <div className={styles.status}>Loading lesson...</div>;
  if (error) return <div className={styles.statusError}>Error: {error}</div>;
  if (!lesson)
    return <div className={styles.statusError}>Lesson not found</div>;

  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;
    const parts = duration.split(":");
    return `${parseInt(parts[0])}h ${parseInt(parts[1])}m`;
  };

  return (
    <div className={styles.container}>
      {lesson.videoUrl && (
        <div className={styles.videoSection}>
          <video
            className={styles.videoPlayer}
            controls
            poster={lesson.thumbnailImage || undefined}
          >
            <source src={lesson.videoUrl} type="video/mp4" />
          </video>
        </div>
      )}
      <div className={styles.contentSection}>
        <div className={styles.header}>
          <h1 className={styles.title}>{lesson.title}</h1>
          <div className={styles.metadata}>
            {formatDuration(lesson.duration)}
          </div>
        </div>
        {lesson.description && (
          <div className={styles.descriptionSection}>
            <h2 className={styles.sectionTitle}>Description</h2>
            <p className={styles.description}>{lesson.description}</p>
          </div>
        )}
      </div>
    </div>
  );
}
