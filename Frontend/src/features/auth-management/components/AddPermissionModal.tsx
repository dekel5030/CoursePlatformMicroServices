import { useState } from 'react';
import Modal from '@/components/ui/Modal/Modal';
import styles from './AddPermissionModal.module.css';
import type { AddPermissionRequest } from '../../types';

interface AddPermissionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (permission: AddPermissionRequest) => void;
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

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
    setFormData({
      effect: 'Allow',
      action: '',
      resource: '',
      resourceId: '*',
    });
  };

  const handleClose = () => {
    setFormData({
      effect: 'Allow',
      action: '',
      resource: '',
      resourceId: '*',
    });
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Add Permission">
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
            className={styles.input}
            placeholder="e.g., Read, Write, Delete"
            required
          />
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
            className={styles.input}
            placeholder="e.g., Course, User, Order"
            required
          />
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
            className={styles.input}
            placeholder="* for all, or specific ID"
          />
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
