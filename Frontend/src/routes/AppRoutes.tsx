import { Routes, Route } from "react-router-dom";
import HomePage from "../Pages/homePage/HomePage";
import CoursePage from "../Pages/CoursePage/CoursePage";
import LessonPage from "../Pages/LessonPage/LessonPage";
import LoginPage from "../features/auth/LoginPage/LoginPage";
import SignUpPage from "../features/auth/SignUpPage/SignUpPage";
import UserProfilePage from "../Pages/UserProfilePage/UserProfilePage";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/courses/:id" element={<CoursePage />} />
      <Route path="/lessons/:id" element={<LessonPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/signup" element={<SignUpPage />} />
      <Route path="/users/:id" element={<UserProfilePage />} />
    </Routes>
  );
}
