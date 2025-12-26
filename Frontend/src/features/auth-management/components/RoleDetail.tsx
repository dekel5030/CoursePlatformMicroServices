import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Settings } from 'lucide-react';
import { useRole } from '../hooks';
import PermissionBadge from './PermissionBadge';
import PermissionMatrix from './PermissionMatrix/PermissionMatrix';
import styles from './RoleDetail.module.css';

export default function RoleDetail() {
  const navigate = useNavigate();
  const { roleName } = useParams<{ roleName: string }>();
  const { data: role, isLoading, error } = useRole(roleName);
  const [isMatrixOpen, setIsMatrixOpen] = useState(false);

  if (isLoading) {
    return (
      <div className={styles.container}>
        <div className={styles.skeleton}>
          <div className={styles.skeletonHeader}></div>
          <div className={styles.skeletonContent}></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <p>Failed to load role: {error.message}</p>
          <button onClick={() => navigate('/admin/roles')} className={styles.backButton}>
            Back to Roles
          </button>
        </div>
      </div>
    );
  }

  if (!role) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <p>Role not found</p>
          <button onClick={() => navigate('/admin/roles')} className={styles.backButton}>
            Back to Roles
          </button>
        </div>
      </div>
    );
  }

  const topPermissions = role.permissions.slice(0, 3);

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <button onClick={() => navigate('/admin/roles')} className={styles.backButton}>
          ← Back
        </button>
        <h1 className={styles.title}>{role.name}</h1>
        <p className={styles.subtitle}>
          {role.permissions.length} {role.permissions.length === 1 ? 'permission' : 'permissions'}
        </p>
      </div>

      <div className={styles.section}>
        <div className={styles.sectionHeader}>
          <h2 className={styles.sectionTitle}>Top Permissions</h2>
          <button onClick={() => setIsMatrixOpen(true)} className={styles.matrixButton}>
            <Settings size={16} />
            Manage All Permissions
          </button>
        </div>

        {role.permissions.length === 0 ? (
          <div className={styles.empty}>
            <p>No permissions assigned to this role</p>
          </div>
        ) : (
          <div className={styles.permissions}>
            {topPermissions.map((permission) => (
              <PermissionBadge
                key={permission.key}
                permission={permission}
                showRemove={false}
              />
            ))}
            {role.permissions.length > 3 && (
              <div className={styles.morePermissions}>
                <button onClick={() => setIsMatrixOpen(true)} className={styles.viewAllButton}>
                  View all {role.permissions.length} permissions →
                </button>
              </div>
            )}
          </div>
        )}
      </div>

      <PermissionMatrix
        isOpen={isMatrixOpen}
        onClose={() => setIsMatrixOpen(false)}
        roleName={roleName!}
        permissions={role.permissions}
      />
    </div>
  );
}
