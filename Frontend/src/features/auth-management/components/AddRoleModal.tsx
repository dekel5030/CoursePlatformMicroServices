import { useState } from 'react';
import Modal from '@/components/ui/Modal/Modal';
import styles from './AddRoleModal.module.css';

interface AddRoleModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (roleName: string) => void;
  isLoading?: boolean;
}

export default function AddRoleModal({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
}: AddRoleModalProps) {
  const [roleName, setRoleName] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(roleName);
    setRoleName('');
  };

  const handleClose = () => {
    setRoleName('');
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Add Role">
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
            className={styles.input}
            placeholder="e.g., Admin, Instructor, Student"
            required
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
            {isLoading ? 'Adding...' : 'Add Role'}
          </button>
        </div>
      </form>
    </Modal>
  );
}
