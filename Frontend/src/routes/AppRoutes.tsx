import { Routes, Route } from "react-router-dom";
import Home from "../features/Home/Home";
import Courses from "../features/Courses/Courses";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/courses" element={<Courses />} />
    </Routes>
  );
}
