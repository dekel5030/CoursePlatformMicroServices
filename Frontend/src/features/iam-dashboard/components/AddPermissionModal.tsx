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
import {
  Button,
  FormField,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components";
import { AlertCircle } from "lucide-react";
import type { AddPermissionRequest } from "../types/AddPermissionRequest";
import type { ApiErrorResponse } from "@/axios/axiosClient";
import { useTranslation } from "react-i18next";

interface AddPermissionModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (permission: AddPermissionRequest) => Promise<void>;
  isLoading?: boolean;
}

export default function AddPermissionModal({
  open,
  onOpenChange,
  onSubmit,
  isLoading = false,
}: AddPermissionModalProps) {
  const { t } = useTranslation(["auth", "translation"]);
  const [formData, setFormData] = useState<AddPermissionRequest>({
    effect: "Allow",
    action: "",
    resource: "",
    resourceId: "*",
  });
  const [apiError, setApiError] = useState<ApiErrorResponse | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev: AddPermissionRequest) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setApiError(null);

    try {
      await onSubmit(formData);
      setFormData({
        effect: "Allow",
        action: "",
        resource: "",
        resourceId: "*",
      });
      setApiError(null);
      onOpenChange(false);
    } catch (err: unknown) {
      setApiError(err as ApiErrorResponse);
    }
  };

  const handleClose = () => {
    setFormData({
      effect: "Allow",
      action: "",
      resource: "",
      resourceId: "*",
    });
    setApiError(null);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("auth:modals.addPermission.title")}</DialogTitle>
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
          <div className="space-y-2">
            <label htmlFor="effect" className="text-sm font-medium">
              {t("auth:modals.addPermission.effect")}
            </label>
            <Select
              value={formData.effect}
              onValueChange={(value) =>
                setFormData((prev) => ({
                  ...prev,
                  effect: value as "Allow" | "Deny",
                }))
              }
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Allow">Allow</SelectItem>
                <SelectItem value="Deny">Deny</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <FormField
            label={t("auth:modals.addPermission.action")}
            name="action"
            value={formData.action}
            onChange={handleChange}
            placeholder={t("auth:modals.addPermission.actionPlaceholder")}
            error={apiError?.errors?.Action?.[0]}
            required
          />

          <FormField
            label={t("auth:modals.addPermission.resource")}
            name="resource"
            value={formData.resource}
            onChange={handleChange}
            placeholder={t("auth:modals.addPermission.resourcePlaceholder")}
            error={apiError?.errors?.Resource?.[0]}
            required
          />

          <FormField
            label={t("auth:modals.addPermission.resourceId")}
            name="resourceId"
            value={formData.resourceId}
            onChange={handleChange}
            placeholder={t("auth:modals.addPermission.resourceIdPlaceholder")}
            error={apiError?.errors?.ResourceId?.[0]}
          />

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={isLoading}
            >
              {t("auth:modals.addPermission.cancel")}
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading
                ? t("auth:modals.addPermission.submitLoading")
                : t("auth:modals.addPermission.submit")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
