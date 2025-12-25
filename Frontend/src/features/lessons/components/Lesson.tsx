import { useNavigate } from "react-router-dom";
import type { Lesson as LessonType } from "@/types";
import styles from "./Lesson.module.css";

interface LessonProps {
  lesson: LessonType;
  index: number;
}

export default function Lesson({ lesson, index }: LessonProps) {
  const navigate = useNavigate();
  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;

    const parts = duration.split(":");
    if (parts.length >= 2) {
      const hours = parseInt(parts[0]);
      const minutes = parseInt(parts[1]);

      if (hours > 0) {
        return `${hours}h ${minutes}m`;
      }
      return `${minutes}m`;
    }
    return duration;
  };

  const handleLessonClick = () => {
    navigate(`/lessons/${lesson.id.value}`);
  };

  return (
    <div 
      className={styles.lesson}
      onClick={handleLessonClick}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          handleLessonClick();
        }
      }}
    >
      <div className={styles.lessonHeader}>
        <div className={styles.lessonNumber}>{index + 1}</div>
        <div className={styles.lessonInfo}>
          <h3 className={styles.lessonTitle}>
            {lesson.title}
            {lesson.isPreview && (
              <span className={styles.previewBadge}>Preview</span>
            )}
          </h3>
          {lesson.description && (
            <p className={styles.lessonDescription}>{lesson.description}</p>
          )}
        </div>
      </div>
      <div className={styles.lessonMeta}>
        {lesson.duration && (
          <span className={styles.duration}>
            {formatDuration(lesson.duration)}
          </span>
        )}
      </div>
    </div>
  );
}
