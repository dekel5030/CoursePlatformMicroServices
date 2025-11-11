import type { Lesson } from "./Lesson";
import type { Money } from "./money";
import type { IdValueObject } from "./idValueObject";

export interface Course {
  id: IdValueObject;
  title: string;
  description: string;
  imageUrl?: string | null;
  instructorUserId?: string | null;
  isPublished: boolean;
  price: Money;
  lessons?: Lesson[];
  updatedAtUtc: string;
}
