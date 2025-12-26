import { Navbar, Footer, Breadcrumb } from "@/components/layout";
import type { ReactNode } from "react";

type LayoutProps = {
  children: ReactNode;
};

export default function Layout({ children }: LayoutProps) {
  return (
    <div className="flex flex-col">
      <Navbar />
      <Breadcrumb />
      <main className="min-h-screen">{children}</main>
      <Footer />
    </div>
  );
}
