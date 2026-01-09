import { Routes, Route } from "react-router-dom";
import { Layout, ManagementLayout } from "@/components";
import { ProtectedRoute } from "@/components/common";
import {
  CourseCatalogPage,
  CoursePage,
  AllCoursesPage,
} from "@/features/courses";
import { LessonPage } from "@/features/lessons";
import { UserProfilePage } from "@/features/users";
import { LandingPage } from "@/features/landing";
import { ForbiddenPage } from "@/features/errors";
import {
  AdminDashboardPage,
  RoleManagementPage,
  RoleDetailPage,
  UsersListPage,
  UserManagementPage,
} from "@/features/iam-dashboard";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/forbidden" element={<ForbiddenPage />} />

      <Route element={<Layout />}>
        <Route path="/catalog" element={<CourseCatalogPage />} />
        <Route path="/courses" element={<AllCoursesPage />} />
        <Route path="/courses/:id" element={<CoursePage />} />
        <Route
          path="/courses/:courseId/lessons/:lessonId"
          element={<LessonPage />}
        />
        <Route path="/users/:id" element={<UserProfilePage />} />
      </Route>

      <Route
        element={
          <ProtectedRoute requiredRoles={["admin", "instructor"]}>
            <ManagementLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/admin" element={<AdminDashboardPage />} />
        <Route path="/admin/roles" element={<RoleManagementPage />} />
        <Route path="/admin/roles/:roleName" element={<RoleDetailPage />} />
        <Route path="/admin/users" element={<UsersListPage />} />
        <Route path="/admin/users/:userId" element={<UserManagementPage />} />
      </Route>
    </Routes>
  );
}
