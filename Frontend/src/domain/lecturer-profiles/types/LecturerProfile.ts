export interface LecturerProfile {
  id: string;
  userId: string;
  professionalBio?: string | null;
  expertise?: string | null;
  yearsOfExperience: number;
  projects: Project[];
  mediaItems: MediaItem[];
  posts: Post[];
}

export interface Project {
  id: string;
  title: string;
  description?: string | null;
  url?: string | null;
  thumbnailUrl?: string | null;
  createdAt: string;
}

export interface MediaItem {
  id: string;
  url: string;
  title?: string | null;
  description?: string | null;
  type: 'Image' | 'Video' | 'Document';
  uploadedAt: string;
}

export interface Post {
  id: string;
  title: string;
  content: string;
  thumbnailUrl?: string | null;
  publishedAt: string;
  updatedAt?: string | null;
}

export interface CreateLecturerProfileRequest {
  professionalBio?: string | null;
  expertise?: string | null;
  yearsOfExperience: number;
}
