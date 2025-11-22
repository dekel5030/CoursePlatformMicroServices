import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../features/auth/AuthContext";
import styles from "./ProfileMenu.module.css";

export default function ProfileMenu() {
  const navigate = useNavigate();
  const { currentUser, logout } = useAuth();
  const [isOpen, setIsOpen] = useState(false);

  const handleProfileClick = () => {
    if (currentUser) {
      navigate(`/users/${currentUser.userId}`);
      setIsOpen(false);
    }
  };

  const handleLogout = async () => {
    try {
      await logout();
      setIsOpen(false);
      navigate("/");
    } catch (error) {
      console.error("Logout failed:", error);
      // Navigate anyway since client state is cleared
      setIsOpen(false);
      navigate("/");
    }
  };

  const getInitial = () => {
    if (currentUser?.email) return currentUser.email.charAt(0).toUpperCase();
    return "U";
  };

  return (
    <div
      className={styles.profileMenuWrapper}
      onMouseEnter={() => setIsOpen(true)}
      onMouseLeave={() => setIsOpen(false)}
    >
      <div className={styles.avatar}>{getInitial()}</div>
      <ul className={`${styles.dropdownMenu} ${isOpen ? styles.show : ""}`}>
        <li className={styles.dropdownItem} onClick={handleProfileClick}>
          Profile Page
        </li>
        <li className={styles.dropdownItem} onClick={handleLogout}>
          Logout
        </li>
      </ul>
    </div>
  );
}
