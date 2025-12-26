import { Routes, Route } from "react-router-dom";
import HomePage from "../Pages/HomePage/HomePage";
import CoursePage from "../Pages/CoursePage/CoursePage";
import LessonPage from "../Pages/LessonPage/LessonPage";
import UserProfilePage from "../Pages/UserProfilePage/UserProfilePage";
import RoleManagementPage from "../Pages/Admin/RoleManagementPage/RoleManagementPage";
import RoleDetailPage from "../Pages/Admin/RoleDetailPage/RoleDetailPage";
import UserManagementPage from "../Pages/Admin/UserManagementPage/UserManagementPage";
import UsersListPage from "../Pages/Admin/UsersListPage/UsersListPage";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/courses/:id" element={<CoursePage />} />
      <Route path="/lessons/:id" element={<LessonPage />} />
      <Route path="/users/:id" element={<UserProfilePage />} />
      <Route path="/admin/roles" element={<RoleManagementPage />} />
      <Route path="/admin/roles/:roleName" element={<RoleDetailPage />} />
      <Route path="/admin/users" element={<UsersListPage />} />
      <Route path="/admin/users/:userId" element={<UserManagementPage />} />
    </Routes>
  );
}
