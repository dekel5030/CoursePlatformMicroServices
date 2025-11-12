import { Routes, Route } from "react-router-dom";
import HomePage from "../pages/homePage/HomePage";
import Courses from "../features/Courses/Courses";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/courses" element={<Courses />} />
    </Routes>
  );
}
