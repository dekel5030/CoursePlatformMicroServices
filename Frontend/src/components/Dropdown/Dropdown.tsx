import type { ReactNode } from "react";
import styles from "./Dropdown.module.css";

interface DropdownProps {
  label: string;
  children: ReactNode;
}

export default function Dropdown({ label, children }: DropdownProps) {
  return (
    <li className={styles.navItem}>
      <button className={styles.dropdownButton}>{label}</button>
      <ul className={styles.dropdownMenu}>{children}</ul>
    </li>
  );
}
