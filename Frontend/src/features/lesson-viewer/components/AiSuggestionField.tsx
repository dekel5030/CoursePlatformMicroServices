import { Button } from "@/shared/ui";
import { Check, X, Sparkles, Pencil } from "lucide-react";
import { cn } from "@/shared/utils";
import { useState } from "react";
import { Input } from "@/shared/ui";
import { Textarea } from "@/shared/ui";

interface AiSuggestionFieldProps {
  label: string;
  originalValue: string;
  suggestedValue: string;
  onAccept: (value: string) => Promise<void>;
  onReject: () => void;
  type?: "text" | "textarea";
  placeholder?: string;
  maxLength?: number;
  rows?: number;
}

export function AiSuggestionField({
  label,
  originalValue,
  suggestedValue,
  onAccept,
  onReject,
  type = "text",
  placeholder,
  maxLength,
  rows = 5,
}: AiSuggestionFieldProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editedValue, setEditedValue] = useState(suggestedValue);
  const [isAccepting, setIsAccepting] = useState(false);

  const handleAccept = async () => {
    setIsAccepting(true);
    try {
      await onAccept(isEditing ? editedValue : suggestedValue);
    } finally {
      setIsAccepting(false);
    }
  };

  const handleEdit = () => {
    setIsEditing(true);
    setEditedValue(suggestedValue);
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setEditedValue(suggestedValue);
  };

  return (
    <div className="border-l-4 border-purple-500 bg-purple-50/50 dark:bg-purple-950/20 rounded-r-lg p-4 space-y-3">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Sparkles className="h-4 w-4 text-purple-600 dark:text-purple-400" />
          <span className="text-sm font-medium text-gray-700 dark:text-gray-300">
            AI suggested {label.toLowerCase()}
          </span>
        </div>
        <div className="flex items-center gap-2">
          {!isEditing && (
            <Button
              onClick={handleEdit}
              variant="ghost"
              size="sm"
              className="h-8 px-3 gap-1.5"
            >
              <Pencil className="h-3.5 w-3.5" />
              Edit
            </Button>
          )}
          <Button
            onClick={handleAccept}
            disabled={isAccepting}
            size="sm"
            className="h-8 px-3 gap-1.5 bg-green-600 hover:bg-green-700"
          >
            <Check className="h-3.5 w-3.5" />
            Accept
          </Button>
          <Button
            onClick={onReject}
            disabled={isAccepting}
            variant="outline"
            size="sm"
            className="h-8 px-3 gap-1.5"
          >
            <X className="h-3.5 w-3.5" />
            Reject
          </Button>
        </div>
      </div>

      {/* Content */}
      {isEditing ? (
        <div className="space-y-2">
          {type === "textarea" ? (
            <Textarea
              value={editedValue}
              onChange={(e) => setEditedValue(e.target.value)}
              placeholder={placeholder}
              rows={rows}
              maxLength={maxLength}
              className="w-full bg-white dark:bg-gray-900 dark:text-gray-100 border-2 border-purple-300 dark:border-purple-700 focus:border-purple-500 dark:focus:border-purple-500"
              autoFocus
            />
          ) : (
            <Input
              value={editedValue}
              onChange={(e) => setEditedValue(e.target.value)}
              placeholder={placeholder}
              maxLength={maxLength}
              className="w-full bg-white dark:bg-gray-900 dark:text-gray-100 border-2 border-purple-300 dark:border-purple-700 focus:border-purple-500 dark:focus:border-purple-500 text-lg"
              autoFocus
            />
          )}
          <div className="flex gap-2">
            <Button
              onClick={() => setIsEditing(false)}
              size="sm"
              className="gap-1.5"
            >
              <Check className="h-3.5 w-3.5" />
              Done
            </Button>
            <Button
              onClick={handleCancelEdit}
              variant="outline"
              size="sm"
              className="gap-1.5"
            >
              <X className="h-3.5 w-3.5" />
              Cancel
            </Button>
          </div>
        </div>
      ) : (
        <div className="space-y-3">
          {/* Original value if exists */}
          {originalValue && (
            <div className="text-xs text-gray-500 dark:text-gray-400">
              <span className="font-medium">Current: </span>
              <span className="line-through">{originalValue}</span>
            </div>
          )}

          {/* AI Suggestion */}
          <div className="bg-white dark:bg-gray-900 rounded-lg border border-purple-200 dark:border-purple-800 p-3">
            <p
              className={cn(
                "text-gray-900 dark:text-gray-100 whitespace-pre-wrap leading-relaxed",
                type === "text" && "text-lg font-semibold",
              )}
              dir="auto"
            >
              {editedValue}
            </p>
          </div>
        </div>
      )}
    </div>
  );
}
