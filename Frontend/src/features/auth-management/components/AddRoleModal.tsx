import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui";
import { Button, FormField } from "@/components/ui";
import type { ApiErrorResponse } from "@/api/axiosClient";

interface AddRoleModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (roleName: string) => Promise<void>;
  isLoading?: boolean;
}

export default function AddRoleModal({
  open,
  onOpenChange,
  onSubmit,
  isLoading = false,
}: AddRoleModalProps) {
  const [roleName, setRoleName] = useState("");
  const [apiError, setApiError] = useState<ApiErrorResponse | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setApiError(null);

    try {
      await onSubmit(roleName);
      setRoleName("");
      onOpenChange(false);
    } catch (err: unknown) {
      setApiError(err as ApiErrorResponse);
    }
  };

  const handleClose = () => {
    setRoleName("");
    setApiError(null);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add Role</DialogTitle>
        </DialogHeader>

        {apiError?.message && (
          <div className="bg-destructive/15 text-destructive px-4 py-2 rounded-md text-sm">
            {apiError.message}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <FormField
            label="Role Name"
            name="roleName"
            value={roleName}
            onChange={(e) => setRoleName(e.target.value)}
            placeholder="e.g., Admin, Instructor, Student"
            error={apiError?.errors?.RoleName?.[0]}
            required
          />

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? "Adding..." : "Add Role"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
