import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { Lesson } from "../../types/Lesson";
import { fetchLessonById } from "../../services/api";
import styles from "./LessonPage.module.css";

export default function LessonPage() {
  const { id } = useParams<{ id: string }>();
  const [lesson, setLesson] = useState<Lesson | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;

    console.log("Fetching lesson id =", id);

    setLoading(true);
    fetchLessonById(id)
      .then((data) => {
        console.log("Lesson data:", data);
        setLesson(data);
      })
      .catch((err) => {
        console.error(err);
        setError(err.message);
      })
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <div className={styles.status}>Loading lesson...</div>;
  if (error) return <div className={styles.statusError}>Error: {error}</div>;
  if (!lesson)
    return <div className={styles.statusError}>Lesson not found</div>;

  // Format duration for display
  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;
    // Duration comes as a TimeSpan string like "00:30:00"
    const parts = duration.split(":");
    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);

    if (hours > 0) {
      return `${hours}h ${minutes}m`;
    }
    return `${minutes}m`;
  };

  return (
    <div className={styles.container}>
      {/* Video Player Section */}
      {lesson.videoUrl && (
        <div className={styles.videoSection}>
          <video
            className={styles.videoPlayer}
            controls
            poster={lesson.thumbnailImage || undefined}
          >
            <source src={lesson.videoUrl} type="video/mp4" />
            Your browser does not support the video tag.
          </video>
        </div>
      )}

      {/* Lesson Info Section */}
      <div className={styles.contentSection}>
        <div className={styles.header}>
          <h1 className={styles.title}>{lesson.title}</h1>
          <div className={styles.metadata}>
            {lesson.duration && (
              <span className={styles.duration}>
                <svg
                  className={styles.icon}
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
                {formatDuration(lesson.duration)}
              </span>
            )}
            {lesson.isPreview && (
              <span className={styles.previewBadge}>Preview</span>
            )}
          </div>
        </div>

        {/* Description */}
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
