import "./Navbar.css";

export default function Navbar() {
  return (
    <nav className="navbar">
      <div className="logo">CourseHub</div>
      <ul className="nav-links">
        <li>Development</li>
        <li>Design</li>
        <li>Marketing</li>
      </ul>
      <button className="login-btn">Login</button>
    </nav>
  );
}
