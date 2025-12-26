import styles from "./Button.module.css";

type ButtonVariant = "filled" | "outlined";

type ButtonProps = {
  variant?: ButtonVariant;
  children: React.ReactNode;
  onClick?: () => void;
};

export default function Button({
  variant = "filled",
  children,
  onClick,
}: ButtonProps) {
  const className = variant === "filled" ? styles.filled : styles.outlined;
  return (
    <button className={className} onClick={onClick}>
      {children}
    </button>
  );
}
