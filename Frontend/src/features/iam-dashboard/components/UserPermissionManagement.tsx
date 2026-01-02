import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { useTranslation } from "react-i18next";

import { Authorized } from "@/features/auth";

import {
  useUser,
  UserRoleManagement,
  UserPermissionMatrix,
} from "@/features/iam-dashboard";

import { Button, Skeleton } from "@/components/ui";

export default function UserManagementPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { userId } = useParams<{ userId: string }>();

  const { data: user, isLoading, error } = useUser(userId);

  return (
    <Authorized
      action="Update"
      resource="User"
      fallback={
        <div className="p-8 text-center">{t("common.unauthorized")}</div>
      }
    >
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
        {/* Navigation & Header */}
        <div className="space-y-4">
          <Button
            onClick={() => navigate(-1)}
            variant="ghost"
            className="gap-2"
          >
            <ArrowLeft className="h-4 w-4" />
            {t("common.back")}
          </Button>

          {isLoading ? (
            <div className="space-y-2">
              <Skeleton className="h-10 w-64" />
              <Skeleton className="h-6 w-48" />
            </div>
          ) : error ? (
            <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
              Failed to load user: {error.message}
            </div>
          ) : user ? (
            <div className="space-y-1">
              <h1 className="text-3xl font-bold">
                {user.firstName} {user.lastName}
              </h1>
              <p className="text-lg text-muted-foreground">{user.email}</p>
            </div>
          ) : (
            <div className="text-muted-foreground">User not found</div>
          )}
        </div>

        {user && (
          <>
            <UserRoleManagement userId={userId!} />
            <UserPermissionMatrix
              userId={userId!}
              permissions={user.permissions}
            />
          </>
        )}
      </div>
    </Authorized>
  );
}
