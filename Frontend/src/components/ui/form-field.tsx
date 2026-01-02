import * as React from "react";
import { Input } from "./input";
import { cn } from "@/utils/utils";

interface FormFieldProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
  name: string;
}

const FormField = React.forwardRef<HTMLInputElement, FormFieldProps>(
  ({ label, error, name, className, required, ...props }, ref) => {
    return (
      <div className="space-y-2">
        <label
          htmlFor={name}
          className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
        >
          {label}
          {required && <span className="text-destructive ml-1">*</span>}
        </label>
        <Input
          id={name}
          name={name}
          ref={ref}
          className={cn(error && "border-destructive", className)}
          required={required}
          {...props}
        />
        {error && <p className="text-sm text-destructive">{error}</p>}
      </div>
    );
  }
);
FormField.displayName = "FormField";

export { FormField };
