import { useState, useEffect, useCallback } from 'react';
import { useToast } from '../../../hooks/useToast';
import { ApiError } from '../../../lib/apiClient';
import styles from './RolesPage.module.css';
import {
  getAllRoles,
  getRoleByName,
  createRole,
  addRolePermission,
  removeRolePermission,
} from '../../../services/AuthAdminAPI';
import type { RoleListDto, RoleDetailDto, RoleAddPermissionRequest } from '../../../types/auth';

export default function RolesPage() {
  const [roles, setRoles] = useState<RoleListDto[]>([]);
  const [selectedRoleName, setSelectedRoleName] = useState<string | null>(null);
  const [selectedRoleDetail, setSelectedRoleDetail] = useState<RoleDetailDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showAddPermissionModal, setShowAddPermissionModal] = useState(false);
  const [newRoleName, setNewRoleName] = useState('');
  const [permissionForm, setPermissionForm] = useState<RoleAddPermissionRequest>({
    key: '',
    effect: 'Allow',
    action: '',
    resource: '',
    resourceId: '*',
  });

  const { showToast } = useToast();

  const loadRoles = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await getAllRoles();
      setRoles(data);
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.problemDetails.title || 'Failed to load roles');
      } else {
        setError('Failed to load roles');
      }
    } finally {
      setLoading(false);
    }
  }, []);

  const loadRoleDetail = useCallback(async (roleName: string) => {
    setDetailLoading(true);

    try {
      const data = await getRoleByName(roleName);
      setSelectedRoleDetail(data);
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to load role details', 'error');
      } else {
        showToast('Failed to load role details', 'error');
      }
      setSelectedRoleDetail(null);
    } finally {
      setDetailLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    loadRoles();
  }, [loadRoles]);

  useEffect(() => {
    if (selectedRoleName) {
      loadRoleDetail(selectedRoleName);
    }
  }, [selectedRoleName, loadRoleDetail]);

  const handleCreateRole = async () => {
    if (!newRoleName.trim()) {
      showToast('Please enter a role name', 'warning');
      return;
    }

    try {
      await createRole({ name: newRoleName });
      showToast('Role created successfully', 'success');
      setShowCreateModal(false);
      setNewRoleName('');
      await loadRoles();
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to create role', 'error');
      } else {
        showToast('Failed to create role', 'error');
      }
    }
  };

  const handleAddPermission = async () => {
    if (!selectedRoleName) return;

    if (!permissionForm.key || !permissionForm.action || !permissionForm.resource) {
      showToast('Please fill in all required fields', 'warning');
      return;
    }

    try {
      await addRolePermission(selectedRoleName, permissionForm);
      showToast('Permission added successfully', 'success');
      setShowAddPermissionModal(false);
      setPermissionForm({
        key: '',
        effect: 'Allow',
        action: '',
        resource: '',
        resourceId: '*',
      });
      await loadRoleDetail(selectedRoleName);
      await loadRoles();
    } catch (err) {
      if (err instanceof ApiError) {
        showToast(err.problemDetails.title || 'Failed to add permission', 'error');
      } else {
        showToast('Failed to add permission', 'error');
      }
    }
  };

  const handleRemovePermission = async (permissionKey: string) => {
    if (!selectedRoleName) return;

    try {
      await removeRolePermission(selectedRoleName, permissionKey);
      showToast('Permission removed successfully', 'success');
      await loadRoleDetail(selectedRoleName);
      await loadRoles();
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
        <div className={styles.headerText}>
          <h1 className={styles.title}>Role Management</h1>
          <p className={styles.subtitle}>Create and manage security roles</p>
        </div>
        <button className={styles.button} onClick={() => setShowCreateModal(true)}>
          Create Role
        </button>
      </div>

      {loading && <div className={styles.loading}>Loading roles...</div>}
      {error && <div className={styles.error}>{error}</div>}

      {!loading && !error && (
        <>
          <div className={styles.grid}>
            {roles.map((role) => (
              <div
                key={role.id}
                className={`${styles.roleCard} ${
                  selectedRoleName === role.name ? styles.selected : ''
                }`}
                onClick={() => setSelectedRoleName(role.name)}
              >
                <h3 className={styles.roleName}>{role.name}</h3>
                <p className={styles.roleStats}>{role.permissionCount} permissions</p>
              </div>
            ))}
            {roles.length === 0 && (
              <div className={styles.emptyState}>
                <p>No roles found. Create your first role to get started.</p>
              </div>
            )}
          </div>

          {selectedRoleDetail && (
            <div className={styles.card}>
              <div className={styles.cardHeader}>
                <h2 className={styles.cardTitle}>
                  {selectedRoleDetail.name} Permissions
                </h2>
                <button
                  className={styles.button}
                  onClick={() => setShowAddPermissionModal(true)}
                >
                  Add Permission
                </button>
              </div>
              <div className={styles.cardContent}>
                {detailLoading ? (
                  <div className={styles.loading}>Loading permissions...</div>
                ) : (
                  <div className={styles.list}>
                    {selectedRoleDetail.permissions.length === 0 ? (
                      <p className={styles.emptyState}>No permissions assigned to this role</p>
                    ) : (
                      selectedRoleDetail.permissions.map((perm) => (
                        <div key={perm.key} className={styles.listItem}>
                          <div className={styles.permissionInfo}>
                            <div className={styles.permissionName}>
                              {perm.effect} {perm.action} on {perm.resource}
                            </div>
                            <div className={styles.permissionKey}>{perm.key}</div>
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
                )}
              </div>
            </div>
          )}
        </>
      )}

      {showCreateModal && (
        <div className={styles.modal} onClick={() => setShowCreateModal(false)}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2 className={styles.modalTitle}>Create New Role</h2>
            </div>
            <div className={styles.form}>
              <div className={styles.formGroup}>
                <label className={styles.label}>Role Name *</label>
                <input
                  type="text"
                  className={styles.input}
                  value={newRoleName}
                  onChange={(e) => setNewRoleName(e.target.value)}
                  placeholder="e.g., Administrator, Editor, Viewer"
                />
              </div>
            </div>
            <div className={styles.modalActions}>
              <button
                className={`${styles.button} ${styles.buttonSecondary}`}
                onClick={() => setShowCreateModal(false)}
              >
                Cancel
              </button>
              <button className={styles.button} onClick={handleCreateRole}>
                Create
              </button>
            </div>
          </div>
        </div>
      )}

      {showAddPermissionModal && (
        <div className={styles.modal} onClick={() => setShowAddPermissionModal(false)}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2 className={styles.modalTitle}>Add Permission</h2>
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
                Add
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
