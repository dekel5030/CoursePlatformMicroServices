import { useNavigate } from 'react-router-dom';
import { Users, Shield } from 'lucide-react';
import { useRoles } from '../hooks';
import { Badge, Card, CardHeader, CardTitle, CardContent, CardFooter, Skeleton } from '@/components/ui';
import { motion } from 'framer-motion';
import { useTranslation } from 'react-i18next';

export default function RoleList() {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const { data: roles, isLoading, error } = useRoles();

  const container = {
    hidden: { opacity: 0 },
    show: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1
      }
    }
  };

  const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
  };

  if (isLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
        <div className="space-y-2">
          <Skeleton className="h-8 w-32" />
          <Skeleton className="h-4 w-64" />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {[1, 2, 3].map((i) => (
            <Card key={i} className="border-border/60">
              <CardHeader>
                <div className="flex items-center gap-2">
                  <Skeleton className="h-5 w-5 rounded-full" />
                  <Skeleton className="h-6 w-32" />
                </div>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1">
                    <Skeleton className="h-8 w-10" />
                    <Skeleton className="h-3 w-16" />
                  </div>
                  <div className="space-y-1">
                    <Skeleton className="h-8 w-10" />
                    <Skeleton className="h-3 w-16" />
                  </div>
                </div>
              </CardContent>
              <CardFooter>
                <Skeleton className="h-5 w-24 rounded-full" />
              </CardFooter>
            </Card>
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          {t('common.error', { message: error.message })}
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
      <div className="space-y-2">
        <h2 className="text-2xl font-bold">{t('authManagement.roles.title')}</h2>
        <p className="text-muted-foreground">{t('authManagement.roles.subtitle')}</p>
      </div>

      {roles && roles.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          <p>{t('authManagement.roles.noRoles')}</p>
        </div>
      ) : (
        <motion.div 
          className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4"
          variants={container}
          initial="hidden"
          animate="show"
        >
          {roles?.map((role) => (
            <motion.div key={role.id} variants={item}>
              <Card
                className="cursor-pointer hover:shadow-lg transition-shadow border-border/60 hover:border-primary/50"
                onClick={() => navigate(`/admin/roles/${encodeURIComponent(role.name)}`)}
              >
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Shield className="h-5 w-5 text-primary" />
                    {role.name}
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-1">
                      <div className="text-2xl font-bold">{role.userCount}</div>
                      <div className="text-sm text-muted-foreground flex items-center gap-1">
                        <Users className="h-3 w-3" />
                        {role.userCount === 1 ? t('authManagement.roles.userCount') : t('authManagement.roles.userCount_plural')}
                      </div>
                    </div>
                    <div className="space-y-1">
                      <div className="text-2xl font-bold">{role.permissionCount}</div>
                      <div className="text-sm text-muted-foreground">
                        {role.permissionCount === 1 ? t('authManagement.roles.permissionCount') : t('authManagement.roles.permissionCount_plural')}
                      </div>
                    </div>
                  </div>
                </CardContent>
                <CardFooter>
                  <Badge variant="secondary" className="text-xs">
                    {role.permissionCount} {t('authManagement.roles.permissions')}
                  </Badge>
                </CardFooter>
              </Card>
            </motion.div>
          ))}
        </motion.div>
      )}
    </div>
  );
}
