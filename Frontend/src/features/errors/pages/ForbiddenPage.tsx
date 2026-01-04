import { Link } from "react-router-dom";
import { ShieldAlert } from "lucide-react";
import { Button } from "@/components/ui";
import { useTranslation } from "react-i18next";

export default function ForbiddenPage() {
  const { t } = useTranslation();

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4">
      <div className="max-w-md w-full text-center space-y-8">
        <div className="flex justify-center">
          <ShieldAlert className="h-24 w-24 text-destructive" />
        </div>
        <div className="space-y-3">
          <h1 className="text-4xl font-bold text-foreground">
            {t("errors.forbidden.title")}
          </h1>
          <p className="text-lg text-muted-foreground">
            {t("errors.forbidden.description")}
          </p>
        </div>
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Button asChild variant="default">
            <Link to="/">{t("errors.forbidden.goHome")}</Link>
          </Button>
          <Button asChild variant="outline">
            <Link to="/catalog">{t("errors.forbidden.browseCatalog")}</Link>
          </Button>
        </div>
      </div>
    </div>
  );
}
