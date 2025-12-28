import type { ReactNode } from "react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/components/ui";

interface DropdownProps {
  label: string;
  children: ReactNode;
}

export default function Dropdown({ label, children }: DropdownProps) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button className="px-3 py-2 text-sm font-medium hover:text-primary transition-colors cursor-pointer">
          {label}
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        {children}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
