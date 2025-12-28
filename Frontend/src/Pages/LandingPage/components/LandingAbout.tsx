import { Button } from "@/components/ui/button";
import { CheckCircle2 } from "lucide-react";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";

export function LandingAbout() {
  const { t } = useTranslation();

  return (
    <section className="py-24 bg-background overflow-hidden">
      <div className="container px-4 md:px-6 mx-auto">
        <div className="grid lg:grid-cols-2 gap-12 lg:gap-8 items-center">
          <motion.div
            initial={{ opacity: 0, x: -50 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6 }}
            className="space-y-8"
          >
            <div className="space-y-4">
              <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
                {t('landing.about.title')}
              </h2>
              <p className="text-muted-foreground text-lg leading-relaxed">
                {t('landing.about.description')}
              </p>
            </div>
            
            <ul className="space-y-4">
              {[
                t('landing.about.points.curriculum'),
                t('landing.about.points.projects'),
                t('landing.about.points.mentorship'),
                t('landing.about.points.community')
              ].map((item, index) => (
                <li key={index} className="flex items-center gap-3">
                  <CheckCircle2 className="h-5 w-5 text-primary flex-shrink-0" />
                  <span className="text-foreground/90 font-medium">{item}</span>
                </li>
              ))}
            </ul>

            <Button size="lg" className="rounded-full px-8">
              {t('landing.about.readStory')}
            </Button>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, x: 50 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="relative"
          >
            <div className="relative aspect-square md:aspect-[4/3] rounded-2xl overflow-hidden shadow-2xl border border-border/50">
               {/* Placeholder for About Image */}
               <img 
                src="https://placehold.co/800x600/1a1a1a/FFF?text=Team+Collaboration" 
                alt="Team working together" 
                className="object-cover w-full h-full hover:scale-105 transition-transform duration-700"
              />
            </div>
            
            {/* Decorative Element */}
            <div className="absolute -bottom-6 -left-6 w-24 h-24 bg-primary/10 rounded-full blur-2xl -z-10" />
            <div className="absolute -top-6 -right-6 w-32 h-32 bg-secondary/20 rounded-full blur-3xl -z-10" />
          </motion.div>
        </div>
      </div>
    </section>
  );
}
