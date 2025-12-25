import { useState } from "react";
import { useParams } from "react-router-dom";
import type { UpdateUserRequest } from "@/services/UsersAPI";
import { EditProfileModal } from "@/components/common";
import styles from "./UserProfilePage.module.css";
import { useAuth } from "react-oidc-context";
import { useUser, useUpdateUser } from "@/features/users";

export default function UserProfilePage() {
  const { id } = useParams<{ id: string }>();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const auth = useAuth();
  const { data: user, isLoading, error } = useUser(id);
  const updateUserMutation = useUpdateUser(id || '');

  const isOwnProfile = auth.user?.profile.sub === id;

  const handleEditProfile = () => {
    setIsEditModalOpen(true);
  };

  const handleSaveProfile = async (updatedData: UpdateUserRequest) => {
    try {
      await updateUserMutation.mutateAsync(updatedData);
    } catch (err) {
      throw new Error(err instanceof Error ? err.message : "Failed to update");
    }
  };

  if (isLoading) return <div className={styles.status}>Loading...</div>;
  if (error) return <div className={styles.statusError}>Error: {error.message}</div>;
  if (!user) return <div className={styles.statusError}>User not found</div>;

  const fullName =
    [user.firstName, user.lastName].filter(Boolean).join(" ") || "N/A";

  return (
    <div className={styles.container}>
      <div className={styles.profileCard}>
        <div className={styles.header}>
          <h1 className={styles.name}>{fullName}</h1>
          <p className={styles.email}>{user.email}</p>
        </div>

        {isOwnProfile && (
          <div className={styles.actions}>
            <button onClick={handleEditProfile} className={styles.editButton}>
              Edit Profile
            </button>
          </div>
        )}
      </div>

      {user && (
        <EditProfileModal
          isOpen={isEditModalOpen}
          onClose={() => setIsEditModalOpen(false)}
          user={user}
          onSave={handleSaveProfile}
        />
      )}
    </div>
  );
}
