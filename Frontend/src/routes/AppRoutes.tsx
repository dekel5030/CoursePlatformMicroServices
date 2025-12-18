import { Routes, Route } from "react-router-dom";
import HomePage from "../Pages/HomePage/HomePage";
import CoursePage from "../Pages/CoursePage/CoursePage";
import LessonPage from "../Pages/LessonPage/LessonPage";
import UserProfilePage from "../Pages/UserProfilePage/UserProfilePage";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/courses/:id" element={<CoursePage />} />
      <Route path="/lessons/:id" element={<LessonPage />} />
      <Route path="/users/:id" element={<UserProfilePage />} />
    </Routes>
  );
}
