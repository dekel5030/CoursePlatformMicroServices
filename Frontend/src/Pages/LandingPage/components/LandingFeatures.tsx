import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { BookOpen, Users, Trophy, Zap, Globe, Shield } from "lucide-react";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";

export function LandingFeatures() {
  const { t } = useTranslation();

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

  const iconProps = {
    strokeWidth: 1.5,
    className: "h-5 w-5 text-primary"
  };

  return (
    <section className="py-24 bg-muted/20">
      <div className="container px-4 md:px-6 mx-auto">
        <motion.div 
          className="text-center mb-16 max-w-2xl mx-auto"
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ duration: 0.5 }}
        >
          <h2 className="text-3xl font-bold tracking-tight sm:text-4xl text-foreground">
            {t('landing.features.title')}
          </h2>
          <p className="mt-4 text-muted-foreground text-lg">
            {t('landing.features.subtitle')}
          </p>
        </motion.div>

        <motion.div 
          className="grid grid-cols-1 md:grid-cols-3 gap-6 max-w-6xl mx-auto"
          variants={container}
          initial="hidden"
          whileInView="show"
          viewport={{ once: true }}
        >
          {/* Large Card */}
          <motion.div className="md:col-span-2 row-span-2" variants={item}>
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
          <motion.div variants={item}>
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
          <motion.div variants={item}>
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
          <motion.div className="md:col-span-1" variants={item}>
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
          <motion.div className="md:col-span-2" variants={item}>
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
      </div>
    </section>
  );
}