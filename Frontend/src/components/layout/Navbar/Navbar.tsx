import { Dropdown, ProfileMenu, LanguageSwitcher } from "../../common";
import { Button } from "../../ui";
import { useAuth } from "react-oidc-context";
import { Link, useLocation } from "react-router-dom";
import { Twitter, Linkedin, Github, Facebook } from "lucide-react";
import { useTranslation } from "react-i18next";

export default function Navbar() {
  const auth = useAuth();
  const location = useLocation();
  const { t } = useTranslation();

  // Build redirect URL for login/register if not on landing page
  const getAuthRedirectUrl = () => {
    if (location.pathname === "/" || location.pathname === "/login" || location.pathname === "/register") {
      return "";
    }
    return `?redirect=${encodeURIComponent(location.pathname + location.search)}`;
  };

  return (
    <header className="flex flex-col bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 border-b border-border sticky top-0 z-50">
      {/* Top Row: Auth - Logo - Socials */}
      <div className="flex items-center justify-between px-6 md:px-12 py-2 border-b border-border/40">
        {/* Left: Auth Buttons & Lang Switcher */}
        <div className="flex items-center gap-3">
          {auth.isAuthenticated ? (
            <ProfileMenu />
          ) : (
            <>
              <Link to={`/login${getAuthRedirectUrl()}`}>
                <Button
                  variant="ghost"
                  size="sm"
                  className="text-xs font-medium h-8 px-3"
                >
                  {t("navbar.login")}
                </Button>
              </Link>
              <Link to={`/register${getAuthRedirectUrl()}`}>
                <Button
                  variant="default"
                  size="sm"
                  className="text-xs font-medium h-8 px-3"
                >
                  {t("navbar.signup")}
                </Button>
              </Link>
            </>
          )}
          <LanguageSwitcher />
        </div>

        {/* Center: Logo */}
        <div className="absolute left-1/2 transform -translate-x-1/2">
          <Link
            to="/"
            className="text-xl font-bold text-foreground tracking-tighter hover:opacity-80 transition-opacity"
          >
            CourseHub
          </Link>
        </div>

        {/* Right: Social Icons */}
        <div className="flex items-center gap-4">
          <a
            href="#"
            className="text-muted-foreground hover:text-foreground transition-colors"
          >
            <Twitter className="h-4 w-4" />
          </a>
          <a
            href="#"
            className="text-muted-foreground hover:text-foreground transition-colors"
          >
            <Linkedin className="h-4 w-4" />
          </a>
          <a
            href="#"
            className="text-muted-foreground hover:text-foreground transition-colors"
          >
            <Github className="h-4 w-4" />
          </a>
          <a
            href="#"
            className="text-muted-foreground hover:text-foreground transition-colors"
          >
            <Facebook className="h-4 w-4" />
          </a>
        </div>
      </div>

      {/* Bottom Row: Navigation Tabs */}
      <div className="px-6 md:px-12 py-3 flex justify-center items-center">
        <nav className="flex items-center gap-8">
          <ul className="flex items-center gap-8 list-none text-sm font-medium text-muted-foreground">
            <li className="hover:text-foreground transition-colors cursor-pointer">
              {t("navbar.marketing")}
            </li>
            <li className="hover:text-foreground transition-colors cursor-pointer">
              {t("navbar.design")}
            </li>

            <Dropdown label={t("navbar.development")}>
              <li className="hover:bg-accent px-3 py-2 rounded-sm cursor-pointer text-sm">
                {t("navbar.dropdowns.disciplines.architecture")}
              </li>
              <li className="hover:bg-accent px-3 py-2 rounded-sm cursor-pointer text-sm">
                {t("navbar.dropdowns.disciplines.interiorDesign")}
              </li>
              <li className="hover:bg-accent px-3 py-2 rounded-sm cursor-pointer text-sm">
                {t("navbar.dropdowns.disciplines.landscape")}
              </li>
            </Dropdown>

            <Dropdown label={t("navbar.explore")}>
              <li className="hover:bg-accent px-3 py-2 rounded-sm cursor-pointer text-sm">
                {t("navbar.dropdowns.software.revit")}
              </li>
              <li className="hover:bg-accent px-3 py-2 rounded-sm cursor-pointer text-sm">
                {t("navbar.dropdowns.software.autocad")}
              </li>
              <li className="hover:bg-accent px-3 py-2 rounded-sm cursor-pointer text-sm">
                {t("navbar.dropdowns.software.lumionVray")}
              </li>
            </Dropdown>

            <li>
              <Link
                to="/catalog"
                className="hover:text-foreground transition-colors"
              >
                {t("navbar.catalog")}
              </Link>
            </li>
          </ul>
        </nav>
      </div>
    </header>
  );
}
