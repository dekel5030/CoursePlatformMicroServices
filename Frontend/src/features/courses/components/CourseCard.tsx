import { Link } from "react-router-dom";
import type { Course } from "@/types";
import { Card, CardContent, CardHeader, CardTitle, CardFooter, Badge } from "@/components/ui";
import { BookOpen, Clock, DollarSign } from "lucide-react";

interface Props {
  course: Course;
}

export default function CourseCard({ course }: Props) {
  return (
    <Link to={`/courses/${course.id.value}`} className="block hover:opacity-80 transition-opacity">
      <Card className="overflow-hidden hover:shadow-lg transition-shadow">
        {course.imageUrl && (
          <div className="relative h-48 w-full overflow-hidden">
            <img 
              src={course.imageUrl} 
              alt={course.title}
              className="h-full w-full object-cover"
            />
          </div>
        )}
        
        <CardHeader>
          <CardTitle className="line-clamp-2">{course.title}</CardTitle>
        </CardHeader>

        <CardContent className="space-y-3">
          {course.description && (
            <p className="text-sm text-muted-foreground line-clamp-3">
              {course.description}
            </p>
          )}

          <div className="flex items-center justify-between">
            <div className="flex items-center gap-1 text-sm font-semibold">
              <DollarSign className="h-4 w-4" />
              {course.price.amount} {course.price.currency}
            </div>
            <Badge variant={course.isPublished ? "default" : "secondary"}>
              {course.isPublished ? "Published" : "Draft"}
            </Badge>
          </div>
        </CardContent>

        <CardFooter className="flex items-center justify-between text-xs text-muted-foreground">
          <div className="flex items-center gap-1">
            <BookOpen className="h-3 w-3" />
            {course.lessons?.length || 0} lessons
          </div>
          <div className="flex items-center gap-1">
            <Clock className="h-3 w-3" />
            {new Date(course.updatedAtUtc).toLocaleDateString()}
          </div>
        </CardFooter>
      </Card>
    </Link>
  );
}
