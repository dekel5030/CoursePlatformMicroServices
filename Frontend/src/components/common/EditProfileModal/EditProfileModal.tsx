import { useState } from "react";
import { Modal, Input } from "@/components";
import { type User, type UpdateUserRequest } from "@/services/UserService";
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
    bio: user.bio || "",
    profilePictureUrl: user.profilePictureUrl || "",
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
      const updatedData: UpdateUserRequest = {
        firstName: formData.firstName || undefined,
        lastName: formData.lastName || undefined,
        bio: formData.bio || undefined,
        profilePictureUrl: formData.profilePictureUrl || undefined,
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
          label="Bio"
          name="bio"
          value={formData.bio}
          onChange={handleChange}
          placeholder="Tell us about yourself"
          error={errors.bio}
        />

        <Input
          label="Profile Picture URL"
          name="profilePictureUrl"
          value={formData.profilePictureUrl}
          onChange={handleChange}
          placeholder="https://example.com/profile.jpg"
          error={errors.profilePictureUrl}
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
