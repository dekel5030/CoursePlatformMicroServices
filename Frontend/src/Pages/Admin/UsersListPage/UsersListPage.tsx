import { useState } from 'react';
import { UserTable, UserDrawer } from '@/features/auth-management';
import { useUsers } from '@/features/auth-management/hooks';
import type { UserDto } from '@/features/auth-management/types';
import styles from './UsersListPage.module.css';

export default function UsersListPage() {
  const { data: users, isLoading, error } = useUsers();
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);

  if (isLoading) {
    return (
      <div className={styles.container}>
        <div className={styles.skeleton}>
          <div className={styles.skeletonHeader}></div>
          <div className={styles.skeletonTable}></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <h2>Failed to load users</h2>
          <p>{error.message}</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1 className={styles.title}>User Management</h1>
          <p className={styles.subtitle}>
            Manage user roles and permissions
          </p>
        </div>
      </div>

      {users && users.length === 0 ? (
        <div className={styles.empty}>
          <p>No users found in the system</p>
        </div>
      ) : (
        <UserTable users={users || []} onUserSelect={setSelectedUser} />
      )}

      <UserDrawer
        isOpen={!!selectedUser}
        onClose={() => setSelectedUser(null)}
        user={selectedUser}
      />
    </div>
  );
}
