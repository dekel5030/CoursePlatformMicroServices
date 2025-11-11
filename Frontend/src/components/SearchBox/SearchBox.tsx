import { useState } from "react";
import styles from "./SearchBox.module.css";

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
    <form className={styles.searchForm} onSubmit={handleSubmit}>
      <button type="submit" className={styles.searchBtn} disabled={!query}>
        🔍
      </button>
      <input
        type="text"
        className={styles.searchInput}
        placeholder={placeholder}
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        autoComplete="off"
      />
    </form>
  );
}
