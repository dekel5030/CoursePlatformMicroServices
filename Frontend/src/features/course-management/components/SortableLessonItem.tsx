import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { LessonCard } from "@/features/lesson-viewer";
import type { LessonModel } from "@/domain/lessons";

interface SortableLessonItemProps {
  lesson: LessonModel;
  index: number;
  courseId: string;
  moduleId: string;
}

const LESSON_PREFIX = "lesson-";

export function getLessonId(id: string): string {
  return id.startsWith(LESSON_PREFIX) ? id.slice(LESSON_PREFIX.length) : id;
}

export function lessonSortableId(lessonId: string): string {
  return LESSON_PREFIX + lessonId;
}

export function SortableLessonItem({
  lesson,
  index,
  courseId,
  moduleId,
}: SortableLessonItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: lessonSortableId(lesson.lessonId),
    data: {
      type: "lesson" as const,
      lessonId: lesson.lessonId,
      moduleId,
    },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <div ref={setNodeRef} style={style} className={isDragging ? "opacity-50" : ""}>
      <LessonCard
        lesson={lesson}
        index={index}
        courseId={courseId}
        dragHandleProps={{ ...attributes, ...listeners }}
      />
    </div>
  );
}
