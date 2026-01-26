import {
  useState,
  useRef,
  useEffect,
  forwardRef,
  useImperativeHandle,
} from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Edit2, Check, X } from "lucide-react";
import { cn } from "@/utils/utils";

interface InlineEditableTextProps {
  value: string;
  onSave: (newValue: string) => Promise<void>;
  className?: string;
  inputClassName?: string;
  displayClassName?: string;
  placeholder?: string;
  canEdit?: boolean;
  maxLength?: number;
}

export interface InlineEditableTextHandle {
  enterEditMode: () => void;
}

export const InlineEditableText = forwardRef<
  InlineEditableTextHandle,
  InlineEditableTextProps
>(function InlineEditableText(
  {
    value,
    onSave,
    className,
    inputClassName,
    displayClassName,
    placeholder = "Enter text...",
    canEdit = true,
    maxLength,
  },
  ref,
) {
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState(value);
  const [isSaving, setIsSaving] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    setEditValue(value);
  }, [value]);

  useEffect(() => {
    if (isEditing && inputRef.current) {
      inputRef.current.focus();
      inputRef.current.select();
    }
  }, [isEditing]);

  useImperativeHandle(ref, () => ({
    enterEditMode: () => setIsEditing(true),
  }));

  const handleSave = async () => {
    // Don't save if value hasn't changed
    if (editValue === value) {
      setIsEditing(false);
      return;
    }

    // Validate non-empty after trimming
    const trimmedValue = editValue.trim();
    if (trimmedValue === "") {
      setIsEditing(false);
      setEditValue(value); // Revert to original
      return;
    }

    setIsSaving(true);
    try {
      await onSave(trimmedValue);
      setIsEditing(false);
    } catch {
      // Error is handled by the caller (toast notification)
      setEditValue(value); // Revert on error
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    setEditValue(value);
    setIsEditing(false);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      e.stopPropagation();
      handleSave();
    } else if (e.key === "Escape") {
      e.preventDefault();
      e.stopPropagation();
      handleCancel();
    }
  };

  if (!canEdit) {
    return <span className={cn(displayClassName)}>{value || placeholder}</span>;
  }

  if (!isEditing) {
    return (
      <div
        className={cn("group flex items-center gap-2", className)}
        onClick={(e) => e.stopPropagation()}
      >
        <span className={cn("flex-1", displayClassName)} dir="auto">
          {value || placeholder}
        </span>
        <Button
          variant="ghost"
          size="sm"
          className="h-8 w-8 p-0 opacity-0 group-hover:opacity-100 transition-opacity"
          onClick={(e) => {
            e.stopPropagation();
            setIsEditing(true);
          }}
          title="Edit"
        >
          <Edit2 className="h-4 w-4" />
        </Button>
      </div>
    );
  }

  return (
    <div
      className={cn("flex items-center gap-2", className)}
      onClick={(e) => e.stopPropagation()}
    >
      <Input
        ref={inputRef}
        value={editValue}
        onChange={(e) => setEditValue(e.target.value)}
        onKeyDown={handleKeyDown}
        onBlur={handleSave}
        className={cn(inputClassName)}
        placeholder={placeholder}
        disabled={isSaving}
        maxLength={maxLength}
      />
      <Button
        variant="ghost"
        size="sm"
        className="h-8 w-8 p-0 text-green-600 hover:text-green-700"
        onClick={(e) => {
          e.stopPropagation();
          handleSave();
        }}
        disabled={isSaving}
        title="Save"
      >
        <Check className="h-4 w-4" />
      </Button>
      <Button
        variant="ghost"
        size="sm"
        className="h-8 w-8 p-0 text-red-600 hover:text-red-700"
        onClick={(e) => {
          e.stopPropagation();
          handleCancel();
        }}
        disabled={isSaving}
        title="Cancel"
      >
        <X className="h-4 w-4" />
      </Button>
    </div>
  );
});
