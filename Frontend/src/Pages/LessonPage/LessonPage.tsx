import { useParams } from "react-router-dom";
import { useLesson } from "@/features/lessons";
import { Card, CardContent, CardHeader, CardTitle, Skeleton } from "@/components/ui";
import { Clock } from "lucide-react";

export default function LessonPage() {
  const { id } = useParams<{ id: string }>();
  const { data: lesson, isLoading, error } = useLesson(id);

  if (isLoading) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Skeleton className="h-96 w-full" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          Error: {error.message}
        </div>
      </div>
    );
  }

  if (!lesson) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          Lesson not found
        </div>
      </div>
    );
  }

  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;
    const parts = duration.split(":");
    return `${parseInt(parts[0])}h ${parseInt(parts[1])}m`;
  };

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
      {lesson.videoUrl && (
        <Card>
          <CardContent className="p-0">
            <video
              className="w-full aspect-video"
              controls
              poster={lesson.thumbnailImage || undefined}
            >
              <source src={lesson.videoUrl} type="video/mp4" />
            </video>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader className="space-y-3">
          <div className="flex items-start justify-between">
            <CardTitle className="text-3xl">{lesson.title}</CardTitle>
            {lesson.duration && (
              <div className="flex items-center gap-1 text-sm text-muted-foreground">
                <Clock className="h-4 w-4" />
                {formatDuration(lesson.duration)}
              </div>
            )}
          </div>
        </CardHeader>

        {lesson.description && (
          <CardContent>
            <div className="space-y-2">
              <h2 className="text-lg font-semibold">Description</h2>
              <p className="text-muted-foreground leading-relaxed">
                {lesson.description}
              </p>
            </div>
          </CardContent>
        )}
      </Card>
    </div>
  );
}
