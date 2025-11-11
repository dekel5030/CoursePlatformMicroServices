import styles from "./Footer.module.css";

export default function Footer() {
  return (
    <footer className={styles.footer}>
      <div className={styles.container}>
        <div className={styles.logo}>CoursePlatform</div>

        <nav className={styles.links}>
          <a href="#">About</a>
          <a href="#">Contact</a>
          <a href="#">Terms</a>
          <a href="#">Privacy</a>
        </nav>

        <p className={styles.copy}>
          © {new Date().getFullYear()} CoursePlatform. All rights reserved.
        </p>
      </div>
    </footer>
  );
}
