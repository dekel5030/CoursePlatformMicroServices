import { RoleDetail } from "@/features/iam-dashboard";
import { BreadcrumbNav } from "@/components";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

export default function RoleDetailPage() {
  const { t } = useTranslation();
  const { roleName } = useParams<{ roleName: string }>();

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.roles"), path: "/admin/roles" },
    { label: roleName || t("breadcrumbs.roleDetails") },
  ];

  return (
    <div className="space-y-6">
      <BreadcrumbNav items={breadcrumbItems} />
      <RoleDetail />
    </div>
  );
}
