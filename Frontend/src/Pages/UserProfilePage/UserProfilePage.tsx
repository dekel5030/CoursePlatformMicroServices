import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  type User,
  type UpdateUserRequest,
  fetchUserById,
  updateUser,
} from "@/services";
import { EditProfileModal } from "@/components";
import styles from "./UserProfilePage.module.css";
import { useAuth } from "react-oidc-context";

export default function UserProfilePage() {
  const { id } = useParams<{ id: string }>();
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const auth = useAuth();

  const isOwnProfile = auth.user?.profile.sub === id;

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    fetchUserById(id)
      .then((userData) => setUser(userData))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [id]);

  const handleEditProfile = () => {
    setIsEditModalOpen(true);
  };

  const handleSaveProfile = async (updatedData: UpdateUserRequest) => {
    if (!id) return;
    try {
      const updatedUser = await updateUser(id, updatedData);
      setUser(updatedUser);
    } catch (err) {
      throw new Error(err instanceof Error ? err.message : "Failed to update");
    }
  };

  if (loading) return <div className={styles.status}>Loading...</div>;
  if (error) return <div className={styles.statusError}>Error: {error}</div>;
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
