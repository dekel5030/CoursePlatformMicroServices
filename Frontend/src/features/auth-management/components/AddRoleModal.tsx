import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  Alert,
  AlertDescription,
} from "@/components/ui";
import { Button, FormField } from "@/components/ui";
import { AlertCircle } from "lucide-react";
import type { ApiErrorResponse } from "@/api/axiosClient";
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
  const { t } = useTranslation();
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
          <DialogTitle>{t('modals.addRole.title')}</DialogTitle>
        </DialogHeader>

        {apiError?.message && (
          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>
              {t('common.error', { message: apiError.message })}
            </AlertDescription>
          </Alert>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <FormField
            label={t('modals.addRole.roleName')}
            name="roleName"
            value={roleName}
            onChange={(e) => setRoleName(e.target.value)}
            placeholder={t('modals.addRole.roleNamePlaceholder')}
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
              {t('modals.addRole.cancel')}
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? t('modals.addRole.submitLoading') : t('modals.addRole.submit')}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
