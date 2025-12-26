import { X } from 'lucide-react';
import { Badge, Button } from '@/components/ui';
import { cn } from '@/lib/utils';
import type { PermissionDto } from '../types';

interface PermissionBadgeProps {
  permission: PermissionDto;
  onRemove?: () => void;
  showRemove?: boolean;
}

export default function PermissionBadge({
  permission,
  onRemove,
  showRemove = false,
}: PermissionBadgeProps) {
  const isAllow = permission.effect.toLowerCase() === 'allow';

  return (
    <div className={cn(
      "flex items-center gap-2 p-3 rounded-md border",
      isAllow ? "border-green-200 bg-green-50" : "border-red-200 bg-red-50"
    )}>
      <div className="flex items-center gap-2 flex-wrap flex-1">
        <Badge variant={isAllow ? 'default' : 'destructive'} className="font-semibold">
          {permission.effect}
        </Badge>
        <span className="text-sm text-muted-foreground">:</span>
        <span className="text-sm font-medium">{permission.action}</span>
        <span className="text-sm text-muted-foreground">on</span>
        <span className="text-sm font-medium">{permission.resource}</span>
        {permission.resourceId !== '*' && (
          <>
            <span className="text-sm text-muted-foreground">/</span>
            <Badge variant="secondary" className="text-xs">
              {permission.resourceId}
            </Badge>
          </>
        )}
      </div>
      {showRemove && onRemove && (
        <Button
          variant="ghost"
          size="icon"
          className="h-6 w-6"
          onClick={onRemove}
          title="Remove permission"
          type="button"
        >
          <X className="h-4 w-4" />
        </Button>
      )}
    </div>
  );
}
