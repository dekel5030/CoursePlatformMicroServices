import { useNavigate } from "react-router-dom";
import { useAuth } from "react-oidc-context";
import { User, LogOut } from "lucide-react";
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
  const auth = useAuth();

  const userProfile = auth.user?.profile;

  const getInitial = () => {
    if (userProfile?.given_name)
      return userProfile.given_name.charAt(0).toUpperCase();
    if (userProfile?.email) return userProfile.email.charAt(0).toUpperCase();
    return "U";
  };

  const handleProfileClick = () => {
    if (userProfile?.sub) {
      navigate(`/users/${userProfile.sub}`);
    }
  };

  const handleLogout = async () => {
    try {
      await auth.signoutRedirect();
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  const displayName = userProfile?.given_name || userProfile?.email || "User";

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button className="focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 rounded-full">
          <ProfileAvatar initial={getInitial()} />
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-56">
        <DropdownMenuLabel className="font-normal">
          <div className="flex flex-col space-y-1">
            <p className="text-sm font-medium leading-none">{displayName}</p>
            {userProfile?.email && (
              <p className="text-xs leading-none text-muted-foreground">
                {userProfile.email}
              </p>
            )}
          </div>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={handleProfileClick}>
          <User className="mr-2 h-4 w-4" />
          <span>Profile Page</span>
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={handleLogout}>
          <LogOut className="mr-2 h-4 w-4" />
          <span>Logout</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
