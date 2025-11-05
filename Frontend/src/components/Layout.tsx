import type { ReactNode } from "react";
import Navbar from "./Navbar";

export default function Layout({ children }: { children: ReactNode }) {
  return (
    <div>
      <Navbar />
      <main style={{ padding: "1rem" }}>{children}</main>
    </div>
  );
}
