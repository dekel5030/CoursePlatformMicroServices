/**
 * Transcript segment (matches backend TranscriptSegmentDto).
 */
export interface TranscriptSegment {
  id: number;
  startTime: number;
  endTime: number;
  text: string;
}
