import { useNavigate } from "react-router-dom";
import { useAuth } from "../../features/auth/AuthContext";
import styles from "./ProfileMenu.module.css";

export default function ProfileMenu() {
  const navigate = useNavigate();
  const { currentUser, logout } = useAuth();

  const handleProfileClick = () => {
    if (currentUser) {
      navigate(`/users/${currentUser.userId}`);
    }
  };

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  // Default avatar - a simple circular div with user's initial
  const getInitial = () => {
    if (currentUser?.email) {
      return currentUser.email.charAt(0).toUpperCase();
    }
    return "U";
  };

  return (
    <div className={styles.profileMenu}>
      <div className={styles.avatar}>
        {getInitial()}
      </div>
      <ul className={styles.dropdownMenu}>
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
