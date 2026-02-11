import { Routes, Route, Navigate } from "react-router-dom";
import { Layout, ManagementLayout } from "@/components/layout";
import { ProtectedRoute } from "@/shared/common";
import { CourseCatalogPage, AllCoursesPage } from "@/features/course-catalog";
import { MyCoursesPage } from "@/features/my-courses";
import { ManagedCoursesPage } from "@/features/managed-courses";
import { CoursePage, ManageCoursePage } from "@/features/course-management";
import { LessonPage } from "@/features/lesson-viewer";
import { UserProfilePage } from "@/features/user-profile";
import { LandingPage } from "@/features/landing";
import { ForbiddenPage } from "@/features/errors";
import {
  AdminDashboardPage,
  RoleManagementPage,
  RoleDetailPage,
  UsersListPage,
  UserManagementPage,
} from "@/features/admin-dashboard";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/forbidden" element={<ForbiddenPage />} />

      <Route element={<Layout />}>
        <Route path="/catalog" element={<CourseCatalogPage />} />
        <Route
          path="/users/me/courses/enrolled"
          element={
            <ProtectedRoute>
              <MyCoursesPage />
            </ProtectedRoute>
          }
        />
        <Route path="/my-courses" element={<Navigate to="/users/me/courses/enrolled" replace />} />
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
        <Route path="/manage/courses" element={<ManagedCoursesPage />} />
        <Route path="/manage/courses/:id" element={<ManageCoursePage />} />
      </Route>
    </Routes>
  );
}
