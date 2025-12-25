import { Routes, Route } from "react-router-dom";
import HomePage from "@/Pages/homePage/HomePage";
import CoursePage from "@/Pages/CoursePage/CoursePage";
import LessonPage from "@/Pages/LessonPage/LessonPage";
import UserProfilePage from "@/Pages/UserProfilePage/UserProfilePage";
import { AdminLayout } from "@/components";
import { UsersPage, RolesPage } from "@/features/admin";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/courses/:id" element={<CoursePage />} />
      <Route path="/lessons/:id" element={<LessonPage />} />
      <Route path="/users/:id" element={<UserProfilePage />} />
      <Route path="/admin" element={<AdminLayout />}>
        <Route path="users" element={<UsersPage />} />
        <Route path="roles" element={<RolesPage />} />
      </Route>
    </Routes>
  );
}
