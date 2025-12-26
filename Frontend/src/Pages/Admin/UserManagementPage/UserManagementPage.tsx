import { useParams, useNavigate } from 'react-router-dom';
import { useAuthUser } from '@/features/auth-management/hooks';
import { UserRoleManagement } from '@/features/auth-management';
import UserPermissionMatrix from '@/features/auth-management/components/UserPermissionMatrix/UserPermissionMatrix';
import styles from './UserManagementPage.module.css';

export default function UserManagementPage() {
  const navigate = useNavigate();
  const { userId } = useParams<{ userId: string }>();
  const { data: user, isLoading, error } = useAuthUser(userId);

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
          <p>Failed to load user: {error.message}</p>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <p>User not found</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <button onClick={() => navigate(-1)} className={styles.backButton}>
          ← Back
        </button>
        <h1 className={styles.title}>
          {user.firstName} {user.lastName}
        </h1>
        <p className={styles.subtitle}>{user.email}</p>
      </div>

      <UserRoleManagement userId={userId!} />
      <UserPermissionMatrix userId={userId!} permissions={user.permissions} />
    </div>
  );
}
