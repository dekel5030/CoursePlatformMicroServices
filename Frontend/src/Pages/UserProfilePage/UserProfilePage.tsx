import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { User } from "../../types/user";
import { fetchUserById } from "../../services/UsersAPI";
import { useAuth } from "../../features/auth/AuthContext";
import styles from "./UserProfilePage.module.css";

export default function UserProfilePage() {
  const { id } = useParams<{ id: string }>();
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Get current user from AuthContext
  const { currentUser } = useAuth();
  const isOwnProfile = currentUser?.userId === id;

  useEffect(() => {
    if (!id) return;

    setLoading(true);
    fetchUserById(id)
      .then((userData) => {
        setUser(userData);
        console.log(userData);
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [id]);

  const handleEditProfile = () => {
    // TODO: Navigate to edit profile page when implemented
    alert("Edit profile functionality to be implemented");
  };

  if (loading)
    return <div className={styles.status}>Loading user profile...</div>;
  if (error) return <div className={styles.statusError}>Error: {error}</div>;
  if (!user) return <div className={styles.statusError}>User not found</div>;

  const fullName =
    [user.firstName, user.lastName].filter(Boolean).join(" ") || "N/A";
  const formattedDate = user.dateOfBirth
    ? new Date(user.dateOfBirth).toLocaleDateString()
    : "N/A";

  return (
    <div className={styles.container}>
      <div className={styles.profileCard}>
        <div className={styles.header}>
          <div className={styles.avatar}>
            {(user.firstName?.[0] || user.email[0]).toUpperCase()}
          </div>
          <div className={styles.headerInfo}>
            <h1 className={styles.name}>{fullName}</h1>
            <p className={styles.email}>{user.email}</p>
          </div>
        </div>

        <div className={styles.divider} />

        <div className={styles.infoSection}>
          <h2 className={styles.sectionTitle}>Personal Information</h2>

          <div className={styles.infoGrid}>
            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>First Name</span>
              <span className={styles.infoValue}>
                {user.firstName || "N/A"}
              </span>
            </div>

            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>Last Name</span>
              <span className={styles.infoValue}>{user.lastName || "N/A"}</span>
            </div>

            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>Email</span>
              <span className={styles.infoValue}>{user.email}</span>
            </div>

            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>Phone Number</span>
              <span className={styles.infoValue}>
                {user.phoneNumber || "N/A"}
              </span>
            </div>

            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>Date of Birth</span>
              <span className={styles.infoValue}>{formattedDate}</span>
            </div>

            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>User ID</span>
              <span className={styles.infoValue}>{user.id}</span>
            </div>
          </div>
        </div>

        {isOwnProfile && (
          <div className={styles.actions}>
            <button onClick={handleEditProfile} className={styles.editButton}>
              Edit Profile
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
