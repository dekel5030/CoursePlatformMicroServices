export {
  fetchFeaturedCourses,
  fetchCourseById,
  fetchLessonById,
  type Fetcher as CoursesFetcher,
} from './CoursesAPI';
export {
  fetchUserById,
  updateUser,
  type User,
  type UpdateUserRequest,
  type Fetcher as UsersFetcher,
} from './UsersAPI';
