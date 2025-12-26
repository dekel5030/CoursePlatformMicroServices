import { useState } from "react";
import { toast } from "sonner";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui";
import { Button, FormField } from "@/components/ui";
import { type User, type UpdateUserRequest } from "@/services/UsersAPI";
import type { ApiErrorResponse } from "@/api/axiosClient";

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
        newErrors.phoneNumber =
          'Phone must be in format: +<country code> <number> (e.g., "+1 1234567890")';
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
            newErrors.phoneNumber = 'Phone must include country code and number separated by space (e.g., "+1 1234567890")';
            setErrors(newErrors);
            return;
          }
        } else {
          newErrors.phoneNumber =
            'Phone must start with + followed by country code (e.g., "+1 1234567890")';
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
      toast.success("Profile updated successfully!");
      onOpenChange(false);
    } catch (error: unknown) {
      const apiError = error as ApiErrorResponse;
      setApiError(apiError);
      toast.error(apiError.message || "Failed to update profile");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Profile</DialogTitle>
        </DialogHeader>

        {apiError?.message && (
          <div className="bg-destructive/15 text-destructive px-4 py-2 rounded-md text-sm">
            {apiError.message}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <FormField
            label="First Name"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            placeholder="Enter your first name"
            error={errors.firstName || (apiError?.errors?.FirstName?.[0])}
          />

          <FormField
            label="Last Name"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            placeholder="Enter your last name"
            error={errors.lastName || (apiError?.errors?.LastName?.[0])}
          />

          <FormField
            label="Phone Number"
            name="phoneNumber"
            value={formData.phoneNumber}
            onChange={handleChange}
            placeholder="+1 1234567890"
            error={errors.phoneNumber || (apiError?.errors?.PhoneNumber?.[0])}
          />

          <FormField
            label="Date of Birth"
            type="date"
            name="dateOfBirth"
            error={errors.dateOfBirth || (apiError?.errors?.DateOfBirth?.[0])}
          />

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Saving..." : "Save Changes"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
