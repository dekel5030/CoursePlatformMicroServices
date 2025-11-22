import { useState } from "react";
import Modal from "../Modal/Modal";
import Input from "../Input/Input";
import { type User, type UpdateUserRequest } from "../../services/UsersAPI";
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
  const [submitError, setSubmitError] = useState<string | null>(null);
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

    // Phone number validation (optional but if provided, should be valid)
    if (formData.phoneNumber && !/^[+]?[\d\s-()]+$/.test(formData.phoneNumber)) {
      newErrors.phoneNumber = "Please enter a valid phone number";
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
    setSubmitError(null);
    setSubmitSuccess(false);

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      // Parse phone number if provided
      let phoneNumber = null;
      if (formData.phoneNumber) {
        // Simple parsing - split by space if format is "+1 1234567890"
        const parts = formData.phoneNumber.trim().split(/\s+/);
        if (parts.length >= 2) {
          phoneNumber = {
            countryCode: parts[0],
            number: parts.slice(1).join(" "),
          };
        } else {
          // If no space, assume whole thing is number with +1 as default country code
          phoneNumber = {
            countryCode: "+1",
            number: formData.phoneNumber.trim(),
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
    } catch (error) {
      setSubmitError(
        error instanceof Error ? error.message : "Failed to update profile"
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Edit Profile">
      <form onSubmit={handleSubmit} className={styles.form}>
        <Input
          label="First Name"
          name="firstName"
          value={formData.firstName}
          onChange={handleChange}
          placeholder="Enter your first name"
          error={errors.firstName}
        />

        <Input
          label="Last Name"
          name="lastName"
          value={formData.lastName}
          onChange={handleChange}
          placeholder="Enter your last name"
          error={errors.lastName}
        />

        <Input
          label="Phone Number"
          name="phoneNumber"
          value={formData.phoneNumber}
          onChange={handleChange}
          placeholder="+1 1234567890"
          error={errors.phoneNumber}
        />

        <Input
          label="Date of Birth"
          type="date"
          name="dateOfBirth"
          value={formData.dateOfBirth}
          onChange={handleChange}
          error={errors.dateOfBirth}
        />

        {submitError && (
          <div className={styles.errorMessage}>{submitError}</div>
        )}

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
