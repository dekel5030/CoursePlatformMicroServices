import { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./ProfileMenu.module.css";
import { useAuth } from "react-oidc-context";

export default function ProfileMenu() {
  const navigate = useNavigate();
  const auth = useAuth();
  const [isOpen, setIsOpen] = useState(false);

  const userProfile = auth.user?.profile;

  const handleProfileClick = () => {
    if (userProfile?.sub) {
      navigate(`/users/${userProfile.sub}`);
      setIsOpen(false);
    }
  };

  const handleLogout = async () => {
    try {
      await auth.signoutRedirect();
      setIsOpen(false);
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  const getInitial = () => {
    if (userProfile?.given_name)
      return userProfile.given_name.charAt(0).toUpperCase();
    if (userProfile?.email) return userProfile.email.charAt(0).toUpperCase();
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
