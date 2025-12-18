import styles from "./Navbar.module.css";
import Dropdown from "../Dropdown/Dropdown";
import Button from "../Button/Button";
import SearchBox from "../SearchBox/SearchBox";
import ProfileMenu from "../ProfileMenu/ProfileMenu";
import { useAuth } from "react-oidc-context";

export default function Navbar() {
  const auth = useAuth();

  const handleLoginClick = () => {
    void auth.signinRedirect();
  };

  const handleSignUpClick = () => {
    void auth.signinRedirect({
      extraQueryParams: { kc_action: "register" },
    });
  };

  return (
    <header className={styles.header}>
      <div className={styles.logo}>CourseHub</div>

      <nav className={styles.mainNav}>
        <ul className={styles.navList}>
          <Dropdown label="Explore">
            <li className={styles.dropdownItem}>Learn AI</li>
            <li className={styles.dropdownItem}>Launch a Career</li>
            <li className={styles.dropdownItem}>Certification Prep</li>
          </Dropdown>
          <SearchBox />
          <Dropdown label="Development">
            <li className={styles.dropdownItem}>Web Development</li>
            <li className={styles.dropdownItem}>Mobile Apps</li>
            <li className={styles.dropdownItem}>Data Science</li>
          </Dropdown>
          <li className={styles.navItem}>Design</li>
          <li className={styles.navItem}>Marketing</li>
        </ul>
      </nav>

      <div className={styles.authButtons}>
        {auth.isAuthenticated ? (
          <ProfileMenu />
        ) : (
          <>
            <Button variant="outlined" onClick={handleLoginClick}>
              Log in
            </Button>
            <Button variant="filled" onClick={handleSignUpClick}>
              Sign up
            </Button>
          </>
        )}
      </div>
    </header>
  );
}
