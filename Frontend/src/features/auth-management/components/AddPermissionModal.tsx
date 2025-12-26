import { useState } from 'react';
import Modal from '@/components/ui/Modal/Modal';
import styles from './AddPermissionModal.module.css';
import type { AddPermissionRequest } from '../types';
import type { ApiErrorResponse } from '@/api/axiosClient';

interface AddPermissionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (permission: AddPermissionRequest) => Promise<void>;
  isLoading?: boolean;
}

export default function AddPermissionModal({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
}: AddPermissionModalProps) {
  const [formData, setFormData] = useState<AddPermissionRequest>({
    effect: 'Allow',
    action: '',
    resource: '',
    resourceId: '*',
  });
  const [apiError, setApiError] = useState<ApiErrorResponse | null>(null);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev: AddPermissionRequest) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setApiError(null);

    try {
      await onSubmit(formData);
      setFormData({
        effect: 'Allow',
        action: '',
        resource: '',
        resourceId: '*',
      });
      setApiError(null);
      onClose();
    } catch (err: unknown) {
      setApiError(err as ApiErrorResponse);
    }
  };

  const handleClose = () => {
    setFormData({
      effect: 'Allow',
      action: '',
      resource: '',
      resourceId: '*',
    });
    setApiError(null);
    onClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title="Add Permission"
      error={apiError?.message}
    >
      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.formGroup}>
          <label htmlFor="effect" className={styles.label}>
            Effect
          </label>
          <select
            id="effect"
            name="effect"
            value={formData.effect}
            onChange={handleChange}
            className={styles.select}
            required
          >
            <option value="Allow">Allow</option>
            <option value="Deny">Deny</option>
          </select>
        </div>

        <div className={styles.formGroup}>
          <label htmlFor="action" className={styles.label}>
            Action
          </label>
          <input
            id="action"
            type="text"
            name="action"
            value={formData.action}
            onChange={handleChange}
            className={
              apiError?.errors?.Action ? styles.inputError : styles.input
            }
            placeholder="e.g., Read, Write, Delete"
            required
          />
          {apiError?.errors?.Action && (
            <span className={styles.fieldError}>
              {apiError.errors.Action[0]}
            </span>
          )}
        </div>

        <div className={styles.formGroup}>
          <label htmlFor="resource" className={styles.label}>
            Resource
          </label>
          <input
            id="resource"
            type="text"
            name="resource"
            value={formData.resource}
            onChange={handleChange}
            className={
              apiError?.errors?.Resource ? styles.inputError : styles.input
            }
            placeholder="e.g., Course, User, Order"
            required
          />
          {apiError?.errors?.Resource && (
            <span className={styles.fieldError}>
              {apiError.errors.Resource[0]}
            </span>
          )}
        </div>

        <div className={styles.formGroup}>
          <label htmlFor="resourceId" className={styles.label}>
            Resource ID
          </label>
          <input
            id="resourceId"
            type="text"
            name="resourceId"
            value={formData.resourceId}
            onChange={handleChange}
            className={
              apiError?.errors?.ResourceId ? styles.inputError : styles.input
            }
            placeholder="* for all, or specific ID"
          />
          {apiError?.errors?.ResourceId && (
            <span className={styles.fieldError}>
              {apiError.errors.ResourceId[0]}
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
            {isLoading ? 'Adding...' : 'Add Permission'}
          </button>
        </div>
      </form>
    </Modal>
  );
}
