import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  Alert,
  AlertDescription,
} from "@/components";
import { Button, FormField } from "@/components";
import { AlertCircle } from "lucide-react";
import type { ApiErrorResponse } from "@/app/axios";
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation(["auth", "translation"]);
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
          <DialogTitle>{t("auth:modals.addRole.title")}</DialogTitle>
        </DialogHeader>

        {apiError?.message && (
          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>
              {t("common.error", { message: apiError.message })}
            </AlertDescription>
          </Alert>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <FormField
            label={t("auth:modals.addRole.roleName")}
            name="roleName"
            value={roleName}
            onChange={(e) => setRoleName(e.target.value)}
            placeholder={t("auth:modals.addRole.roleNamePlaceholder")}
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
              {t("auth:modals.addRole.cancel")}
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading
                ? t("auth:modals.addRole.submitLoading")
                : t("auth:modals.addRole.submit")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
