import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { Lesson as LessonType } from "../types/Lesson";
import { Card, CardContent, Badge, Button } from "@/components";
import { Clock, PlayCircle, Edit, Trash2 } from "lucide-react";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";
import { toast } from "sonner";

interface LessonProps {
  lesson: LessonType;
  index: number;
}

export default function Lesson({ lesson, index }: LessonProps) {
  const navigate = useNavigate();
  const { t } = useTranslation(['lessons', 'translation']);

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
    navigate(`/lessons/${lesson.id}`);
  };

  const handleEdit = (e: React.MouseEvent) => {
    e.stopPropagation();
    toast.info(t('lessons:actions.editNotImplemented'));
  };

  const handleDelete = (e: React.MouseEvent) => {
    e.stopPropagation();
    toast.info(t('lessons:actions.deleteNotImplemented'));
  };

  return (
    <Card
      className="cursor-pointer hover:shadow-md transition-shadow"
      onClick={handleLessonClick}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === " ") {
          handleLessonClick();
        }
      }}
    >
      <CardContent className="p-4">
        <div className="flex items-start gap-4">
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary font-semibold">
            {index + 1}
          </div>

          <div className="flex-1 space-y-1 min-w-0">
            <div className="flex items-center gap-2 flex-wrap">
              <h3 className="font-semibold text-base line-clamp-1">
                {lesson.title}
              </h3>
              {lesson.isPreview && (
                <Badge variant="secondary" className="text-xs">
                  Preview
                </Badge>
              )}
            </div>
            {lesson.description && (
              <p className="text-sm text-muted-foreground line-clamp-2">
                {lesson.description}
              </p>
            )}
          </div>

          <div className="flex items-center gap-2 text-sm text-muted-foreground shrink-0">
            {lesson.duration && (
              <div className="flex items-center gap-1">
                <Clock className="h-4 w-4" />
                <span>{formatDuration(lesson.duration)}</span>
              </div>
            )}
            <div className="flex gap-1">
              <Authorized 
                action={ActionType.Update} 
                resource={ResourceType.Lesson}
                resourceId={ResourceId.create(lesson.id)}
              >
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-8 w-8 p-0"
                  onClick={handleEdit}
                  title="Edit lesson"
                >
                  <Edit className="h-4 w-4" />
                </Button>
              </Authorized>
              <Authorized 
                action={ActionType.Delete} 
                resource={ResourceType.Lesson}
                resourceId={ResourceId.create(lesson.id)}
              >
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-8 w-8 p-0 hover:text-destructive"
                  onClick={handleDelete}
                  title="Delete lesson"
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </Authorized>
              <PlayCircle className="h-5 w-5 ml-1" />
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
