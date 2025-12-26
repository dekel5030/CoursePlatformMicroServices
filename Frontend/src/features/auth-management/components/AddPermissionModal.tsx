import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui';
import { Button, FormField, Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui';
import type { AddPermissionRequest } from '../types';
import type { ApiErrorResponse } from '@/api/axiosClient';

interface AddPermissionModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (permission: AddPermissionRequest) => Promise<void>;
  isLoading?: boolean;
}

export default function AddPermissionModal({
  open,
  onOpenChange,
  onSubmit,
  isLoading = false,
}: AddPermissionModalProps) {
  const [formData, setFormData] = useState<AddPermissionRequest>({
    effect: 'Allow',
    action: '',
    resource: '',
    resourceId: '*',
  });
  const [apiError, setApiError] = useState<ApiErrorResponse | null>(null);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev: AddPermissionRequest) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setApiError(null);

    try {
      await onSubmit(formData);
      setFormData({
        effect: 'Allow',
        action: '',
        resource: '',
        resourceId: '*',
      });
      setApiError(null);
      onOpenChange(false);
    } catch (err: unknown) {
      setApiError(err as ApiErrorResponse);
    }
  };

  const handleClose = () => {
    setFormData({
      effect: 'Allow',
      action: '',
      resource: '',
      resourceId: '*',
    });
    setApiError(null);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add Permission</DialogTitle>
        </DialogHeader>

        {apiError?.message && (
          <div className="bg-destructive/15 text-destructive px-4 py-2 rounded-md text-sm">
            {apiError.message}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <label htmlFor="effect" className="text-sm font-medium">
              Effect
            </label>
            <Select
              value={formData.effect}
              onValueChange={(value) => setFormData(prev => ({ ...prev, effect: value as 'Allow' | 'Deny' }))}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Allow">Allow</SelectItem>
                <SelectItem value="Deny">Deny</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <FormField
            label="Action"
            name="action"
            value={formData.action}
            onChange={handleChange}
            placeholder="e.g., Read, Write, Delete"
            error={apiError?.errors?.Action?.[0]}
            required
          />

          <FormField
            label="Resource"
            name="resource"
            value={formData.resource}
            onChange={handleChange}
            placeholder="e.g., Course, User, Order"
            error={apiError?.errors?.Resource?.[0]}
            required
          />

          <FormField
            label="Resource ID"
            name="resourceId"
            value={formData.resourceId}
            onChange={handleChange}
            placeholder="* for all, or specific ID"
            error={apiError?.errors?.ResourceId?.[0]}
          />

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Adding...' : 'Add Permission'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
