import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  BreadcrumbNav,
} from "@/components";
import { Users, Shield, ArrowRight } from "lucide-react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { motion } from "framer-motion";

export default function AdminDashboardPage() {
  const { t } = useTranslation(["auth", "translation"]);

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.admin") },
  ];

  const cards = [
    {
      title: t("auth:admin.dashboard.usersCard.title"),
      description: t("auth:admin.dashboard.usersCard.description"),
      icon: Users,
      href: "/admin/users",
      color: "bg-blue-500/10 text-blue-500",
    },
    {
      title: t("auth:admin.dashboard.rolesCard.title"),
      description: t("auth:admin.dashboard.rolesCard.description"),
      icon: Shield,
      href: "/admin/roles",
      color: "bg-purple-500/10 text-purple-500",
    },
  ];

  const container = {
    hidden: { opacity: 0 },
    show: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1,
      },
    },
  };

  const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 },
  };

  return (
    <div className="space-y-6">
      <BreadcrumbNav items={breadcrumbItems} />
      <motion.div
        className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8"
        variants={container}
        initial="hidden"
        animate="show"
      >
        <motion.div variants={item} className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">
            {t("auth:admin.dashboard.title")}
          </h1>
          <p className="text-muted-foreground">
            {t("auth:admin.dashboard.subtitle")}
          </p>
        </motion.div>

        <motion.div
          variants={container}
          className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
        >
          {cards.map((card) => (
            <motion.div key={card.href} variants={item}>
              <Link to={card.href}>
                <Card className="group hover:border-primary/50 transition-all hover:shadow-md h-full">
                  <CardHeader>
                    <div
                      className={`w-12 h-12 rounded-lg ${card.color} flex items-center justify-center mb-4`}
                    >
                      <card.icon className="h-6 w-6" />
                    </div>
                    <CardTitle className="group-hover:text-primary transition-colors flex items-center justify-between">
                      {card.title}
                      <ArrowRight className="h-4 w-4 opacity-0 -translate-x-2 group-hover:opacity-100 group-hover:translate-x-0 transition-all" />
                    </CardTitle>
                    <CardDescription>{card.description}</CardDescription>
                  </CardHeader>
                  <CardContent />
                </Card>
              </Link>
            </motion.div>
          ))}
        </motion.div>
      </motion.div>
    </div>
  );
}
