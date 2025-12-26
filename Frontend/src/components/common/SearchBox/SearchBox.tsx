import { useState } from "react";
import { Search } from "lucide-react";
import { Input } from "@/components/ui";
import { Button } from "@/components/ui";

interface SearchBoxProps {
  placeholder?: string;
  onSearch?: (query: string) => void;
}

export default function SearchBox({
  placeholder = "Search for anything",
  onSearch,
}: SearchBoxProps) {
  const [query, setQuery] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (onSearch) onSearch(query);
  };

  return (
    <form onSubmit={handleSubmit} className="flex gap-2 w-full max-w-sm">
      <div className="relative flex-1">
        <Input
          type="text"
          placeholder={placeholder}
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          autoComplete="off"
          className="pr-10"
        />
      </div>
      <Button type="submit" size="icon" disabled={!query}>
        <Search className="h-4 w-4" />
      </Button>
    </form>
  );
}
