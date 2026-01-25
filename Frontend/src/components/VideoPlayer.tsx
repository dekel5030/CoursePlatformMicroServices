import "@vidstack/react/player/styles/default/theme.css";
import "@vidstack/react/player/styles/default/layouts/video.css";

import { MediaPlayer, MediaProvider } from "@vidstack/react";
import {
  defaultLayoutIcons,
  DefaultVideoLayout,
} from "@vidstack/react/player/layouts/default";

type VideoPlayerProps = {
  src: string;
  poster?: string;
  title?: string;
};

export function HlsVideoPlayer({ src, poster, title }: VideoPlayerProps) {
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
        <MediaProvider />

        <DefaultVideoLayout icons={defaultLayoutIcons} thumbnails={null} />
      </MediaPlayer>
    </div>
  );
}
