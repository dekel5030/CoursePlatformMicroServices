import { useParams } from "react-router-dom";
import { Lesson } from "@/features/lessons";
import { useCourse } from "@/features/courses";
import { Button, Card, CardContent, CardHeader, CardTitle, Skeleton } from "@/components/ui";
import Breadcrumb from "@/components/layout/Breadcrumb/Breadcrumb";
import { ShoppingCart, CreditCard } from "lucide-react";
import { useTranslation } from "react-i18next";

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const { data: course, isLoading, error } = useCourse(id);
  const { t } = useTranslation();

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

  if (!course) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          Course not found
        </div>
      </div>
    );
  }

  const breadcrumbItems = [
    { label: t('breadcrumbs.home'), path: '/' },
    { label: t('breadcrumbs.courses'), path: '/catalog' },
    { label: course.title },
  ];

  return (
    <div className="space-y-6">
      <Breadcrumb items={breadcrumbItems} />
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
        <Card className="overflow-hidden">
        <div className="grid md:grid-cols-2 gap-6 p-6">
          {course.imageUrl && (
            <div className="relative h-64 md:h-full overflow-hidden rounded-lg">
              <img
                src={course.imageUrl}
                alt={course.title}
                className="h-full w-full object-cover"
              />
            </div>
          )}
          <div className="space-y-4">
            <div className="space-y-2">
              <h1 className="text-3xl font-bold">{course.title}</h1>
              <p className="text-muted-foreground">
                Instructor: {course.instructorUserId ?? "Unknown"}
              </p>
            </div>
            <div className="flex gap-3">
              <Button className="flex-1 gap-2">
                <CreditCard className="h-4 w-4" />
                Buy Now
              </Button>
              <Button variant="outline" className="flex-1 gap-2">
                <ShoppingCart className="h-4 w-4" />
                Add to Cart
              </Button>
            </div>
          </div>
        </div>
      </Card>

      {course.description && (
        <Card>
          <CardHeader>
            <CardTitle>About This Course</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-muted-foreground">{course.description}</p>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader>
          <CardTitle>Lessons</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {course.lessons && course.lessons.length > 0 ? (
            course.lessons
              .sort((a, b) => a.order - b.order)
              .map((lesson, index) => (
                <Lesson key={lesson.id.value} lesson={lesson} index={index} />
              ))
          ) : (
            <p className="text-muted-foreground text-center py-8">
              No lessons available.
            </p>
          )}
        </CardContent>
      </Card>
      </div>
    </div>
  );
}
