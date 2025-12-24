import { useState, useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import { useToast } from '../../../hooks/useToast';
import { ApiError } from '../../../lib/apiClient';
import styles from './UsersPage.module.css';
import {
  getUserById,
  getAllRoles,
  addUserRole,
  removeUserRole,
  addUserPermission,
  removeUserPermission,
} from '../../../services/AuthAdminAPI';
import type { UserDto, RoleListDto, UserAddPermissionRequest } from '../../../types/auth';

export default function UsersPage() {
  const [userId, setUserId] = useState('');
  const [user, setUser] = useState<UserDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [roles, setRoles] = useState<RoleListDto[]>([]);
  const [showAddRoleModal, setShowAddRoleModal] = useState(false);
  const [showAddPermissionModal, setShowAddPermissionModal] = useState(false);
  const [selectedRole, setSelectedRole] = useState('');
  const [permissionForm, setPermissionForm] = useState<UserAddPermissionRequest>({
    key: '',
    effect: 'Allow',
    action: '',
    resource: '',
    resourceId: '*',
  });

  const auth = useAuth();
  const token = auth.user?.access_token;
  const { showToast } = useToast();

  useEffect(() => {
    if (token) {
      getAllRoles(token)
        .then(setRoles)
        .catch((err) => {
          console.error('Failed to load roles:', err);
        });
    }
  }, [token]);

  const handleSearch = async () => {
    if (!userId.trim()) {
      showToast('Please enter a user ID', 'warning');
      return;
    }

    setLoading(true);
    setError(null);
    setUser(null);

    try {
      const userData = await getUserById(userId, token);
      setUser(userData);
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.problemDetails.title || 'Failed to load user');
        showToast(err.problemDetails.title || 'Failed to load user', 'error');
      } else {
        setError('Failed to load user');
        showToast('Failed to load user', 'error');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleAddRole = async () => {
    if (!user || !selectedRole) return;

    try {
      await addUserRole(user.id, { roleName: selectedRole }, token);
      showToast('Role assigned successfully', 'success');
      setShowAddRoleModal(false);
      setSelectedRole('');
      const updatedUser = await getUserById(user.id, token);
      setUser(updatedUser);
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to assign role', 'error');
      } else {
        showToast('Failed to assign role', 'error');
      }
    }
  };

  const handleRemoveRole = async (roleName: string) => {
    if (!user) return;

    try {
      await removeUserRole(user.id, roleName, token);
      showToast('Role removed successfully', 'success');
      const updatedUser = await getUserById(user.id, token);
      setUser(updatedUser);
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to remove role', 'error');
      } else {
        showToast('Failed to remove role', 'error');
      }
    }
  };

  const handleAddPermission = async () => {
    if (!user) return;

    if (!permissionForm.key || !permissionForm.action || !permissionForm.resource) {
      showToast('Please fill in all required fields', 'warning');
      return;
    }

    try {
      await addUserPermission(user.id, permissionForm, token);
      showToast('Permission assigned successfully', 'success');
      setShowAddPermissionModal(false);
      setPermissionForm({
        key: '',
        effect: 'Allow',
        action: '',
        resource: '',
        resourceId: '*',
      });
      const updatedUser = await getUserById(user.id, token);
      setUser(updatedUser);
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to assign permission', 'error');
      } else {
        showToast('Failed to assign permission', 'error');
      }
    }
  };

  const handleRemovePermission = async (permissionKey: string) => {
    if (!user) return;

    try {
      await removeUserPermission(user.id, permissionKey, token);
      showToast('Permission removed successfully', 'success');
      const updatedUser = await getUserById(user.id, token);
      setUser(updatedUser);
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to remove permission', 'error');
      } else {
        showToast('Failed to remove permission', 'error');
      }
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>User Management</h1>
        <p className={styles.subtitle}>Manage user roles and permissions</p>
      </div>

      <div className={styles.searchSection}>
        <input
          type="text"
          className={styles.searchBox}
          placeholder="Enter User ID (GUID)..."
          value={userId}
          onChange={(e) => setUserId(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
        />
        <button className={styles.button} onClick={handleSearch} style={{ marginLeft: '12px' }}>
          Search
        </button>
      </div>

      {loading && <div className={styles.loading}>Loading user...</div>}
      {error && <div className={styles.error}>{error}</div>}

      {user && (
        <div className={styles.card}>
          <div className={styles.cardHeader}>
            <h2 className={styles.cardTitle}>User Details</h2>
            <p className={styles.cardDescription}>ID: {user.id}</p>
          </div>
          <div className={styles.cardContent}>
            <div className={styles.infoGrid}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Email</span>
                <span className={styles.infoValue}>{user.email}</span>
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Name</span>
                <span className={styles.infoValue}>
                  {user.firstName} {user.lastName}
                </span>
              </div>
            </div>

            <div className={styles.section}>
              <div className={styles.sectionHeader}>
                <h3 className={styles.sectionTitle}>Roles ({user.roles.length})</h3>
                <button
                  className={styles.button}
                  onClick={() => setShowAddRoleModal(true)}
                >
                  Assign Role
                </button>
              </div>
              <div className={styles.list}>
                {user.roles.length === 0 ? (
                  <p style={{ color: '#6b7280', fontSize: '14px' }}>No roles assigned</p>
                ) : (
                  user.roles.map((role) => (
                    <div key={role.id} className={styles.listItem}>
                      <span className={styles.listItemName}>{role.name}</span>
                      <button
                        className={`${styles.button} ${styles.buttonSmall} ${styles.buttonDanger}`}
                        onClick={() => handleRemoveRole(role.name)}
                      >
                        Remove
                      </button>
                    </div>
                  ))
                )}
              </div>
            </div>

            <div className={styles.section}>
              <div className={styles.sectionHeader}>
                <h3 className={styles.sectionTitle}>
                  Direct Permissions ({user.permissions.length})
                </h3>
                <button
                  className={styles.button}
                  onClick={() => setShowAddPermissionModal(true)}
                >
                  Assign Permission
                </button>
              </div>
              <div className={styles.list}>
                {user.permissions.length === 0 ? (
                  <p style={{ color: '#6b7280', fontSize: '14px' }}>
                    No direct permissions assigned
                  </p>
                ) : (
                  user.permissions.map((perm) => (
                    <div key={perm.key} className={styles.listItem}>
                      <div>
                        <div className={styles.listItemName}>
                          {perm.effect} {perm.action} on {perm.resource}
                        </div>
                        <div className={styles.listItemKey}>{perm.key}</div>
                      </div>
                      <button
                        className={`${styles.button} ${styles.buttonSmall} ${styles.buttonDanger}`}
                        onClick={() => handleRemovePermission(perm.key)}
                      >
                        Remove
                      </button>
                    </div>
                  ))
                )}
              </div>
            </div>
          </div>
        </div>
      )}

      {showAddRoleModal && (
        <div className={styles.modal} onClick={() => setShowAddRoleModal(false)}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2 className={styles.modalTitle}>Assign Role</h2>
            </div>
            <div className={styles.form}>
              <div className={styles.formGroup}>
                <label className={styles.label}>Role</label>
                <select
                  className={styles.select}
                  value={selectedRole}
                  onChange={(e) => setSelectedRole(e.target.value)}
                >
                  <option value="">Select a role...</option>
                  {roles.map((role) => (
                    <option key={role.id} value={role.name}>
                      {role.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>
            <div className={styles.modalActions}>
              <button
                className={`${styles.button} ${styles.buttonSecondary}`}
                onClick={() => setShowAddRoleModal(false)}
              >
                Cancel
              </button>
              <button className={styles.button} onClick={handleAddRole}>
                Assign
              </button>
            </div>
          </div>
        </div>
      )}

      {showAddPermissionModal && (
        <div className={styles.modal} onClick={() => setShowAddPermissionModal(false)}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2 className={styles.modalTitle}>Assign Permission</h2>
            </div>
            <div className={styles.form}>
              <div className={styles.formGroup}>
                <label className={styles.label}>Key *</label>
                <input
                  type="text"
                  className={styles.input}
                  value={permissionForm.key}
                  onChange={(e) =>
                    setPermissionForm({ ...permissionForm, key: e.target.value })
                  }
                  placeholder="e.g., user:read:all"
                />
              </div>
              <div className={styles.formGroup}>
                <label className={styles.label}>Effect *</label>
                <select
                  className={styles.select}
                  value={permissionForm.effect}
                  onChange={(e) =>
                    setPermissionForm({ ...permissionForm, effect: e.target.value })
                  }
                >
                  <option value="Allow">Allow</option>
                  <option value="Deny">Deny</option>
                </select>
              </div>
              <div className={styles.formGroup}>
                <label className={styles.label}>Action *</label>
                <input
                  type="text"
                  className={styles.input}
                  value={permissionForm.action}
                  onChange={(e) =>
                    setPermissionForm({ ...permissionForm, action: e.target.value })
                  }
                  placeholder="e.g., read, write, delete"
                />
              </div>
              <div className={styles.formGroup}>
                <label className={styles.label}>Resource *</label>
                <input
                  type="text"
                  className={styles.input}
                  value={permissionForm.resource}
                  onChange={(e) =>
                    setPermissionForm({ ...permissionForm, resource: e.target.value })
                  }
                  placeholder="e.g., users, courses"
                />
              </div>
              <div className={styles.formGroup}>
                <label className={styles.label}>Resource ID</label>
                <input
                  type="text"
                  className={styles.input}
                  value={permissionForm.resourceId}
                  onChange={(e) =>
                    setPermissionForm({ ...permissionForm, resourceId: e.target.value })
                  }
                  placeholder="* for all resources"
                />
              </div>
            </div>
            <div className={styles.modalActions}>
              <button
                className={`${styles.button} ${styles.buttonSecondary}`}
                onClick={() => setShowAddPermissionModal(false)}
              >
                Cancel
              </button>
              <button className={styles.button} onClick={handleAddPermission}>
                Assign
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
