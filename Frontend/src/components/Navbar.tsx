import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav
      style={{ padding: "1rem", backgroundColor: "#282c34", color: "white" }}
    >
      <Link
        to="/"
        style={{ marginRight: "1rem", color: "white", textDecoration: "none" }}
      >
        Home
      </Link>
      <Link
        to="/courses"
        style={{ marginRight: "1rem", color: "white", textDecoration: "none" }}
      >
        Courses
      </Link>
      <Link to="/login" style={{ color: "white", textDecoration: "none" }}>
        Login
      </Link>
    </nav>
  );
}
