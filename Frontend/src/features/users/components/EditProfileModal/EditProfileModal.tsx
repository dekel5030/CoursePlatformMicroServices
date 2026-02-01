import { useState } from "react";
import { toast } from "sonner";
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
import { type User, type UpdateUserRequest } from "@/features/users/api";
import type { ApiErrorResponse } from "@/app/axios";
import { useTranslation } from "react-i18next";

type EditProfileModalProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  user: User;
  onSave: (updatedUser: UpdateUserRequest) => Promise<void>;
};

export default function EditProfileModal({
  open,
  onOpenChange,
  user,
  onSave,
}: EditProfileModalProps) {
  const { t } = useTranslation();
  const [formData, setFormData] = useState({
    firstName: user.firstName || "",
    lastName: user.lastName || "",
    phoneNumber: user.phoneNumber || "",
    dateOfBirth: user.dateOfBirth
      ? new Date(user.dateOfBirth).toISOString().split("T")[0]
      : "",
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [apiError, setApiError] = useState<ApiErrorResponse | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    // Clear error for this field
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    // Phone number validation - must match format: countryCode number (e.g., "+1 1234567890")
    if (formData.phoneNumber) {
      const trimmed = formData.phoneNumber.trim();
      const phoneRegex = /^\+\d{1,4}\s\d{7,15}$/;

      if (!phoneRegex.test(trimmed)) {
        newErrors.phoneNumber = t("modals.editProfile.validation.phoneFormat");
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    setApiError(null);

    try {
      let phoneNumber: { countryCode: string; number: string } | null = null;
      const newErrors: Record<string, string> = {};

      if (formData.phoneNumber?.trim()) {
        const cleaned = formData.phoneNumber.replace(/\s+/g, " ").trim();

        if (cleaned.startsWith("+")) {
          // Split into country code and number
          const parts = cleaned.split(" ");
          if (parts.length >= 2) {
            phoneNumber = {
              countryCode: parts[0],
              number: parts.slice(1).join(""),
            };
          } else {
            newErrors.phoneNumber = t(
              "modals.editProfile.validation.phoneFormat"
            );
            setErrors(newErrors);
            return;
          }
        } else {
          newErrors.phoneNumber = t(
            "modals.editProfile.validation.phoneFormat"
          );
          setErrors(newErrors);
          return;
        }
      }

      const updatedData = {
        firstName: formData.firstName || null,
        lastName: formData.lastName || null,
        phoneNumber: phoneNumber,
        dateOfBirth: formData.dateOfBirth || null,
      };

      await onSave(updatedData);
      toast.success(t("modals.editProfile.success"));
      onOpenChange(false);
    } catch (error: unknown) {
      const apiError = error as ApiErrorResponse;
      setApiError(apiError);
      toast.error(
        apiError.message ||
          t("common.error", { message: "Failed to update profile" })
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("modals.editProfile.title")}</DialogTitle>
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
            label={t("modals.editProfile.firstName")}
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            placeholder={t("modals.editProfile.firstNamePlaceholder")}
            error={errors.firstName || apiError?.errors?.FirstName?.[0]}
          />

          <FormField
            label={t("modals.editProfile.lastName")}
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            placeholder={t("modals.editProfile.lastNamePlaceholder")}
            error={errors.lastName || apiError?.errors?.LastName?.[0]}
          />

          <FormField
            label={t("modals.editProfile.phone")}
            name="phoneNumber"
            value={formData.phoneNumber}
            onChange={handleChange}
            placeholder={t("modals.editProfile.phonePlaceholder")}
            error={errors.phoneNumber || apiError?.errors?.PhoneNumber?.[0]}
          />

          <FormField
            label={t("modals.editProfile.dob")}
            type="date"
            name="dateOfBirth"
            value={formData.dateOfBirth}
            onChange={handleChange}
            error={errors.dateOfBirth || apiError?.errors?.DateOfBirth?.[0]}
          />

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isSubmitting}
            >
              {t("modals.editProfile.cancel")}
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting
                ? t("modals.editProfile.submitLoading")
                : t("modals.editProfile.submit")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
