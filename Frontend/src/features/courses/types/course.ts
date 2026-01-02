import type { Lesson } from "@/features/lessons/types/Lesson";
import type { Money } from "./money";

export interface Course {
  id: string;
  title: string;
  description: string;
  imageUrl?: string | null;
  instructorUserId?: string | null;
  isPublished: boolean;
  price: Money;
  lessons?: Lesson[];
  updatedAtUtc: string;
}
