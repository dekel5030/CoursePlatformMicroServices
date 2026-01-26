import ReactMarkdown from "react-markdown";
import rehypeRaw from "rehype-raw";
import { cn } from "@/lib/utils";

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
        className,
      )}
      dir="auto"
    >
      <ReactMarkdown
        rehypePlugins={[rehypeRaw]}
        components={{
          // Customize rendering of specific elements if needed
          p: ({ children }) => <p className="my-2">{children}</p>,
          ul: ({ children }) => <ul className="my-2 space-y-1">{children}</ul>,
          ol: ({ children }) => <ol className="my-2 space-y-1">{children}</ol>,
        }}
      >
        {content}
      </ReactMarkdown>
    </div>
  );
}
