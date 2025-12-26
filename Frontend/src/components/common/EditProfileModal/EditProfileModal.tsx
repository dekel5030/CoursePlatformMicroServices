import { useState } from "react";
import { Modal } from "@/components/ui";
import { Input } from "@/components/ui";
import { type User, type UpdateUserRequest } from "@/services/UsersAPI";
import type { ApiErrorResponse } from "@/api/axiosClient";
import styles from "./EditProfileModal.module.css";

type EditProfileModalProps = {
  isOpen: boolean;
  onClose: () => void;
  user: User;
  onSave: (updatedUser: UpdateUserRequest) => Promise<void>;
};

export default function EditProfileModal({
  isOpen,
  onClose,
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
  const [submitSuccess, setSubmitSuccess] = useState(false);

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
      // Check if it matches the expected format with country code and number separated by space
      const phoneRegex = /^\+\d{1,4}\s\d{7,15}$/;
      if (!phoneRegex.test(trimmed)) {
        newErrors.phoneNumber = 
          "Phone number must be in format: +CountryCode Number (e.g., +1 1234567890)";
      }
    }

    // Date of birth validation (optional but if provided, should be valid)
    if (formData.dateOfBirth) {
      const dob = new Date(formData.dateOfBirth);
      const today = new Date();
      if (dob > today) {
        newErrors.dateOfBirth = "Date of birth cannot be in the future";
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setApiError(null);
    setSubmitSuccess(false);

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      // Parse phone number if provided
      // Expected format: "+CountryCode Number" (e.g., "+1 1234567890")
      let phoneNumber = null;
      if (formData.phoneNumber) {
        const trimmed = formData.phoneNumber.trim();
        const spaceIndex = trimmed.indexOf(" ");
        
        if (spaceIndex > 0) {
          phoneNumber = {
            countryCode: trimmed.substring(0, spaceIndex),
            number: trimmed.substring(spaceIndex + 1),
          };
        } else {
          // This case should be caught by validation, but handle it safely
          phoneNumber = {
            countryCode: "+1",
            number: trimmed,
          };
        }
      }

      const updatedData = {
        firstName: formData.firstName || null,
        lastName: formData.lastName || null,
        phoneNumber: phoneNumber,
        dateOfBirth: formData.dateOfBirth || null,
      };

      await onSave(updatedData);
      setSubmitSuccess(true);
      setTimeout(() => {
        onClose();
      }, 1500);
    } catch (error: unknown) {
      setApiError(error as ApiErrorResponse);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Edit Profile"
      error={apiError?.message}
    >
      <form onSubmit={handleSubmit} className={styles.form}>
        <Input
          label="First Name"
          name="firstName"
          value={formData.firstName}
          onChange={handleChange}
          placeholder="Enter your first name"
          error={errors.firstName || (apiError?.errors?.FirstName?.[0])}
        />

        <Input
          label="Last Name"
          name="lastName"
          value={formData.lastName}
          onChange={handleChange}
          placeholder="Enter your last name"
          error={errors.lastName || (apiError?.errors?.LastName?.[0])}
        />

        <Input
          label="Phone Number"
          name="phoneNumber"
          value={formData.phoneNumber}
          onChange={handleChange}
          placeholder="+1 1234567890"
          error={errors.phoneNumber || (apiError?.errors?.PhoneNumber?.[0])}
        />

        <Input
          label="Date of Birth"
          type="date"
          name="dateOfBirth"
          value={formData.dateOfBirth}
          onChange={handleChange}
          error={errors.dateOfBirth || (apiError?.errors?.DateOfBirth?.[0])}
        />

        {submitSuccess && (
          <div className={styles.successMessage}>
            Profile updated successfully!
          </div>
        )}

        <div className={styles.actions}>
          <button
            type="button"
            onClick={onClose}
            className={styles.cancelButton}
            disabled={isSubmitting}
          >
            Cancel
          </button>
          <button
            type="submit"
            className={styles.saveButton}
            disabled={isSubmitting}
          >
            {isSubmitting ? "Saving..." : "Save Changes"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
