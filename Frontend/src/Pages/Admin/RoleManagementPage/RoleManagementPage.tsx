import { RoleList } from '@/features/auth-management';
import Breadcrumb from "@/components/layout/Breadcrumb/Breadcrumb";
import { useTranslation } from "react-i18next";

export default function RoleManagementPage() {
  const { t } = useTranslation();

  const breadcrumbItems = [
    { label: t('breadcrumbs.home'), path: '/' },
    { label: t('breadcrumbs.roles') },
  ];

  return (
    <div className="space-y-6">
      <Breadcrumb items={breadcrumbItems} />
      <RoleList />
    </div>
  );
}
