import * as React from "react";
import { Check } from "lucide-react";
import { cn } from "@/utils/utils";

export interface CheckboxProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {}

const Checkbox = React.forwardRef<HTMLInputElement, CheckboxProps>(
  ({ className, ...props }, ref) => {
    return (
      <div className="relative inline-flex items-center">
        <input
          type="checkbox"
          ref={ref}
          className={cn(
            "peer h-4 w-4 shrink-0 rounded-sm border border-input bg-background ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer appearance-none",
            "checked:bg-primary checked:border-primary",
            className
          )}
          {...props}
        />
        <Check
          className={cn(
            "absolute left-0 h-4 w-4 text-primary-foreground pointer-events-none opacity-0 peer-checked:opacity-100 transition-opacity",
            "p-0.5"
          )}
        />
      </div>
    );
  }
);
Checkbox.displayName = "Checkbox";

export { Checkbox };
