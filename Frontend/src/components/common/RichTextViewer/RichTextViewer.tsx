import ReactMarkdown from "react-markdown";
import rehypeRaw from "rehype-raw";
import { cn } from "@/lib/utils";
import type { Components } from "react-markdown";

interface RichTextViewerProps {
  content: string;
  className?: string;
}

/**
 * RichTextViewer Component
 *
 * Renders Markdown content with support for embedded HTML tags (like <kbd>).
 * Uses Tailwind Typography for professional styling.
 *
 * Features:
 * - Markdown rendering (bold, italic, lists, headings, etc.)
 * - HTML tag support (especially <kbd> for keyboard shortcuts)
 * - RTL/LTR auto-detection with dir="auto"
 * - Responsive prose styling
 */
export function RichTextViewer({ content, className }: RichTextViewerProps) {
  if (!content) {
    return null;
  }

  // Pre-process content to fix inline bullets
  // Convert bullets that are on the same line (• text • text) into separate lines
  const processedContent = content.replace(/([•])\s*/g, "\n$1 ").trim();

  // Custom components for better control
  const components: Components = {
    p: ({ children, ...props }) => (
      <p className="my-2 leading-relaxed" {...props}>
        {children}
      </p>
    ),
    ul: ({ children, ...props }) => (
      <ul className="my-2 space-y-1 list-disc list-inside" {...props}>
        {children}
      </ul>
    ),
    ol: ({ children, ...props }) => (
      <ol className="my-2 space-y-1 list-decimal list-inside" {...props}>
        {children}
      </ol>
    ),
    li: ({ children, ...props }) => (
      <li className="my-0.5" {...props}>
        {children}
      </li>
    ),
    strong: ({ children, ...props }) => (
      <strong className="font-semibold text-foreground" {...props}>
        {children}
      </strong>
    ),
    em: ({ children, ...props }) => (
      <em className="italic" {...props}>
        {children}
      </em>
    ),
  };

  return (
    <div
      className={cn(
        "prose prose-sm max-w-none",
        "prose-headings:font-semibold prose-headings:tracking-tight",
        "prose-p:leading-relaxed prose-p:text-muted-foreground",
        "prose-ul:text-muted-foreground prose-ol:text-muted-foreground",
        "prose-li:my-1",
        "prose-strong:text-foreground prose-strong:font-semibold",
        "dark:prose-invert",
        "text-muted-foreground",
        className,
      )}
      dir="auto"
    >
      <ReactMarkdown rehypePlugins={[rehypeRaw]} components={components}>
        {processedContent}
      </ReactMarkdown>
    </div>
  );
}
