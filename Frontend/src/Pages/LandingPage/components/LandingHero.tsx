import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { fadeInUp, staggerContainer } from "@/utils/animations";
import { PageSection } from "@/components/common";

export function LandingHero() {
  const { t } = useTranslation();
  
  return (
    <PageSection className="relative lg:py-32 overflow-hidden bg-background">
      <div className="relative z-10">
        <motion.div 
          className="flex flex-col items-center space-y-6 text-center max-w-4xl mx-auto"
          variants={staggerContainer}
          initial="hidden"
          animate="show"
        >
          <motion.div variants={fadeInUp}>
            <Badge variant="outline" className="px-3 py-1 text-sm font-medium border-primary/20 bg-primary/5 text-primary rounded-full">
              {t('landing.hero.badge')}
            </Badge>
          </motion.div>
          
          <motion.h1 
            className="text-4xl font-bold tracking-tight sm:text-5xl md:text-6xl lg:text-7xl text-foreground"
            variants={fadeInUp}
          >
            {t('landing.hero.title')} <span className="text-primary">CourseHub</span>
          </motion.h1>

          <motion.p 
            className="mx-auto max-w-[700px] text-muted-foreground text-lg md:text-xl leading-relaxed"
            variants={fadeInUp}
          >
            {t('landing.hero.subtitle')}
          </motion.p>
          
          <motion.div 
            className="flex flex-col sm:flex-row gap-4 mt-8"
            variants={fadeInUp}
          >
            <Link to="/catalog">
              <Button size="lg" className="w-full sm:w-auto text-base px-8 h-11 rounded-md shadow-sm">
                {t('landing.hero.getStarted')}
              </Button>
            </Link>
            <Link to="/catalog">
              <Button variant="outline" size="lg" className="w-full sm:w-auto text-base px-8 h-11 rounded-md bg-background hover:bg-accent/50">
                {t('landing.hero.viewCatalog')}
              </Button>
            </Link>
          </motion.div>
        </motion.div>
      </div>
    </PageSection>
  );
}
