import { NavLink, Outlet } from 'react-router-dom';
import styles from './AdminLayout.module.css';

export default function AdminLayout() {
  return (
    <div className={styles.adminLayout}>
      <aside className={styles.sidebar}>
        <div className={styles.logo}>
          <h1 className={styles.logoTitle}>Admin Panel</h1>
          <p className={styles.logoSubtitle}>Identity Management</p>
        </div>
        <nav className={styles.nav}>
          <NavLink
            to="/admin/users"
            className={({ isActive }) =>
              isActive ? `${styles.navLink} ${styles.active}` : styles.navLink
            }
          >
            Users
          </NavLink>
          <NavLink
            to="/admin/roles"
            className={({ isActive }) =>
              isActive ? `${styles.navLink} ${styles.active}` : styles.navLink
            }
          >
            Roles
          </NavLink>
        </nav>
      </aside>
      <main className={styles.content}>
        <Outlet />
      </main>
    </div>
  );
}
