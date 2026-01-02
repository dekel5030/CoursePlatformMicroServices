import { useState } from "react";
import { Skeleton, BreadcrumbNav } from "@/components";
import { UserTable, UserDrawer } from "@/features/iam-dashboard";
import { useUsers } from "@/features/iam-dashboard/hooks";
import type { UserDto } from "../../types/UserDto";
import { useTranslation } from "react-i18next";
import { motion } from "framer-motion";

export default function UsersListPage() {
  const { t } = useTranslation();
  const { data: users, isLoading, error } = useUsers();
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.users") },
  ];

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="bg-background border-b border-border py-3 px-8">
          <div className="max-w-7xl mx-auto">
            <Skeleton className="h-4 w-32" />
          </div>
        </div>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
          <div className="space-y-2">
            <Skeleton className="h-10 w-64" />
            <Skeleton className="h-4 w-96" />
          </div>
          <div className="space-y-4">
            <Skeleton className="h-10 w-full" />
            <div className="rounded-lg border border-border overflow-hidden">
              <div className="border-b bg-muted/50 p-4">
                <div className="grid grid-cols-4 gap-4">
                  {[1, 2, 3, 4].map((i) => (
                    <Skeleton key={i} className="h-4 w-full" />
                  ))}
                </div>
              </div>
              {[1, 2, 3, 4, 5].map((i) => (
                <div key={i} className="p-4 border-b">
                  <div className="grid grid-cols-4 gap-4">
                    <Skeleton className="h-4 w-3/4" />
                    <Skeleton className="h-4 w-full" />
                    <div className="flex gap-1">
                      <Skeleton className="h-5 w-16" />
                      <Skeleton className="h-5 w-16" />
                    </div>
                    <Skeleton className="h-8 w-20 ml-auto" />
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-6">
        <BreadcrumbNav items={breadcrumbItems} />
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
            <h2 className="font-semibold mb-1">
              {t("common.error", { message: error.message })}
            </h2>
            <p className="text-sm">{error.message}</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <BreadcrumbNav items={breadcrumbItems} />
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6"
      >
        <div className="space-y-2">
          <h1 className="text-3xl font-bold">
            {t("authManagement.users.title")}
          </h1>
          <p className="text-muted-foreground">
            {t("authManagement.users.subtitle")}
          </p>
        </div>

        {users && users.length === 0 ? (
          <div className="text-center py-12 text-muted-foreground">
            <p>{t("authManagement.users.noUsers")}</p>
          </div>
        ) : (
          <UserTable users={users || []} onUserSelect={setSelectedUser} />
        )}

        <UserDrawer
          open={!!selectedUser}
          onOpenChange={(open) => !open && setSelectedUser(null)}
          user={selectedUser}
        />
      </motion.div>
    </div>
  );
}
