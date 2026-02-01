export interface DurationUnitLabels {
  hour: string;
  minute: string;
  second: string;
}


import i18n from 'i18next';

export function formatDuration(duration: string | null | undefined): string | null {
  if (!duration) return null;

  const [timePart] = duration.split(".");
  const parts = timePart.split(":");
  if (parts.length < 3) return null;

  const hours = parseInt(parts[0], 10) || 0;
  const minutes = parseInt(parts[1], 10) || 0;
  const seconds = parseInt(parts[2], 10) || 0;

  if (hours === 0 && minutes === 0 && seconds === 0) return null;

  const segments: string[] = [];

  if (hours > 0) {
    segments.push(`${hours} ${i18n.t('time.hour', { count: hours })}`);
    
    if (minutes > 0) {
      segments.push(`${minutes} ${i18n.t('time.minute', { count: minutes })}`);
    }
  } else {
    if (minutes > 0) {
      segments.push(`${minutes} ${i18n.t('time.minute', { count: minutes })}`);
    }
    
    if (seconds > 0 || minutes === 0) {
      segments.push(`${seconds} ${i18n.t('time.second', { count: seconds })}`);
    }
  }

  return segments.join(" ");
}