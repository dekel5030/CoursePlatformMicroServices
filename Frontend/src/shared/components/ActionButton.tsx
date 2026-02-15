import { Button } from "@/shared/ui";
import { getLinkFromRecord } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";
import type { LinkRecord } from "@/shared/types/LinkRecord";

export interface ActionButtonProps {
  links?: LinksRecord;
  rel: string;
  label: string;
  onAction: (link: LinkRecord) => void | Promise<void>;
  variant?: "default" | "destructive" | "outline" | "secondary" | "ghost" | "link";
  size?: "default" | "sm" | "lg" | "icon";
  className?: string;
  disabled?: boolean;
  children?: React.ReactNode;
}

/**
 * Renders a button only when the given HATEOAS link exists.
 * Calls onAction with the link when clicked.
 */
export function ActionButton({
  links,
  rel,
  label,
  onAction,
  variant = "outline",
  size = "sm",
  className,
  disabled = false,
  children,
}: ActionButtonProps) {
  const link = getLinkFromRecord(links, rel);
  if (!link?.href) return null;

  const handleClick = () => {
    void onAction(link);
  };

  return (
    <Button
      variant={variant}
      size={size}
      className={className}
      disabled={disabled}
      onClick={handleClick}
    >
      {children ?? label}
    </Button>
  );
}
