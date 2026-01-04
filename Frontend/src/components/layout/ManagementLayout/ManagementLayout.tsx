import { Outlet, Link, useLocation } from "react-router-dom";
import { useState } from "react";
import {
  Users,
  Shield,
  Menu,
  X,
  LayoutDashboard,
  BookOpen,
  BarChart3,
  UserPlus,
} from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui";
import { useHasRole } from "@/hooks";
import { cn } from "@/utils/utils";

interface NavItem {
  label: string;
  icon: React.ElementType;
  href: string;
  roles: string[];
}

export default function ManagementLayout() {
  const { t } = useTranslation();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const isAdmin = useHasRole("Admin");
  const isInstructor = useHasRole("Instructor");

  const navItems: NavItem[] = [
    {
      label: t("management.nav.dashboard"),
      icon: LayoutDashboard,
      href: "/admin",
      roles: ["Admin", "Instructor"],
    },
    {
      label: t("management.nav.users"),
      icon: Users,
      href: "/admin/users",
      roles: ["Admin"],
    },
    {
      label: t("management.nav.roles"),
      icon: Shield,
      href: "/admin/roles",
      roles: ["Admin"],
    },
    {
      label: t("management.nav.courses"),
      icon: BookOpen,
      href: "/admin/courses",
      roles: ["Admin", "Instructor"],
    },
    {
      label: t("management.nav.analytics"),
      icon: BarChart3,
      href: "/admin/analytics",
      roles: ["Admin", "Instructor"],
    },
    {
      label: t("management.nav.enrollments"),
      icon: UserPlus,
      href: "/admin/enrollments",
      roles: ["Admin", "Instructor"],
    },
  ];

  const filteredNavItems = navItems.filter((item) => {
    if (item.roles.includes("Admin") && isAdmin) return true;
    if (item.roles.includes("Instructor") && isInstructor) return true;
    return false;
  });

  const isActivePath = (href: string) => {
    if (href === "/admin") {
      return location.pathname === href;
    }
    return location.pathname.startsWith(href);
  };

  const toggleSidebar = () => setSidebarOpen(!sidebarOpen);

  return (
    <div className="flex h-screen overflow-hidden">
      {/* Mobile Sidebar Overlay */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={toggleSidebar}
        />
      )}

      {/* Sidebar */}
      <aside
        className={cn(
          "fixed lg:static inset-y-0 left-0 z-50 w-64 bg-card border-r border-border transform transition-transform duration-300 ease-in-out lg:translate-x-0",
          sidebarOpen ? "translate-x-0" : "-translate-x-full"
        )}
      >
        <div className="flex flex-col h-full">
          {/* Sidebar Header */}
          <div className="flex items-center justify-between px-6 py-4 border-b border-border">
            <h2 className="text-lg font-semibold">{t("management.title")}</h2>
            <button
              onClick={toggleSidebar}
              className="lg:hidden p-2 hover:bg-accent rounded-md"
            >
              <X className="h-5 w-5" />
            </button>
          </div>

          {/* Navigation Links */}
          <nav className="flex-1 overflow-y-auto py-4">
            <ul className="space-y-1 px-3">
              {filteredNavItems.map((item) => {
                const Icon = item.icon;
                const active = isActivePath(item.href);
                return (
                  <li key={item.href}>
                    <Link
                      to={item.href}
                      onClick={() => setSidebarOpen(false)}
                      className={cn(
                        "flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors",
                        active
                          ? "bg-primary text-primary-foreground"
                          : "text-muted-foreground hover:bg-accent hover:text-foreground"
                      )}
                    >
                      <Icon className="h-5 w-5" />
                      <span>{item.label}</span>
                    </Link>
                  </li>
                );
              })}
            </ul>
          </nav>

          {/* Back to Learning Environment */}
          <div className="p-4 border-t border-border">
            <Link to="/">
              <Button
                variant="outline"
                className="w-full justify-start"
                onClick={() => setSidebarOpen(false)}
              >
                {t("management.backToLearning")}
              </Button>
            </Link>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Top Bar */}
        <header className="bg-card border-b border-border px-4 py-3 flex items-center gap-4">
          <button
            onClick={toggleSidebar}
            className="lg:hidden p-2 hover:bg-accent rounded-md"
          >
            <Menu className="h-5 w-5" />
          </button>
          <h1 className="text-xl font-semibold">
            {t("management.title")}
          </h1>
        </header>

        {/* Page Content */}
        <main className="flex-1 overflow-y-auto bg-background">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
