import { Navbar, Footer, Breadcrumb } from "@/components/layout";
import type { ReactNode } from "react";
import styles from "./Layout.module.css";

type LayoutProps = {
  children: ReactNode;
};

export default function Layout({ children }: LayoutProps) {
  return (
    <div className={styles.layout}>
      <Navbar />
      <Breadcrumb />
      <main>{children}</main>
      <Footer />
    </div>
  );
}
