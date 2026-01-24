import { Button, type ButtonProps } from "@/components/ui/button";
import type { LinkDto } from "@/types/LinkDto";
import { hasLink, getLink } from "@/utils/linkHelpers";
import type { LucideIcon } from "lucide-react";

interface HateoasButtonProps extends Omit<ButtonProps, "onClick"> {
  links: LinkDto[] | undefined;
  rel: string;
  icon?: LucideIcon;
  children: React.ReactNode;
  onClick: (link: LinkDto) => void | Promise<void>;
  hideIfDisabled?: boolean;
}

/**
 * HateoasButton: A reusable button component that renders based on HATEOAS links
 *
 * This component checks if a specific link relation exists in the provided links array
 * and renders a button accordingly. If the link doesn't exist and hideIfDisabled is true,
 * the button won't be rendered at all.
 *
 * @example
 * ```tsx
 * <HateoasButton
 *   links={course.links}
 *   rel="delete"
 *   icon={Trash2}
 *   onClick={(link) => handleDelete(link.href)}
 *   variant="destructive"
 *   hideIfDisabled
 * >
 *   Delete Course
 * </HateoasButton>
 * ```
 */
export function HateoasButton({
  links,
  rel,
  icon: Icon,
  children,
  onClick,
  hideIfDisabled = false,
  ...buttonProps
}: HateoasButtonProps) {
  const link = getLink(links, rel);
  const isAvailable = hasLink(links, rel);

  // If no link and we should hide, don't render anything
  if (!isAvailable && hideIfDisabled) {
    return null;
  }

  const handleClick = () => {
    if (link) {
      onClick(link);
    }
  };

  return (
    <Button onClick={handleClick} disabled={!isAvailable} {...buttonProps}>
      {Icon && <Icon className="h-4 w-4" />}
      {children}
    </Button>
  );
}
