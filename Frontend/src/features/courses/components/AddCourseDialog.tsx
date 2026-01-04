import { useState } from "react";
import { toast } from "sonner";
import { useTranslation } from "react-i18next";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useCreateCourse } from "@/features/courses";

interface AddCourseDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function AddCourseDialog({ open, onOpenChange }: AddCourseDialogProps) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const { t, i18n } = useTranslation(["courses", "translation"]);

  const createCourseMutation = useCreateCourse();

  const handleClose = () => {
    setTitle("");
    setDescription("");
    onOpenChange(false);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!title.trim()) {
      toast.error(t("courses:addDialog.validationError"));
      return;
    }

    createCourseMutation.mutate(
      {
        title: title.trim(),
        description: description.trim() || undefined,
      },
      {
        onSuccess: () => {
          toast.success(t("courses:addDialog.successMessage"));
          handleClose();
        },
        onError: (error: Error) => {
          toast.error(
            t("courses:addDialog.errorMessage", { message: error.message })
          );
        },
      }
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[525px]" dir={i18n.dir()}>
        <DialogHeader dir={i18n.dir()}>
          <DialogTitle>{t("courses:addDialog.title")}</DialogTitle>
          <DialogDescription>
            {t("courses:addDialog.description")}
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} dir={i18n.dir()}>
          <div className="grid gap-4 py-4">
            <div className="space-y-2">
              <label
                htmlFor="title"
                className="text-sm font-medium leading-none"
                dir={i18n.dir()}
              >
                {t("courses:addDialog.titleLabel")}
                <span className="text-destructive ml-1">*</span>
              </label>
              <Input
                id="title"
                name="title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder={t("courses:addDialog.titlePlaceholder")}
                dir={i18n.dir()}
                required
              />
            </div>
            <div className="space-y-2">
              <label
                htmlFor="description"
                className="text-sm font-medium leading-none"
                dir={i18n.dir()}
              >
                {t("courses:addDialog.descriptionLabel")}
              </label>
              <textarea
                id="description"
                name="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder={t("courses:addDialog.descriptionPlaceholder")}
                dir={i18n.dir()}
                className="flex min-h-[100px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={createCourseMutation.isPending}
            >
              {t("courses:addDialog.cancel")}
            </Button>
            <Button type="submit" disabled={createCourseMutation.isPending}>
              {createCourseMutation.isPending
                ? t("courses:addDialog.submitting")
                : t("courses:addDialog.submit")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
