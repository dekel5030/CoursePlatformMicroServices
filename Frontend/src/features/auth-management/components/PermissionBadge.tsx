import styles from './PermissionBadge.module.css';
import type { PermissionDto } from '../../types';

interface PermissionBadgeProps {
  permission: PermissionDto;
  onRemove?: () => void;
  showRemove?: boolean;
}

export default function PermissionBadge({
  permission,
  onRemove,
  showRemove = false,
}: PermissionBadgeProps) {
  const effectClass =
    permission.effect.toLowerCase() === 'allow'
      ? styles.allow
      : styles.deny;

  return (
    <div className={`${styles.badge} ${effectClass}`}>
      <div className={styles.content}>
        <span className={styles.effect}>{permission.effect}</span>
        <span className={styles.separator}>:</span>
        <span className={styles.action}>{permission.action}</span>
        <span className={styles.separator}>on</span>
        <span className={styles.resource}>{permission.resource}</span>
        {permission.resourceId !== '*' && (
          <>
            <span className={styles.separator}>/</span>
            <span className={styles.resourceId}>{permission.resourceId}</span>
          </>
        )}
      </div>
      {showRemove && onRemove && (
        <button
          className={styles.removeButton}
          onClick={onRemove}
          title="Remove permission"
          type="button"
        >
          ×
        </button>
      )}
    </div>
  );
}
