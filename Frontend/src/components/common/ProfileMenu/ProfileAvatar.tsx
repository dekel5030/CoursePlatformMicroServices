import { Avatar, AvatarFallback } from "@/components/ui";

interface ProfileAvatarProps {
  initial: string;
  className?: string;
}

export default function ProfileAvatar({ initial, className }: ProfileAvatarProps) {
  return (
    <Avatar className={className}>
      <AvatarFallback className="bg-primary text-primary-foreground font-semibold">
        {initial}
      </AvatarFallback>
    </Avatar>
  );
}
