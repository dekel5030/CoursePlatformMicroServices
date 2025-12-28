import type { Course } from "@/types";
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui";
import { Badge } from "@/components/ui";
import { BookOpen, Clock } from "lucide-react";
import { motion } from "framer-motion";
import { Link } from "react-router-dom";

interface Props {
  course: Course;
}

export default function CourseCard({ course }: Props) {
  return (
    <Link to={`/courses/${course.id.value}`}>
      <motion.div
        whileHover={{ y: -5 }}
        transition={{ type: "spring", stiffness: 300 }}
      >
        <Card className="h-full overflow-hidden border border-border/60 hover:border-primary/50 transition-colors bg-card rounded-md group">
          <div className="relative aspect-[16/9] w-full overflow-hidden bg-muted">
            {course.imageUrl ? (
              <img 
                src={course.imageUrl} 
                alt={course.title}
                className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
              />
            ) : (
              <div className="flex items-center justify-center h-full text-muted-foreground bg-muted">
                No Image
              </div>
            )}
            <div className="absolute top-2 right-2">
              <Badge variant="secondary" className="bg-background/80 backdrop-blur-sm shadow-sm border border-border/50 text-foreground font-medium">
                {course.price.amount === 0 ? "Free" : `${course.price.amount} ${course.price.currency}`}
              </Badge>
            </div>
          </div>

          <CardHeader className="p-5 pb-2">
            <CardTitle className="line-clamp-2 text-lg font-bold tracking-tight group-hover:text-primary transition-colors">
              {course.title}
            </CardTitle>
          </CardHeader>

          <CardContent className="p-5 pt-2 pb-4 space-y-4">
            <p className="text-sm text-muted-foreground line-clamp-2 leading-relaxed">
              {course.description}
            </p>
            
            <div className="flex flex-wrap gap-2">
               {!course.isPublished && (
                 <Badge variant="outline" className="text-xs text-yellow-600 border-yellow-200 bg-yellow-50">
                   Draft
                 </Badge>
               )}
            </div>
          </CardContent>

          <CardFooter className="p-5 pt-0 flex items-center justify-between text-xs text-muted-foreground font-medium border-t border-border/40 mt-auto bg-muted/10 h-10">
            <div className="flex items-center gap-1.5">
              <BookOpen className="h-3.5 w-3.5" />
              <span>{course.lessons?.length || 0} Lessons</span>
            </div>
            <div className="flex items-center gap-1.5">
              <Clock className="h-3.5 w-3.5" />
              <span>{new Date(course.updatedAtUtc).toLocaleDateString()}</span>
            </div>
          </CardFooter>
        </Card>
      </motion.div>
    </Link>
  );
}
