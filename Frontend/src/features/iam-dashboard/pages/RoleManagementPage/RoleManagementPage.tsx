import { RoleList } from "@/features/iam-dashboard";
import { BreadcrumbNav } from "@/components";
import { useTranslation } from "react-i18next";

export default function RoleManagementPage() {
  const { t } = useTranslation(["auth", "translation"]);

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.roles") },
  ];

  return (
    <div className="space-y-6">
      <BreadcrumbNav items={breadcrumbItems} />
      <RoleList />
    </div>
  );
}
