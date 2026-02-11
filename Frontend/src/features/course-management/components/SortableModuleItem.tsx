import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { ModuleCard } from "./ModuleCard";
import type { ModuleModel } from "@/domain/courses";

interface SortableModuleItemProps {
  module: ModuleModel;
  courseId: string;
  index: number;
}

const MODULE_PREFIX = "module-";

export function getModuleId(id: string): string {
  return id.startsWith(MODULE_PREFIX) ? id.slice(MODULE_PREFIX.length) : id;
}

export function moduleSortableId(moduleId: string): string {
  return MODULE_PREFIX + moduleId;
}

export function SortableModuleItem({
  module,
  courseId,
  index,
}: SortableModuleItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: moduleSortableId(module.id),
    data: { type: "module" as const, moduleId: module.id },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <div ref={setNodeRef} style={style} className={isDragging ? "opacity-50" : ""}>
      <ModuleCard
        module={module}
        courseId={courseId}
        index={index}
        dragHandleProps={{ ...attributes, ...listeners }}
      />
    </div>
  );
}
