import { useState } from "react";
import { useParams } from "react-router-dom";
import { useUser, useUpdateUser } from "@/features/users";
import { EditProfileModal } from "@/components/common";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Skeleton,
} from "@/components/ui";
import Breadcrumb from "@/components/layout/Breadcrumb/Breadcrumb";
import { User, Mail, Phone, Calendar } from "lucide-react";
import { useTranslation } from "react-i18next";

export default function UserProfilePage() {
  const { id: userId } = useParams<{ id: string }>();
  const { data: user, isLoading, error } = useUser(userId!);
  const updateUser = useUpdateUser(userId!);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const { t } = useTranslation();

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="bg-background border-b border-border py-3 px-8">
          <div className="max-w-7xl mx-auto">
            <Skeleton className="h-4 w-32" />
          </div>
        </div>
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <Skeleton className="h-8 w-48" />
              <Skeleton className="h-10 w-24" />
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {[1, 2, 3, 4].map((i) => (
                  <div key={i} className="space-y-2">
                    <div className="flex items-center gap-2">
                      <Skeleton className="h-4 w-4 rounded-full" />
                      <Skeleton className="h-4 w-20" />
                    </div>
                    <Skeleton className="h-6 w-3/4" />
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  if (error || !user) {
    return (
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          {error?.message || "User not found"}
        </div>
      </div>
    );
  }

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.profile") },
  ];

  return (
    <div className="space-y-6">
      <Breadcrumb items={breadcrumbItems} />
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Profile Information</CardTitle>
            <Button onClick={() => setIsEditModalOpen(true)}>
              Edit Profile
            </Button>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <User className="h-4 w-4" />
                  <span>Name</span>
                </div>
                <p className="text-lg font-medium">
                  {user.firstName || user.lastName
                    ? `${user.firstName || ""} ${user.lastName || ""}`.trim()
                    : "Not provided"}
                </p>
              </div>

              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Mail className="h-4 w-4" />
                  <span>Email</span>
                </div>
                <p className="text-lg font-medium">
                  {user.email || "Not provided"}
                </p>
              </div>

              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Phone className="h-4 w-4" />
                  <span>Phone</span>
                </div>
                <p className="text-lg font-medium">
                  {user.phoneNumber || "Not provided"}
                </p>
              </div>

              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Calendar className="h-4 w-4" />
                  <span>Date of Birth</span>
                </div>
                <p className="text-lg font-medium">
                  {user.dateOfBirth
                    ? new Date(user.dateOfBirth).toLocaleDateString()
                    : "Not provided"}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <EditProfileModal
          open={isEditModalOpen}
          onOpenChange={(open) => setIsEditModalOpen(open)}
          user={user}
          onSave={async (data) => {
            await updateUser.mutateAsync(data);
          }}
        />
      </div>
    </div>
  );
}
