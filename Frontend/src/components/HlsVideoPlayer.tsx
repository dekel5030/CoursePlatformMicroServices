import "@vidstack/react/player/styles/default/theme.css";
import "@vidstack/react/player/styles/default/layouts/video.css";

import { MediaPlayer, MediaProvider, Track } from "@vidstack/react";
import {
  defaultLayoutIcons,
  DefaultVideoLayout,
} from "@vidstack/react/player/layouts/default";

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
  return (
    <div className="w-full aspect-video rounded-xl overflow-hidden shadow-lg bg-black">
      <MediaPlayer
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
