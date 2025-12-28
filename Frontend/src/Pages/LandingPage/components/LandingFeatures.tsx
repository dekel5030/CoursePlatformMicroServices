import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { BookOpen, Users, Trophy, Zap, Globe, Shield } from "lucide-react";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { PageSection } from "@/components/common";
import { staggerContainer, fadeInUp } from "@/utils/animations";

export function LandingFeatures() {
  const { t } = useTranslation();

  const iconProps = {
    strokeWidth: 1.5,
    className: "h-5 w-5 text-primary"
  };

  return (
    <PageSection className="bg-muted/20">
      <motion.div 
        className="text-center mb-16 max-w-2xl mx-auto"
        initial="hidden"
        whileInView="show"
        viewport={{ once: true }}
        variants={staggerContainer}
      >
        <motion.h2 
          className="text-3xl font-bold tracking-tight sm:text-4xl text-foreground"
          variants={fadeInUp}
        >
          {t('landing.features.title')}
        </motion.h2>
        <motion.p 
          className="mt-4 text-muted-foreground text-lg"
          variants={fadeInUp}
        >
          {t('landing.features.subtitle')}
        </motion.p>
      </motion.div>

      <motion.div 
        className="grid grid-cols-1 md:grid-cols-3 gap-6 max-w-6xl mx-auto"
        variants={staggerContainer}
        initial="hidden"
        whileInView="show"
        viewport={{ once: true }}
      >
        {/* Large Card */}
        <motion.div className="md:col-span-2 row-span-2" variants={fadeInUp}>
          <Card className="h-full border-border/60 shadow-sm hover:shadow-md transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-3 text-xl font-semibold">
                <Globe {...iconProps} className="h-6 w-6 text-primary" />
                {t('landing.features.globalCommunity.title')}
              </CardTitle>
              <CardDescription className="text-base mt-2">
                {t('landing.features.globalCommunity.description')}
              </CardDescription>
            </CardHeader>
            <CardContent className="flex items-center justify-center min-h-[200px] bg-muted/10 m-6 rounded-md border border-dashed border-border/50">
              <Users className="h-24 w-24 text-muted-foreground/10" strokeWidth={1} />
            </CardContent>
          </Card>
        </motion.div>

        {/* Small Card 1 */}
        <motion.div variants={fadeInUp}>
          <Card className="h-full border-border/60 shadow-sm hover:shadow-md transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <BookOpen {...iconProps} />
                {t('landing.features.expertContent.title')}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground leading-relaxed">
                {t('landing.features.expertContent.description')}
              </p>
            </CardContent>
          </Card>
        </motion.div>

        {/* Small Card 2 */}
        <motion.div variants={fadeInUp}>
          <Card className="h-full border-border/60 shadow-sm hover:shadow-md transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <Zap {...iconProps} />
                {t('landing.features.fastLearning.title')}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground leading-relaxed">
                {t('landing.features.fastLearning.description')}
              </p>
            </CardContent>
          </Card>
        </motion.div>

        {/* Medium Card */}
        <motion.div className="md:col-span-1" variants={fadeInUp}>
          <Card className="h-full border-border/60 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <Trophy {...iconProps} />
                {t('landing.features.certificates.title')}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground leading-relaxed">
                {t('landing.features.certificates.description')}
              </p>
            </CardContent>
          </Card>
        </motion.div>

        {/* Medium Card */}
        <motion.div className="md:col-span-2" variants={fadeInUp}>
            <Card className="h-full border-border/60 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <Shield {...iconProps} />
                {t('landing.features.securePlatform.title')}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground leading-relaxed">
                {t('landing.features.securePlatform.description')}
              </p>
            </CardContent>
          </Card>
        </motion.div>
      </motion.div>
    </PageSection>
  );
}
