import { useState, useCallback, useRef, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useLesson, getLessonTranscript, putLessonTranscript } from "@/domain/lessons";
import type { TranscriptSegment } from "@/domain/lessons";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { BreadcrumbNav } from "@/components/layout";
import { HlsVideoPlayer, type HlsVideoPlayerHandle } from "@/components/HlsVideoPlayer";
import { getLinkFromRecord } from "@/shared/utils";
import { Button } from "@/shared/ui";
import { Card, CardContent, CardHeader } from "@/shared/ui";
import { ArrowLeft, Save, Search } from "lucide-react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";

export default function TranscriptEditorPage() {
  const { courseId, lessonId } = useParams<{ courseId: string; lessonId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { t } = useTranslation(["lesson-viewer", "translation"]);

  const manageLessonUrl = `/manage/lessons/${lessonId}`;
  const { data: lesson, isLoading: lessonLoading } = useLesson(
    courseId!,
    lessonId!,
    manageLessonUrl,
  );

  const transcriptHref = getLinkFromRecord(lesson?.links, "manageTranscript")?.href ?? null;

  const {
    data: initialSegments,
    isLoading: transcriptLoading,
    error: transcriptError,
  } = useQuery({
    queryKey: ["lesson-transcript", transcriptHref],
    queryFn: () => getLessonTranscript(transcriptHref!),
    enabled: !!transcriptHref,
  });

  const [segments, setSegments] = useState<TranscriptSegment[]>([]);
  const [currentTime, setCurrentTime] = useState(0);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [dirty, setDirty] = useState(false);
  const [findText, setFindText] = useState("");
  const [replaceText, setReplaceText] = useState("");
  const [showSearch, setShowSearch] = useState(false);
  const playerRef = useRef<HlsVideoPlayerHandle>(null);
  const activeSegmentRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (initialSegments && !dirty) {
      setSegments(initialSegments);
    }
  }, [initialSegments, dirty]);

  const saveMutation = useMutation({
    mutationFn: () => putLessonTranscript(transcriptHref!, segments),
    onSuccess: () => {
      setDirty(false);
      toast.success(t("lesson-viewer:transcript.saved", { defaultValue: "Transcript saved." }));
      queryClient.invalidateQueries({ queryKey: ["lesson-transcript", transcriptHref] });
    },
    onError: () => {
      toast.error(t("common.error", { message: "Failed to save transcript" }));
    },
  });

  const handleSave = useCallback(() => {
    if (!transcriptHref || !dirty) return;
    saveMutation.mutate();
  }, [transcriptHref, dirty, saveMutation]);

  const handleSeek = useCallback((seconds: number) => {
    playerRef.current?.seekTo(seconds);
  }, []);

  const handleSegmentClick = useCallback(
    (seg: TranscriptSegment) => {
      if (editingId === seg.id) return;
      handleSeek(seg.startTime);
    },
    [editingId, handleSeek],
  );

  const handleSegmentDoubleClick = useCallback((seg: TranscriptSegment) => {
    setEditingId(seg.id);
  }, []);

  const handleSegmentEdit = useCallback(
    (id: number, newText: string) => {
      setSegments((prev) =>
        prev.map((s) => (s.id === id ? { ...s, text: newText } : s)),
      );
      setDirty(true);
      setEditingId(null);
    },
    [],
  );

  const handleReplaceAll = useCallback(() => {
    if (!findText) return;
    setSegments((prev) =>
      prev.map((s) => ({
        ...s,
        text: s.text.replaceAll(findText, replaceText),
      })),
    );
    setDirty(true);
    setFindText("");
    setReplaceText("");
    setShowSearch(false);
  }, [findText, replaceText]);

  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      const tag = (e.target as HTMLElement)?.tagName;
      const isInput = ["INPUT", "TEXTAREA", "BUTTON"].includes(tag);
      if (e.key === " " && !isInput) {
        e.preventDefault();
        const video = document.querySelector("video");
        if (video) {
          if (video.paused) video.play();
          else video.pause();
        }
      }
      if (e.ctrlKey && e.key === "s") {
        e.preventDefault();
        handleSave();
      }
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [handleSave]);

  useEffect(() => {
    if (activeSegmentRef.current) {
      activeSegmentRef.current.scrollIntoView({ block: "nearest", behavior: "smooth" });
    }
  }, [currentTime]);

  if (lessonLoading || !courseId || !lessonId) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="h-8 w-48 bg-muted animate-pulse rounded" />
        <div className="mt-6 grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2 aspect-video bg-muted animate-pulse rounded-xl" />
          <div className="h-96 bg-muted animate-pulse rounded-xl" />
        </div>
      </div>
    );
  }

  if (!lesson) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-8">
        <p className="text-destructive">Lesson not found.</p>
        <Button variant="link" onClick={() => navigate(-1)}>
          {t("common.back", { defaultValue: "Back" })}
        </Button>
      </div>
    );
  }

  if (!transcriptHref) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-8">
        <p className="text-muted-foreground">
          {t("lesson-viewer:transcript.notAvailable", {
            defaultValue: "Transcript is not available for this lesson.",
          })}
        </p>
        <Button
          variant="outline"
          onClick={() =>
            navigate(`/manage/courses/${courseId}/lessons/${lessonId}`)
          }
        >
          <ArrowLeft className="me-2 h-4 w-4" />
          {t("common.back", { defaultValue: "Back" })}
        </Button>
      </div>
    );
  }

  if (transcriptError) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-8">
        <p className="text-destructive">
          {t("lesson-viewer:transcript.loadFailed", {
            defaultValue: "Failed to load transcript.",
          })}
        </p>
        <Button variant="outline" onClick={() => navigate(-1)}>
          {t("common.back", { defaultValue: "Back" })}
        </Button>
      </div>
    );
  }

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses"), path: "/catalog" },
    {
      label: lesson.title,
      path: `/manage/courses/${courseId}/lessons/${lessonId}`,
    },
    { label: t("lesson-viewer:transcript.editor", { defaultValue: "Edit transcript" }) },
  ];

  return (
    <div className="space-y-4">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="flex flex-wrap items-center gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() =>
            navigate(`/manage/courses/${courseId}/lessons/${lessonId}`)
          }
        >
          <ArrowLeft className="me-2 h-4 w-4" />
          {t("common.back", { defaultValue: "Back" })}
        </Button>
        <Button
          variant="default"
          size="sm"
          disabled={!dirty || saveMutation.isPending}
          onClick={handleSave}
        >
          <Save className="me-2 h-4 w-4" />
          {t("common.save", { defaultValue: "Save" })}
          {dirty && " *"}
        </Button>
        <Button
          variant="ghost"
          size="sm"
          onClick={() => setShowSearch((s) => !s)}
        >
          <Search className="me-2 h-4 w-4" />
          {t("lesson-viewer:transcript.searchReplace", {
            defaultValue: "Find & replace",
          })}
        </Button>
        <span className="text-xs text-muted-foreground ms-2">
          {t("lesson-viewer:transcript.shortcuts", {
            defaultValue: "Space: Play/Pause · Ctrl+S: Save",
          })}
        </span>
      </div>

      {showSearch && (
        <Card>
          <CardContent className="pt-4 flex flex-wrap gap-2 items-end">
            <label className="flex flex-col gap-1">
              <span className="text-sm">{t("lesson-viewer:transcript.find", { defaultValue: "Find" })}</span>
              <input
                type="text"
                value={findText}
                onChange={(e) => setFindText(e.target.value)}
                className="border rounded px-2 py-1 w-48"
                placeholder={t("lesson-viewer:transcript.find", { defaultValue: "Find" })}
              />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-sm">{t("lesson-viewer:transcript.replace", { defaultValue: "Replace" })}</span>
              <input
                type="text"
                value={replaceText}
                onChange={(e) => setReplaceText(e.target.value)}
                className="border rounded px-2 py-1 w-48"
                placeholder={t("lesson-viewer:transcript.replace", { defaultValue: "Replace" })}
              />
            </label>
            <Button size="sm" onClick={handleReplaceAll} disabled={!findText}>
              {t("lesson-viewer:transcript.replaceAll", { defaultValue: "Replace all" })}
            </Button>
          </CardContent>
        </Card>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <Card className="overflow-hidden border-0 shadow-lg bg-black">
            <CardContent className="p-0">
              <HlsVideoPlayer
                ref={playerRef}
                src={lesson.videoUrl ?? ""}
                poster={lesson.thumbnailImage ?? undefined}
                title={lesson.title}
                transcripts={
                  lesson.transcriptUrl
                    ? [
                        {
                          src: lesson.transcriptUrl,
                          label: "Transcript",
                          lang: "en",
                          isDefault: true,
                        },
                      ]
                    : []
                }
                onCurrentTimeChange={setCurrentTime}
              />
            </CardContent>
          </Card>
        </div>

        <Card className="flex flex-col max-h-[70vh]">
          <CardHeader className="py-2 text-sm text-muted-foreground">
            {t("lesson-viewer:transcript.clickToSeek", {
              defaultValue: "Click to seek · Double-click to edit",
            })}
          </CardHeader>
          <CardContent className="flex-1 overflow-y-auto p-2">
            {transcriptLoading ? (
              <div className="animate-pulse space-y-2">
                {[1, 2, 3, 4, 5].map((i) => (
                  <div key={i} className="h-12 bg-muted rounded" />
                ))}
              </div>
            ) : (
              <div className="space-y-1">
                {segments.map((seg) => {
                  const isActive =
                    currentTime >= seg.startTime && currentTime <= seg.endTime;
                  const isEditing = editingId === seg.id;
                  return (
                    <div
                      key={seg.id}
                      ref={isActive ? activeSegmentRef : undefined}
                      onClick={() => handleSegmentClick(seg)}
                      onDoubleClick={() => handleSegmentDoubleClick(seg)}
                      className={`rounded px-2 py-1.5 cursor-pointer text-sm ${
                        isActive
                          ? "bg-primary/20 ring-1 ring-primary"
                          : "hover:bg-muted"
                      } ${isEditing ? "ring-2 ring-primary" : ""}`}
                    >
                      {isEditing ? (
                        <input
                          type="text"
                          defaultValue={seg.text}
                          autoFocus
                          className="w-full bg-background border rounded px-2 py-1 text-sm"
                          onBlur={(e) => {
                            const v = e.target.value.trim();
                            if (v !== seg.text) handleSegmentEdit(seg.id, v);
                            setEditingId(null);
                          }}
                          onKeyDown={(e) => {
                            if (e.key === "Enter") {
                              const v = (e.target as HTMLInputElement).value.trim();
                              if (v !== seg.text) handleSegmentEdit(seg.id, v);
                              setEditingId(null);
                            }
                            if (e.key === "Escape") setEditingId(null);
                          }}
                        />
                      ) : (
                        <span>{seg.text}</span>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
