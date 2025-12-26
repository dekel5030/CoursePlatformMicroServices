import { useState, useRef, useEffect } from 'react';
import { X, ChevronDown } from 'lucide-react';
import styles from './MultiSelect.module.css';

interface Option {
  value: string;
  label: string;
}

interface MultiSelectProps {
  options: Option[];
  selected: string[];
  onChange: (selected: string[]) => void;
  placeholder?: string;
  disabled?: boolean;
}

export default function MultiSelect({
  options,
  selected,
  onChange,
  placeholder = 'Select...',
  disabled = false,
}: MultiSelectProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isOpen]);

  const filteredOptions = options.filter(
    (option) =>
      !selected.includes(option.value) &&
      option.label.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const selectedOptions = options.filter((option) => selected.includes(option.value));

  const handleSelect = (value: string) => {
    onChange([...selected, value]);
    setSearchTerm('');
  };

  const handleRemove = (value: string) => {
    onChange(selected.filter((v) => v !== value));
  };

  return (
    <div className={styles.container} ref={containerRef}>
      <div
        className={`${styles.trigger} ${disabled ? styles.disabled : ''} ${isOpen ? styles.open : ''}`}
        onClick={() => !disabled && setIsOpen(!isOpen)}
      >
        <div className={styles.selectedContainer}>
          {selectedOptions.length === 0 ? (
            <span className={styles.placeholder}>{placeholder}</span>
          ) : (
            selectedOptions.map((option) => (
              <span key={option.value} className={styles.selectedItem}>
                {option.label}
                <button
                  type="button"
                  className={styles.removeButton}
                  onClick={(e) => {
                    e.stopPropagation();
                    handleRemove(option.value);
                  }}
                >
                  <X size={14} />
                </button>
              </span>
            ))
          )}
        </div>
        <ChevronDown size={16} className={styles.chevron} />
      </div>

      {isOpen && (
        <div className={styles.dropdown}>
          <input
            type="text"
            className={styles.search}
            placeholder="Search..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            onClick={(e) => e.stopPropagation()}
          />
          <div className={styles.options}>
            {filteredOptions.length === 0 ? (
              <div className={styles.empty}>No options found</div>
            ) : (
              filteredOptions.map((option) => (
                <div
                  key={option.value}
                  className={styles.option}
                  onClick={() => handleSelect(option.value)}
                >
                  {option.label}
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}
