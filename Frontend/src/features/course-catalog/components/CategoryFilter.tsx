import { useTranslation } from "react-i18next";
import { Button } from "@/shared/ui";

interface CategoryFilterProps {
  categories: string[];
  selectedCategory: string | null;
  onSelectCategory: (category: string | null) => void;
}

export function CategoryFilter({ categories, selectedCategory, onSelectCategory }: CategoryFilterProps) {
  const { t } = useTranslation(['course-catalog', 'translation']);

  if (categories.length === 0) return null;

  return (
    <div
      className="flex overflow-x-auto pb-4 gap-2 scrollbar-none"
      aria-label="Categories"
    >
      <Button
        variant={!selectedCategory ? "default" : "secondary"}
        size="sm"
        onClick={() => onSelectCategory(null)}
        className="rounded-full px-6 transition-all"
      >
        {t('course-catalog:catalog.all')}
      </Button>
      {categories.map((cat) => (
        <Button
          key={cat}
          variant={selectedCategory === cat ? "default" : "secondary"}
          size="sm"
          onClick={() => onSelectCategory(cat)}
          className="rounded-full px-6 whitespace-nowrap transition-all"
        >
          {cat}
        </Button>
      ))}
    </div>
  );
}
