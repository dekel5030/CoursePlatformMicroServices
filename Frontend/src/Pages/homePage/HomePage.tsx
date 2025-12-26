import { useMemo, useState } from "react";
import { CourseCard, useCourses } from "@/features/courses";
import { useAuth } from "react-oidc-context";
import { Button, Skeleton } from "@/components/ui";

export default function HomePage() {
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  const auth = useAuth();
  const { data: courses = [], isLoading, error } = useCourses();

  const categories = useMemo(() => {
    const set = new Set<string>();
    courses.forEach((c) => c.title && set.add(c.title));
    return Array.from(set).sort();
  }, [courses]);

  const filtered = selectedCategory
    ? courses.filter((c) => c.title === selectedCategory)
    : courses;

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
      <header className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div className="space-y-2">
          <h1 className="text-4xl font-bold">Learn anything, anywhere</h1>
          <p className="text-lg text-muted-foreground">
            Browse curated courses from local devs or your API.
          </p>
        </div>
        <div>
          {!auth.isAuthenticated && (
            <Button onClick={() => void auth.signinRedirect()} size="lg">
              Log in
            </Button>
          )}
        </div>
      </header>

      <section className="flex flex-wrap gap-2" aria-label="Categories">
        <Button
          variant={!selectedCategory ? "default" : "outline"}
          size="sm"
          onClick={() => setSelectedCategory(null)}
        >
          All
        </Button>
        {categories.map((cat) => (
          <Button
            key={cat}
            variant={selectedCategory === cat ? "default" : "outline"}
            size="sm"
            onClick={() => setSelectedCategory(cat)}
          >
            {cat}
          </Button>
        ))}
      </section>

      <main>
        {isLoading && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[1, 2, 3].map((i) => (
              <Skeleton key={i} className="h-80" />
            ))}
          </div>
        )}
        
        {error && (
          <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
            Error: {error.message}
          </div>
        )}

        {!isLoading && !error && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filtered.map((course) => (
              <CourseCard key={course.id.value} course={course} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
