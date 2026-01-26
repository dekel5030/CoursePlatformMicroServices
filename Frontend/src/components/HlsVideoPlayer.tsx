import "@vidstack/react/player/styles/default/theme.css";
import "@vidstack/react/player/styles/default/layouts/video.css";

import { MediaPlayer, MediaProvider, Track } from "@vidstack/react";
import {
  defaultLayoutIcons,
  DefaultVideoLayout,
} from "@vidstack/react/player/layouts/default";
import { useEffect, useRef } from "react";

export type TranscriptTrack = {
  src: string;
  label: string;
  lang: string;
  isDefault?: boolean;
};

type VideoPlayerProps = {
  src: string;
  poster?: string;
  title?: string;
  transcripts?: TranscriptTrack[];
};

export function HlsVideoPlayer({
  src,
  poster,
  title,
  transcripts = [],
}: VideoPlayerProps) {
  const playerRef = useRef<any>(null);

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

  return (
    <div className="w-full aspect-video rounded-xl overflow-hidden shadow-lg bg-black">
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
}
