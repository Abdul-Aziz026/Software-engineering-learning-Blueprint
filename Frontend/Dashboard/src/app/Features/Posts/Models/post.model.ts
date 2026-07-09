export type PostStatus = 'Pending' | 'Published' | 'Rejected';

export interface PostSummary {
  id: string;
  title: string;
  summary: string;
  authorId: string;
  authorUsername: string;
  createdAt: string;
  updatedAt?: string | null;
  publishedAt?: string | null;
  status: PostStatus;
  likeCount: number;
  commentCount: number;
}

export interface PostComment {
  id: string;
  postId: string;
  authorId: string;
  authorUsername: string;
  content: string;
  createdAt: string;
}

export interface PostDetail {
  id: string;
  title: string;
  content: string;
  summary: string;
  authorId: string;
  authorUsername: string;
  createdAt: string;
  updatedAt?: string | null;
  publishedAt?: string | null;
  status: PostStatus;
  likeCount: number;
  commentCount: number;
  likedByCurrentUser: boolean;
  comments: PostComment[];
}

export interface CreatePostRequest {
  title: string;
  content: string;
  summary?: string;
}

export interface UpdatePostRequest {
  title: string;
  content: string;
  summary?: string;
}

export interface AddCommentRequest {
  content: string;
}

export interface ToggleLikeResponse {
  liked: boolean;
  likeCount: number;
}

export interface CreatePostResponse {
  id: string;
}
