import { cn } from "@/utils/utils";
import type { ReactNode } from "react";

interface PageSectionProps extends React.HTMLAttributes<HTMLElement> {
  children: ReactNode;
  containerClassName?: string;
  fullWidth?: boolean;
}

export function PageSection({
  children,
  className,
  containerClassName,
  fullWidth = false,
  ...props
}: PageSectionProps) {
  return (
    <section className={cn("py-24", className)} {...props}>
      <div
        className={cn(
          "container px-4 md:px-6 mx-auto",
          fullWidth ? "max-w-none" : "",
          containerClassName
        )}
      >
        {children}
      </div>
    </section>
  );
}
