import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";

export function LandingStats() {
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
    hidden: { opacity: 0, y: 10 },
    show: { opacity: 1, y: 0 }
  };

  return (
    <section className="border-y border-border bg-card/50">
      <div className="container px-4 md:px-6 mx-auto">
        <motion.div 
          className="grid grid-cols-2 md:grid-cols-4 divide-y md:divide-y-0 md:divide-x divide-border"
          variants={container}
          initial="hidden"
          whileInView="show"
          viewport={{ once: true }}
        >
          <div className="p-8 text-center">
            <motion.div variants={item}>
              <h3 className="text-4xl font-bold tracking-tight text-foreground">100+</h3>
              <p className="text-sm font-medium text-muted-foreground mt-1 uppercase tracking-wide">
                {t('landing.stats.courses')}
              </p>
            </motion.div>
          </div>
          <div className="p-8 text-center">
             <motion.div variants={item}>
              <h3 className="text-4xl font-bold tracking-tight text-foreground">10k+</h3>
              <p className="text-sm font-medium text-muted-foreground mt-1 uppercase tracking-wide">
                {t('landing.stats.students')}
              </p>
            </motion.div>
          </div>
          <div className="p-8 text-center">
             <motion.div variants={item}>
              <h3 className="text-4xl font-bold tracking-tight text-foreground">50+</h3>
              <p className="text-sm font-medium text-muted-foreground mt-1 uppercase tracking-wide">
                {t('landing.stats.instructors')}
              </p>
            </motion.div>
          </div>
          <div className="p-8 text-center">
             <motion.div variants={item}>
              <h3 className="text-4xl font-bold tracking-tight text-foreground">4.8</h3>
              <p className="text-sm font-medium text-muted-foreground mt-1 uppercase tracking-wide">
                {t('landing.stats.rating')}
              </p>
            </motion.div>
          </div>
        </motion.div>
      </div>
    </section>
  );
}