import Navbar from "../NavBar/Navbar";
import Footer from "../Footer/Footer";
import type { ReactNode } from "react";
import styles from "./Layout.module.css";

type LayoutProps = {
  children: ReactNode;
};

export default function Layout({ children }: LayoutProps) {
  return (
    <div className={styles.layout}>
      <Navbar />
      <main>{children}</main>
      <Footer />
    </div>
  );
}
