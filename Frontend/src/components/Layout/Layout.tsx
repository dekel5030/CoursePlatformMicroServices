import Navbar from "../NavBar/Navbar";
import Footer from "../Footer/Footer";
import Breadcrumb from "../Breadcrumb/Breadcrumb";
import type { ReactNode } from "react";
import { useLocation } from "react-router-dom";
import styles from "./Layout.module.css";

type LayoutProps = {
  children: ReactNode;
};

export default function Layout({ children }: LayoutProps) {
  const location = useLocation();
  const isAdminRoute = location.pathname.startsWith('/admin');

  if (isAdminRoute) {
    return <>{children}</>;
  }

  return (
    <div className={styles.layout}>
      <Navbar />
      <Breadcrumb />
      <main>{children}</main>
      <Footer />
    </div>
  );
}
