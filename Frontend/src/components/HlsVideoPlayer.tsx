import "@vidstack/react/player/styles/default/theme.css";
import "@vidstack/react/player/styles/default/layouts/video.css";

import { MediaPlayer, MediaProvider, Track } from "@vidstack/react";
import {
  defaultLayoutIcons,
  DefaultVideoLayout,
} from "@vidstack/react/player/layouts/default";
import { forwardRef, useEffect, useImperativeHandle, useRef } from "react";

export type TranscriptTrack = {
  src: string;
  label: string;
  lang: string;
  isDefault?: boolean;
};

/** Progress report interval in seconds (throttle PATCH calls) */
const PROGRESS_INTERVAL_SEC = 15;

export interface HlsVideoPlayerHandle {
  seekTo: (seconds: number) => void;
}

type VideoPlayerProps = {
  src: string;
  poster?: string;
  title?: string;
  transcripts?: TranscriptTrack[];
  /** Called periodically with current playback position in seconds (throttled). */
  onTimeUpdate?: (seconds: number) => void;
  /** Called frequently with current time (e.g. for transcript sync). Throttled to ~4 FPS. */
  onCurrentTimeChange?: (seconds: number) => void;
  /** Called when playback reaches the end. */
  onEnded?: () => void;
  /** Optional initial time in seconds (e.g. for resume). */
  initialTime?: number;
};

export const HlsVideoPlayer = forwardRef<HlsVideoPlayerHandle, VideoPlayerProps>(function HlsVideoPlayer(
  {
    src,
    poster,
    title,
    transcripts = [],
    onTimeUpdate,
    onCurrentTimeChange,
    onEnded,
    initialTime,
  },
  ref,
) {
  const playerRef = useRef<any>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const lastReportedSecondsRef = useRef(0);
  const lastCurrentTimeRef = useRef(0);
  const hasFiredEndedRef = useRef(false);

  useImperativeHandle(ref, () => ({
    seekTo(seconds: number) {
      const video = containerRef.current?.querySelector("video");
      if (video) video.currentTime = seconds;
    },
  }), []);

  useEffect(() => {
    // Custom styling for subtitles/captions
    const style = document.createElement("style");
    style.textContent = `
      /* Customize subtitle appearance */
      video::cue {
        font-size: 1.1em;
        line-height: 1.4;
        background-color: rgba(0, 0, 0, 0.75);
        color: white;
        padding: 0.2em 0.5em;
        border-radius: 4px;
        /* Limit max width to prevent long lines */
        max-width: 80%;
        text-align: center;
        /* Better text shadow for readability */
        text-shadow: 
          1px 1px 2px rgba(0, 0, 0, 0.9),
          -1px -1px 2px rgba(0, 0, 0, 0.9),
          1px -1px 2px rgba(0, 0, 0, 0.9),
          -1px 1px 2px rgba(0, 0, 0, 0.9);
      }
      
      /* Adjust subtitle position - slightly higher to not cover too much */
      .vds-captions {
        bottom: 8% !important;
      }
    `;
    document.head.appendChild(style);

    return () => {
      document.head.removeChild(style);
    };
  }, []);

  // Attach to native video for timeupdate and ended (throttled progress, one-shot ended)
  useEffect(() => {
    if (!onTimeUpdate && !onEnded) return;
    const container = containerRef.current;
    if (!container) return;

    const findVideo = (): HTMLVideoElement | null =>
      container.querySelector("video");

    const onTimeUpdateEvent = () => {
      const video = findVideo();
      if (!video) return;
      const now = video.currentTime;
      const sec = Math.floor(now);
      if (onTimeUpdate && sec - lastReportedSecondsRef.current >= PROGRESS_INTERVAL_SEC) {
        lastReportedSecondsRef.current = sec;
        onTimeUpdate(sec);
      }
      if (onCurrentTimeChange && now - lastCurrentTimeRef.current >= 0.25) {
        lastCurrentTimeRef.current = now;
        onCurrentTimeChange(now);
      }
    };

    const onEndedEvent = () => {
      if (!onEnded || hasFiredEndedRef.current) return;
      hasFiredEndedRef.current = true;
      onEnded();
    };

    let video: HTMLVideoElement | null = findVideo();
    let observer: MutationObserver | null = null;

    const attach = (v: HTMLVideoElement) => {
      v.addEventListener("timeupdate", onTimeUpdateEvent);
      v.addEventListener("ended", onEndedEvent);
    };
    const detach = (v: HTMLVideoElement) => {
      v.removeEventListener("timeupdate", onTimeUpdateEvent);
      v.removeEventListener("ended", onEndedEvent);
    };

    if (video) {
      attach(video);
      return () => detach(video!);
    }

    observer = new MutationObserver(() => {
      const v = findVideo();
      if (v && !video) {
        video = v;
        attach(v);
        observer?.disconnect();
        observer = null;
      }
    });
    observer.observe(container, { childList: true, subtree: true });

    return () => {
      observer?.disconnect();
      if (video) detach(video);
    };
  }, [onTimeUpdate, onCurrentTimeChange, onEnded]);

  return (
    <div
      ref={containerRef}
      className="w-full aspect-video rounded-xl overflow-hidden shadow-lg bg-black"
    >
      <MediaPlayer
        ref={playerRef}
        title={title}
        src={src}
        poster={poster}
        viewType="video"
        streamType="on-demand"
        logLevel="warn"
        crossOrigin
        playsInline
        className="w-full h-full"
        currentTime={initialTime}
      >
        <MediaProvider>
          {transcripts.map((track) => (
            <Track
              key={track.lang}
              src={track.src}
              kind="subtitles"
              label={track.label}
              lang={track.lang}
              default={track.isDefault}
            />
          ))}
        </MediaProvider>

        <DefaultVideoLayout icons={defaultLayoutIcons} thumbnails={null} />
      </MediaPlayer>
    </div>
  );
});
