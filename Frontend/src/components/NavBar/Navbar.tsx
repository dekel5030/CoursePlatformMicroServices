import styles from "./Navbar.module.css";
import Dropdown from "../Dropdown/Dropdown";
import Button from "../Button/Button";
import SearchBox from "../SearchBox/SearchBox";

export default function Navbar() {
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
        <Button variant="outlined">Log in</Button>
        <Button variant="filled">Sign up</Button>
      </div>
    </header>
  );
}
