import { useParams } from "react-router-dom";
import { useLesson } from "@/features/lessons";
import styles from "./LessonPage.module.css";

export default function LessonPage() {
  const { id } = useParams<{ id: string }>();
  const { data: lesson, isLoading, error } = useLesson(id);

  if (isLoading) return <div className={styles.status}>Loading lesson...</div>;
  if (error) return <div className={styles.statusError}>Error: {error.message}</div>;
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
