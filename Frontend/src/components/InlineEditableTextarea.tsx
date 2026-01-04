import { useState, useRef, useEffect } from "react";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Edit2, Check, X } from "lucide-react";
import { cn } from "@/utils/utils";

interface InlineEditableTextareaProps {
  value: string;
  onSave: (newValue: string) => Promise<void>;
  className?: string;
  textareaClassName?: string;
  displayClassName?: string;
  placeholder?: string;
  canEdit?: boolean;
  rows?: number;
  maxLength?: number;
}

export function InlineEditableTextarea({
  value,
  onSave,
  className,
  textareaClassName,
  displayClassName,
  placeholder = "Enter description...",
  canEdit = true,
  rows = 3,
  maxLength,
}: InlineEditableTextareaProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState(value);
  const [isSaving, setIsSaving] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    setEditValue(value);
  }, [value]);

  useEffect(() => {
    if (isEditing && textareaRef.current) {
      textareaRef.current.focus();
      textareaRef.current.select();
    }
  }, [isEditing]);

  const handleSave = async () => {
    // Don't save if value hasn't changed (exact comparison to preserve whitespace)
    if (editValue === value) {
      setIsEditing(false);
      return;
    }

    setIsSaving(true);
    try {
      // Save with trimmed value (backend requirement)
      await onSave(editValue.trim());
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

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Escape") {
      e.preventDefault();
      e.stopPropagation();
      handleCancel();
    }
    // Allow Ctrl+Enter or Cmd+Enter to save
    if ((e.ctrlKey || e.metaKey) && e.key === "Enter") {
      e.preventDefault();
      e.stopPropagation();
      handleSave();
    }
  };

  if (!canEdit) {
    return <p className={cn(displayClassName)}>{value || placeholder}</p>;
  }

  if (!isEditing) {
    return (
      <div className={cn("group", className)}>
        <div className="flex items-start justify-between gap-2">
          <p className={cn("flex-1 whitespace-pre-wrap", displayClassName)} dir="auto">
            {value || placeholder}
          </p>
          <Button
            variant="ghost"
            size="sm"
            className="h-8 w-8 p-0 opacity-0 group-hover:opacity-100 transition-opacity shrink-0"
            onClick={(e) => {
              e.stopPropagation();
              setIsEditing(true);
            }}
            title="Edit"
          >
            <Edit2 className="h-4 w-4" />
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className={cn("space-y-2", className)}>
      <Textarea
        ref={textareaRef}
        value={editValue}
        onChange={(e) => setEditValue(e.target.value)}
        onKeyDown={handleKeyDown}
        className={cn(textareaClassName)}
        placeholder={placeholder}
        disabled={isSaving}
        rows={rows}
        maxLength={maxLength}
      />
      <div className="flex items-center gap-2">
        <Button
          variant="default"
          size="sm"
          onClick={(e) => {
            e.stopPropagation();
            handleSave();
          }}
          disabled={isSaving}
          className="gap-2"
        >
          <Check className="h-4 w-4" />
          Save
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={(e) => {
            e.stopPropagation();
            handleCancel();
          }}
          disabled={isSaving}
          className="gap-2"
        >
          <X className="h-4 w-4" />
          Cancel
        </Button>
        <span className="text-xs text-muted-foreground ml-auto">
          Ctrl+Enter to save • Esc to cancel
        </span>
      </div>
    </div>
  );
}
