import { useNavigate } from "react-router-dom";
import { User, LogOut, Settings, BookOpen } from "lucide-react";
import { useTranslation } from "react-i18next";
// ייבוא של useAuth מהמיקום החדש (feature/auth)
import { useAuth } from "@/features/auth";
import { useHasRole } from "@/hooks";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui";
import ProfileAvatar from "./ProfileAvatar";

export default function UserNav() {
  const navigate = useNavigate();
  const { t, i18n } = useTranslation();
  const { user, signoutRedirect } = useAuth();
  const isStaff = useHasRole(["Admin", "Instructor"]);

  if (!user) return null;

  const getInitial = () => {
    const namePart = user.firstName || user.email || "U";
    return namePart.charAt(0).toUpperCase();
  };

  const handleProfileClick = () => {
    if (user.id) {
      navigate(`/users/${user.id}`);
    } else {
      navigate(`/profile`);
    }
  };

  const handleMyCoursesClick = () => {
    navigate("/users/me/courses/enrolled");
  };

  const handleManagementClick = () => {
    navigate("/admin");
  };

  const handleLogout = async () => {
    try {
      await signoutRedirect();
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  // בניית שם התצוגה בצורה נקייה
  const displayName = user.firstName
    ? `${user.firstName} ${user.lastName || ""}`.trim()
    : user.email || "User";

  return (
    <DropdownMenu dir={i18n.dir()}>
      <DropdownMenuTrigger asChild>
        <button className="focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 rounded-full cursor-pointer">
          <ProfileAvatar initial={getInitial()} />
        </button>
      </DropdownMenuTrigger>

      <DropdownMenuContent align="end" className="w-56" forceMount>
        <DropdownMenuLabel className="font-normal">
          <div className="flex flex-col space-y-1">
            <p className="text-sm font-medium leading-none" dir="auto">
              {displayName}
            </p>
            <p className="text-xs leading-none text-muted-foreground" dir="ltr">
              {user.email}
            </p>
          </div>
        </DropdownMenuLabel>

        <DropdownMenuSeparator />

        <DropdownMenuItem onClick={handleProfileClick}>
          <User className="mr-2 h-4 w-4" />
          <span>{t("profileMenu.profile")}</span>
        </DropdownMenuItem>

        <DropdownMenuItem onClick={handleMyCoursesClick}>
          <BookOpen className="mr-2 h-4 w-4" />
          <span>{t("profileMenu.myCourses")}</span>
        </DropdownMenuItem>

        {isStaff && (
          <DropdownMenuItem onClick={handleManagementClick}>
            <Settings className="mr-2 h-4 w-4" />
            <span>{t("profileMenu.management")}</span>
          </DropdownMenuItem>
        )}

        <DropdownMenuSeparator />

        <DropdownMenuItem onClick={handleLogout}>
          <LogOut className="mr-2 h-4 w-4" />
          <span>{t("profileMenu.logout")}</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
