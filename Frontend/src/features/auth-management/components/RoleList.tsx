import { useNavigate } from 'react-router-dom';
import { Users } from 'lucide-react';
import { useRoles } from '../hooks';
import Badge from '@/components/ui/Badge/Badge';
import styles from './RoleList.module.css';

export default function RoleList() {
  const navigate = useNavigate();
  const { data: roles, isLoading, error } = useRoles();

  if (isLoading) {
    return (
      <div className={styles.container}>
        <div className={styles.skeleton}>
          <div className={styles.skeletonItem}></div>
          <div className={styles.skeletonItem}></div>
          <div className={styles.skeletonItem}></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <p>Failed to load roles: {error.message}</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h2 className={styles.title}>Roles</h2>
        <p className={styles.subtitle}>Manage security roles and their permissions</p>
      </div>

      {roles && roles.length === 0 ? (
        <div className={styles.empty}>
          <p>No roles found</p>
        </div>
      ) : (
        <div className={styles.grid}>
          {roles?.map((role) => (
            <div
              key={role.id}
              className={styles.card}
              onClick={() => navigate(`/admin/roles/${encodeURIComponent(role.name)}`)}
            >
              <div className={styles.cardHeader}>
                <h3 className={styles.roleName}>{role.name}</h3>
              </div>
              <div className={styles.cardBody}>
                <div className={styles.statsContainer}>
                  <div className={styles.stat}>
                    <span className={styles.statValue}>{role.userCount}</span>
                    <span className={styles.statLabel}>
                      <Users size={14} />
                      {role.userCount === 1 ? 'User' : 'Users'}
                    </span>
                  </div>
                  <div className={styles.stat}>
                    <span className={styles.statValue}>{role.permissionCount}</span>
                    <span className={styles.statLabel}>
                      {role.permissionCount === 1 ? 'Permission' : 'Permissions'}
                    </span>
                  </div>
                </div>
              </div>
              <div className={styles.cardFooter}>
                <Badge variant="default">{role.permissionCount} permissions</Badge>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
