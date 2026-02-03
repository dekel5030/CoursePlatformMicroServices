export const enrollmentsQueryKeys = {
  all: ["enrollments"] as const,
  myEnrollments: (pageNumber: number, pageSize: number) =>
    [...enrollmentsQueryKeys.all, "me", pageNumber, pageSize] as const,
};
