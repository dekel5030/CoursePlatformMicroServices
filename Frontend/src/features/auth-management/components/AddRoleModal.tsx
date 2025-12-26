import { useState } from "react";
import { Modal } from "@/components/ui";
import styles from "./AddRoleModal.module.css";
import type { ApiErrorResponse } from "@/api/axiosClient";

interface AddRoleModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (roleName: string) => Promise<void>;
  isLoading?: boolean;
}

export default function AddRoleModal({
  isOpen,
  onClose,
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
      onClose();
    } catch (err: unknown) {
      setApiError(err as ApiErrorResponse);
    }
  };

  const handleClose = () => {
    setRoleName("");
    setApiError(null);
    onClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title="Add Role"
      error={apiError?.message}
    >
      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.formGroup}>
          <label htmlFor="roleName" className={styles.label}>
            Role Name
          </label>
          <input
            id="roleName"
            type="text"
            value={roleName}
            onChange={(e) => setRoleName(e.target.value)}
            className={
              apiError?.errors?.RoleName ? styles.inputError : styles.input
            }
            placeholder="e.g., Admin, Instructor, Student"
            required
          />

          {apiError?.errors?.RoleName && (
            <span className={styles.fieldError}>
              {apiError.errors.RoleName[0]}
            </span>
          )}
        </div>

        <div className={styles.actions}>
          <button
            type="button"
            onClick={handleClose}
            className={styles.cancelButton}
            disabled={isLoading}
          >
            Cancel
          </button>
          <button
            type="submit"
            className={styles.submitButton}
            disabled={isLoading}
          >
            {isLoading ? "Adding..." : "Add Role"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
