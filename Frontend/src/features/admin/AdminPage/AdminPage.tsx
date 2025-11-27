import { useState, useEffect } from "react";
import styles from "./AdminPage.module.css";
import type { Permission, Role, User } from "../../../services/AdminAPI";
import {
  getAllPermissions,
  createPermission,
  deletePermission,
  getAllRoles,
  createRole,
  deleteRole,
  assignPermissionToRole,
  removePermissionFromRole,
  getAllUsers,
  assignPermissionToUser,
  removePermissionFromUser,
  assignRoleToUser,
  removeRoleFromUser,
} from "../../../services/AdminAPI";

type Tab = "permissions" | "roles" | "users";

export default function AdminPage() {
  const [activeTab, setActiveTab] = useState<Tab>("permissions");
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [newPermissionName, setNewPermissionName] = useState("");
  const [newRoleName, setNewRoleName] = useState("");

  const [showAssignModal, setShowAssignModal] = useState(false);
  const [modalType, setModalType] = useState<
    "rolePermissions" | "userPermissions" | "userRoles"
  >("rolePermissions");
  const [selectedRoleId, setSelectedRoleId] = useState<number | null>(null);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [perms, rols, usrs] = await Promise.all([
        getAllPermissions(),
        getAllRoles(),
        getAllUsers(),
      ]);
      setPermissions(perms);
      setRoles(rols);
      setUsers(usrs);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load data");
    } finally {
      setLoading(false);
    }
  };

  const handleCreatePermission = async () => {
    if (!newPermissionName.trim()) return;
    try {
      await createPermission(newPermissionName.trim());
      setNewPermissionName("");
      await loadData();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to create permission"
      );
    }
  };

  const handleDeletePermission = async (id: number) => {
    if (!confirm("Are you sure you want to delete this permission?")) return;
    try {
      await deletePermission(id);
      await loadData();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to delete permission"
      );
    }
  };

  const handleCreateRole = async () => {
    if (!newRoleName.trim()) return;
    try {
      await createRole(newRoleName.trim());
      setNewRoleName("");
      await loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create role");
    }
  };

  const handleDeleteRole = async (id: number) => {
    if (!confirm("Are you sure you want to delete this role?")) return;
    try {
      await deleteRole(id);
      await loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete role");
    }
  };

  const handleAssignPermissionToRole = async (permissionId: number) => {
    if (!selectedRoleId) return;
    try {
      await assignPermissionToRole(selectedRoleId, permissionId);
      await loadData();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to assign permission"
      );
    }
  };

  const handleRemovePermissionFromRole = async (
    roleId: number,
    permissionId: number
  ) => {
    try {
      await removePermissionFromRole(roleId, permissionId);
      await loadData();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to remove permission"
      );
    }
  };

  const handleAssignPermissionToUser = async (permissionId: number) => {
    if (!selectedUserId) return;
    try {
      await assignPermissionToUser(selectedUserId, permissionId);
      await loadData();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to assign permission"
      );
    }
  };

  const handleRemovePermissionFromUser = async (
    userId: string,
    permissionId: number
  ) => {
    try {
      await removePermissionFromUser(userId, permissionId);
      await loadData();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to remove permission"
      );
    }
  };

  const handleAssignRoleToUser = async (roleId: number) => {
    if (!selectedUserId) return;
    try {
      await assignRoleToUser(selectedUserId, roleId);
      await loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to assign role");
    }
  };

  const handleRemoveRoleFromUser = async (userId: string, roleId: number) => {
    try {
      await removeRoleFromUser(userId, roleId);
      await loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to remove role");
    }
  };

  const openAssignModal = (
    type: "rolePermissions" | "userPermissions" | "userRoles",
    id: number | string
  ) => {
    setModalType(type);
    if (type === "rolePermissions") {
      setSelectedRoleId(id as number);
      setSelectedUserId(null);
    } else {
      setSelectedUserId(id as string);
      setSelectedRoleId(null);
    }
    setShowAssignModal(true);
  };

  const getSelectedRole = () =>
    roles.find((r) => r.id === selectedRoleId) || null;
  const getSelectedUser = () =>
    users.find((u) => u.id === selectedUserId) || null;

  const renderPermissionsTab = () => (
    <div className={styles.section}>
      <div className={styles.sectionHeader}>
        <h2 className={styles.sectionTitle}>Permissions</h2>
        <div className={styles.addForm}>
          <div className={styles.inputGroup}>
            <label className={styles.inputLabel}>Permission Name</label>
            <input
              type="text"
              className={styles.input}
              value={newPermissionName}
              onChange={(e) => setNewPermissionName(e.target.value)}
              placeholder="e.g., users:read"
              onKeyDown={(e) => e.key === "Enter" && handleCreatePermission()}
            />
          </div>
          <button
            className={styles.addButton}
            onClick={handleCreatePermission}
            disabled={!newPermissionName.trim()}
          >
            Add Permission
          </button>
        </div>
      </div>

      {permissions.length === 0 ? (
        <div className={styles.emptyState}>
          No permissions found. Create your first permission above.
        </div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {permissions.map((permission) => (
              <tr key={permission.id}>
                <td>{permission.id}</td>
                <td>{permission.name}</td>
                <td>
                  <button
                    className={`${styles.actionButton} ${styles.deleteButton}`}
                    onClick={() => handleDeletePermission(permission.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );

  const renderRolesTab = () => (
    <div className={styles.section}>
      <div className={styles.sectionHeader}>
        <h2 className={styles.sectionTitle}>Roles</h2>
        <div className={styles.addForm}>
          <div className={styles.inputGroup}>
            <label className={styles.inputLabel}>Role Name</label>
            <input
              type="text"
              className={styles.input}
              value={newRoleName}
              onChange={(e) => setNewRoleName(e.target.value)}
              placeholder="e.g., Admin"
              onKeyDown={(e) => e.key === "Enter" && handleCreateRole()}
            />
          </div>
          <button
            className={styles.addButton}
            onClick={handleCreateRole}
            disabled={!newRoleName.trim()}
          >
            Add Role
          </button>
        </div>
      </div>

      {roles.length === 0 ? (
        <div className={styles.emptyState}>
          No roles found. Create your first role above.
        </div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Permissions</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {roles.map((role) => (
              <tr key={role.id}>
                <td>{role.id}</td>
                <td>{role.name}</td>
                <td>
                  <div className={styles.permissionsList}>
                    {role.permissions.map((perm) => (
                      <span key={perm.id} className={styles.permissionTag}>
                        {perm.name}
                        <button
                          className={styles.removeTag}
                          onClick={() =>
                            handleRemovePermissionFromRole(role.id, perm.id)
                          }
                          title="Remove permission"
                        >
                          ×
                        </button>
                      </span>
                    ))}
                  </div>
                </td>
                <td>
                  <button
                    className={`${styles.actionButton} ${styles.assignButton}`}
                    onClick={() => openAssignModal("rolePermissions", role.id)}
                  >
                    Assign Permissions
                  </button>
                  <button
                    className={`${styles.actionButton} ${styles.deleteButton}`}
                    onClick={() => handleDeleteRole(role.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );

  const renderUsersTab = () => (
    <div className={styles.section}>
      <div className={styles.sectionHeader}>
        <h2 className={styles.sectionTitle}>Users</h2>
      </div>

      {users.length === 0 ? (
        <div className={styles.emptyState}>No users found.</div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Email</th>
              <th>Status</th>
              <th>Roles</th>
              <th>Direct Permissions</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map((user) => (
              <tr key={user.id}>
                <td>{user.email}</td>
                <td>{user.isActive ? "Active" : "Inactive"}</td>
                <td>
                  <div className={styles.rolesList}>
                    {user.roles.map((roleName) => {
                      const role = roles.find((r) => r.name === roleName);
                      return (
                        <span key={roleName} className={styles.roleTag}>
                          {roleName}
                          {role && (
                            <button
                              className={styles.removeTag}
                              onClick={() =>
                                handleRemoveRoleFromUser(user.id, role.id)
                              }
                              title="Remove role"
                            >
                              ×
                            </button>
                          )}
                        </span>
                      );
                    })}
                  </div>
                </td>
                <td>
                  <div className={styles.permissionsList}>
                    {user.directPermissions.map((permName) => {
                      const perm = permissions.find((p) => p.name === permName);
                      return (
                        <span key={permName} className={styles.permissionTag}>
                          {permName}
                          {perm && (
                            <button
                              className={styles.removeTag}
                              onClick={() =>
                                handleRemovePermissionFromUser(user.id, perm.id)
                              }
                              title="Remove permission"
                            >
                              ×
                            </button>
                          )}
                        </span>
                      );
                    })}
                  </div>
                </td>
                <td>
                  <button
                    className={`${styles.actionButton} ${styles.assignButton}`}
                    onClick={() => openAssignModal("userRoles", user.id)}
                  >
                    Assign Roles
                  </button>
                  <button
                    className={`${styles.actionButton} ${styles.assignButton}`}
                    onClick={() => openAssignModal("userPermissions", user.id)}
                  >
                    Assign Permissions
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );

  const renderAssignModal = () => {
    if (!showAssignModal) return null;

    let title = "";
    let items: { id: number; name: string; assigned: boolean }[] = [];
    let onAssign: (id: number) => void = () => {};

    if (modalType === "rolePermissions") {
      const role = getSelectedRole();
      title = `Assign Permissions to ${role?.name || "Role"}`;
      const assignedIds = new Set(role?.permissions.map((p) => p.id) || []);
      items = permissions.map((p) => ({
        id: p.id,
        name: p.name,
        assigned: assignedIds.has(p.id),
      }));
      onAssign = handleAssignPermissionToRole;
    } else if (modalType === "userPermissions") {
      const user = getSelectedUser();
      title = `Assign Permissions to ${user?.email || "User"}`;
      const assignedNames = new Set(user?.directPermissions || []);
      items = permissions.map((p) => ({
        id: p.id,
        name: p.name,
        assigned: assignedNames.has(p.name),
      }));
      onAssign = handleAssignPermissionToUser;
    } else if (modalType === "userRoles") {
      const user = getSelectedUser();
      title = `Assign Roles to ${user?.email || "User"}`;
      const assignedNames = new Set(user?.roles || []);
      items = roles.map((r) => ({
        id: r.id,
        name: r.name,
        assigned: assignedNames.has(r.name),
      }));
      onAssign = handleAssignRoleToUser;
    }

    return (
      <div className={styles.modal} onClick={() => setShowAssignModal(false)}>
        <div
          className={styles.modalContent}
          onClick={(e) => e.stopPropagation()}
        >
          <div className={styles.modalHeader}>
            <h3 className={styles.modalTitle}>{title}</h3>
            <button
              className={styles.closeButton}
              onClick={() => setShowAssignModal(false)}
            >
              ×
            </button>
          </div>
          <div className={styles.modalBody}>
            <div className={styles.checkboxList}>
              {items.map((item) => (
                <div key={item.id} className={styles.checkboxItem}>
                  <input
                    type="checkbox"
                    id={`item-${item.id}`}
                    checked={item.assigned}
                    onChange={() => {
                      if (!item.assigned) {
                        onAssign(item.id);
                      }
                    }}
                    disabled={item.assigned}
                  />
                  <label htmlFor={`item-${item.id}`}>
                    {item.name}
                    {item.assigned && " (assigned)"}
                  </label>
                </div>
              ))}
            </div>
          </div>
          <div className={styles.modalFooter}>
            <button
              className={styles.cancelButton}
              onClick={() => setShowAssignModal(false)}
            >
              Close
            </button>
          </div>
        </div>
      </div>
    );
  };

  if (loading && permissions.length === 0) {
    return (
      <div className={styles.container}>
        <div className={styles.loading}>Loading...</div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Admin Panel</h1>
        <p className={styles.subtitle}>
          Manage roles, permissions, and user access
        </p>
      </div>

      {error && <div className={styles.error}>{error}</div>}

      <div className={styles.tabs}>
        <button
          className={`${styles.tab} ${activeTab === "permissions" ? styles.tabActive : ""}`}
          onClick={() => setActiveTab("permissions")}
        >
          Permissions
        </button>
        <button
          className={`${styles.tab} ${activeTab === "roles" ? styles.tabActive : ""}`}
          onClick={() => setActiveTab("roles")}
        >
          Roles
        </button>
        <button
          className={`${styles.tab} ${activeTab === "users" ? styles.tabActive : ""}`}
          onClick={() => setActiveTab("users")}
        >
          Users
        </button>
      </div>

      {activeTab === "permissions" && renderPermissionsTab()}
      {activeTab === "roles" && renderRolesTab()}
      {activeTab === "users" && renderUsersTab()}

      {renderAssignModal()}
    </div>
  );
}
