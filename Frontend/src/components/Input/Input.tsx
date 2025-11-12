import styles from "./Input.module.css";

type InputProps = {
  label: string;
  type?: string;
  name: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  placeholder?: string;
  error?: string;
  required?: boolean;
};

export default function Input({
  label,
  type = "text",
  name,
  value,
  onChange,
  placeholder,
  error,
  required = false,
}: InputProps) {
  return (
    <div className={styles.container}>
      <label htmlFor={name} className={styles.label}>
        {label}
        {required && <span className={styles.required}> *</span>}
      </label>
      <input
        id={name}
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className={`${styles.input} ${error ? styles.inputError : ""}`}
        required={required}
      />
      {error && <span className={styles.errorMessage}>{error}</span>}
    </div>
  );
}
