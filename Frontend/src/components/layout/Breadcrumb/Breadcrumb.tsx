import { Link, useLocation } from "react-router-dom";

export interface BreadcrumbItem {
  label: string;
  path?: string;
}

interface BreadcrumbProps {
  items?: BreadcrumbItem[];
}

export default function Breadcrumb({ items }: BreadcrumbProps) {
  const location = useLocation();

  // If no items are provided, auto-generate from URL path
  const breadcrumbItems = items || generateBreadcrumbsFromPath(location.pathname);

  if (breadcrumbItems.length === 0) return null;

  return (
    <nav className="bg-background border-b border-border py-3 px-8 text-sm" aria-label="Breadcrumb">
      <ol className="flex flex-wrap items-center gap-2 list-none max-w-7xl mx-auto">
        {breadcrumbItems.map((item, index) => {
          const isLast = index === breadcrumbItems.length - 1;
          
          return (
            <li key={index} className="flex items-center gap-2">
              {!isLast && item.path ? (
                <>
                  <Link to={item.path} className="text-primary font-medium transition-colors hover:text-primary/80 hover:underline">
                    {item.label}
                  </Link>
                  <span className="text-muted-foreground select-none" aria-hidden="true">
                    &gt;
                  </span>
                </>
              ) : (
                <span className="text-foreground font-semibold" aria-current={isLast ? "page" : undefined}>
                  {item.label}
                </span>
              )}
            </li>
          );
        })}
      </ol>
    </nav>
  );
}

function generateBreadcrumbsFromPath(pathname: string): BreadcrumbItem[] {
  const items: BreadcrumbItem[] = [{ label: "Home", path: "/" }];

  const pathSegments = pathname.split("/").filter(Boolean);

  // Map path segments to readable labels
  const pathMapping: Record<string, string> = {
    courses: "Courses",
    lessons: "Lessons",
    course: "Course",
    lesson: "Lesson",
  };

  let currentPath = "";
  pathSegments.forEach((segment, index) => {
    currentPath += `/${segment}`;
    
    // Skip if it's a GUID (lesson or course ID)
    const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(segment);
    
    if (!isGuid) {
      const label = pathMapping[segment] || capitalizeFirst(segment);
      items.push({
        label,
        path: index < pathSegments.length - 1 ? currentPath : undefined,
      });
    }
  });

  return items;
}

function capitalizeFirst(str: string): string {
  return str.charAt(0).toUpperCase() + str.slice(1);
}
