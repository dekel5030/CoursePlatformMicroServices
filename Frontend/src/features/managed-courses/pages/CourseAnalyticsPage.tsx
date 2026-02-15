import { Link, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import { Users, Eye, Star, BookOpen, ExternalLink, Settings } from "lucide-react";
import { useCourseAnalytics } from "@/domain/courses";
import { BreadcrumbNav } from "@/components/layout";
import { Button, Skeleton } from "@/shared/ui";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui";
import { formatDuration } from "@/shared/utils/format-duration";
import { getLinkFromRecord } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";
import type { CourseViewerDto } from "@/domain/courses/types";

function getInitials(displayName: string): string {
  const parts = displayName.trim().split(/\s+/).filter(Boolean);
  if (parts.length >= 2) {
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }
  return displayName.slice(0, 2).toUpperCase() || "?";
}

function formatViewedAt(viewedAt: string): string {
  try {
    const d = new Date(viewedAt);
    return new Intl.DateTimeFormat(undefined, {
      dateStyle: "short",
      timeStyle: "short",
    }).format(d);
  } catch {
    return viewedAt;
  }
}

export default function CourseAnalyticsPage() {
  const { t } = useTranslation("translation");
  const { id } = useParams<{ id: string }>();
  const { data: analytics, isLoading, error } = useCourseAnalytics(id);

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("managedCourses.title"), path: "/manage/courses" },
    { label: t("analytics.title") },
  ];

  if (isLoading) {
    return (
      <div className="flex flex-col">
        <BreadcrumbNav items={breadcrumbItems} />
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-8 w-full">
          <Skeleton className="h-10 w-64" />
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {[1, 2, 3, 4].map((i) => (
              <Skeleton key={i} className="h-32 rounded-lg" />
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col">
        <BreadcrumbNav items={breadcrumbItems} />
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <div className="bg-destructive/10 border border-destructive/20 text-destructive px-6 py-4 rounded-lg">
            {t("common.error", { message: error.message })}
          </div>
        </div>
      </div>
    );
  }

  if (!analytics) {
    return null;
  }

  const stats = [
    {
      label: t("analytics.enrollments"),
      value: analytics.enrollmentsCount.toLocaleString(),
      icon: Users,
    },
    {
      label: t("analytics.views"),
      value: analytics.viewCount.toLocaleString(),
      icon: Eye,
    },
    {
      label: t("analytics.rating"),
      value: analytics.averageRating.toFixed(1),
      subtitle: `${analytics.reviewsCount} ${t("analytics.reviews")}`,
      icon: Star,
    },
    {
      label: t("analytics.lessons"),
      value: analytics.totalLessonsCount.toLocaleString(),
      icon: BookOpen,
    },
  ];

  const chartData =
    analytics.enrollmentsOverTime?.length > 0
      ? analytics.enrollmentsOverTime.map((d) => ({
          date: new Date(d.date).toLocaleDateString(undefined, {
            month: "short",
            day: "numeric",
          }),
          count: d.count,
          fullDate: d.date,
        }))
      : [];

  const courseViewers: CourseViewerDto[] = analytics.courseViewers ?? [];
  const links = analytics.links as LinksRecord | undefined;
  const courseLink = links ? getLinkFromRecord(links, "course") : undefined;
  const managedCourseLink = links ? getLinkFromRecord(links, "managedCourse") : undefined;

  return (
    <div className="flex flex-col">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-8 w-full">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold tracking-tight">
              {t("analytics.title")}
            </h1>
            <p className="text-muted-foreground mt-1">
              {t("analytics.subtitle")}
            </p>
          </div>
          {(courseLink?.href || managedCourseLink?.href) && (
            <div className="flex flex-wrap gap-2">
              {courseLink?.href &&
                (courseLink.href.startsWith("http") ? (
                  <Button variant="outline" size="sm" asChild>
                    <a href={courseLink.href} target="_blank" rel="noopener noreferrer" className="gap-2 inline-flex items-center">
                      <ExternalLink className="h-4 w-4" />
                      {t("analytics.viewCourse", { defaultValue: "View course" })}
                    </a>
                  </Button>
                ) : (
                  <Button variant="outline" size="sm" asChild>
                    <Link to={courseLink.href} className="gap-2">
                      <ExternalLink className="h-4 w-4" />
                      {t("analytics.viewCourse", { defaultValue: "View course" })}
                    </Link>
                  </Button>
                ))}
              {managedCourseLink?.href &&
                (managedCourseLink.href.startsWith("http") ? (
                  <Button variant="outline" size="sm" asChild>
                    <a href={managedCourseLink.href} target="_blank" rel="noopener noreferrer" className="gap-2 inline-flex items-center">
                      <Settings className="h-4 w-4" />
                      {t("analytics.manageCourse", { defaultValue: "Manage course" })}
                    </a>
                  </Button>
                ) : (
                  <Button variant="outline" size="sm" asChild>
                    <Link to={managedCourseLink.href} className="gap-2">
                      <Settings className="h-4 w-4" />
                      {t("analytics.manageCourse", { defaultValue: "Manage course" })}
                    </Link>
                  </Button>
                ))}
            </div>
          )}
        </div>

        <section aria-label={t("analytics.overview")}>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {stats.map((stat) => {
              const Icon = stat.icon;
              return (
                <div
                  key={stat.label}
                  className="bg-card rounded-lg border p-6 shadow-sm flex flex-col"
                >
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-muted-foreground">
                      <Icon className="h-5 w-5" aria-hidden />
                    </span>
                  </div>
                  <div className="text-3xl font-bold">{stat.value}</div>
                  <div className="text-sm text-muted-foreground mt-1">
                    {stat.label}
                  </div>
                  {stat.subtitle && (
                    <div className="text-xs text-muted-foreground mt-1">
                      {stat.subtitle}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </section>

        {chartData.length > 0 && (
          <section>
            <h2 className="text-xl font-semibold mb-4">
              {t("analytics.enrollmentsOverTime")}
            </h2>
            <div className="bg-card rounded-lg border p-6 shadow-sm">
              <div className="h-64 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart
                    data={chartData}
                    margin={{ top: 10, right: 10, left: 0, bottom: 0 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                    <XAxis
                      dataKey="date"
                      tick={{ fontSize: 12 }}
                      className="text-muted-foreground"
                    />
                    <YAxis
                      allowDecimals={false}
                      tick={{ fontSize: 12 }}
                      className="text-muted-foreground"
                    />
                    <Tooltip
                      contentStyle={{
                        backgroundColor: "hsl(var(--card))",
                        border: "1px solid hsl(var(--border))",
                        borderRadius: "var(--radius)",
                      }}
                      labelStyle={{ color: "hsl(var(--foreground))" }}
                    />
                    <Area
                      type="monotone"
                      dataKey="count"
                      stroke="hsl(var(--primary))"
                      fill="hsl(var(--primary))"
                      fillOpacity={0.2}
                      name={t("analytics.enrollments")}
                    />
                  </AreaChart>
                </ResponsiveContainer>
              </div>
            </div>
          </section>
        )}

        {chartData.length === 0 &&
          analytics.enrollmentsOverTime &&
          analytics.enrollmentsOverTime.length === 0 && (
            <section>
              <h2 className="text-xl font-semibold mb-4">
                {t("analytics.enrollmentsOverTime")}
              </h2>
              <div className="bg-card rounded-lg border p-6 shadow-sm text-muted-foreground">
                {t("analytics.noData")}
              </div>
            </section>
          )}

        {analytics.moduleAnalytics && analytics.moduleAnalytics.length > 0 && (
          <section>
            <h2 className="text-xl font-semibold mb-4">
              {t("analytics.moduleBreakdown")}
            </h2>
            <div className="bg-card rounded-lg border shadow-sm overflow-hidden">
              <table className="w-full">
                <thead className="bg-muted/50">
                  <tr>
                    <th className="px-6 py-3 text-left text-sm font-medium">
                      {t("analytics.module")}
                    </th>
                    <th className="px-6 py-3 text-left text-sm font-medium">
                      {t("analytics.lessons")}
                    </th>
                    <th className="px-6 py-3 text-left text-sm font-medium">
                      {t("analytics.duration")}
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {analytics.moduleAnalytics.map((module, idx) => (
                    <tr key={module.moduleId}>
                      <td className="px-6 py-4 text-sm">
                        {t("analytics.module")} {idx + 1}
                      </td>
                      <td className="px-6 py-4 text-sm">
                        {module.lessonCount}
                      </td>
                      <td className="px-6 py-4 text-sm">
                        {formatDuration(module.totalDuration) ??
                          module.totalDuration}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </section>
        )}

        <section>
          <h2 className="text-xl font-semibold mb-4">
            {t("analytics.whoViewed")}
          </h2>
          <div className="bg-card rounded-lg border shadow-sm overflow-hidden">
            {courseViewers.length === 0 ? (
              <div className="px-6 py-8 text-center text-muted-foreground">
                {t("analytics.noViewers")}
              </div>
            ) : (
              <ul className="divide-y">
                {courseViewers.map((viewer) => (
                  <li
                    key={viewer.userId}
                    className="px-6 py-4 flex items-center gap-4"
                  >
                    <Avatar className="h-10 w-10 shrink-0">
                      {viewer.avatarUrl ? (
                        <AvatarImage
                          src={viewer.avatarUrl}
                          alt={viewer.displayName}
                        />
                      ) : null}
                      <AvatarFallback className="bg-muted text-muted-foreground font-medium">
                        {getInitials(viewer.displayName)}
                      </AvatarFallback>
                    </Avatar>
                    <div className="min-w-0 flex-1">
                      <p className="font-medium truncate">
                        {viewer.displayName}
                      </p>
                      <p className="text-sm text-muted-foreground">
                        {t("analytics.lastViewed")}:{" "}
                        {formatViewedAt(viewer.viewedAt)}
                      </p>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </section>
      </div>
    </div>
  );
}
