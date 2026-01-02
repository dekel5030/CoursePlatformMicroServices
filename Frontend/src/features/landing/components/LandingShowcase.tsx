import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { PageSection } from "@/components";
import { staggerContainer, scaleIn, fadeInUp } from "@/utils/animations";

export function LandingShowcase() {
  const { t } = useTranslation();

  const images = [
    {
      src: "https://placehold.co/600x400/2a2a2a/FFF?text=BIM+Modeling",
      alt: "Revit BIM Modeling",
      category: t('landing.showcase.categories.interactive')
    },
    {
      src: "https://placehold.co/600x400/2a2a2a/FFF?text=Rendering",
      alt: "Photorealistic Rendering",
      category: t('landing.showcase.categories.guidance')
    },
    {
      src: "https://placehold.co/600x400/2a2a2a/FFF?text=Parametric+Design",
      alt: "Parametric Design with Grasshopper",
      category: t('landing.showcase.categories.community')
    },
    {
      src: "https://placehold.co/600x400/2a2a2a/FFF?text=Construction+Docs",
      alt: "Construction Documentation",
      category: t('landing.showcase.categories.achievement')
    }
  ];

  return (
    <PageSection className="bg-muted/30">
      <motion.div 
        className="text-center mb-16 space-y-4"
        initial="hidden"
        whileInView="show"
        viewport={{ once: true }}
        variants={staggerContainer}
      >
        <motion.h2 
          className="text-3xl font-bold tracking-tight sm:text-4xl"
          variants={fadeInUp}
        >
          {t('landing.showcase.title')}
        </motion.h2>
        <motion.p 
          className="text-muted-foreground text-lg max-w-2xl mx-auto"
          variants={fadeInUp}
        >
          {t('landing.showcase.subtitle')}
        </motion.p>
      </motion.div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-5xl mx-auto">
        {images.map((img, idx) => (
          <motion.div
            key={idx}
            initial="hidden"
            whileInView="show"
            viewport={{ once: true }}
            variants={scaleIn}
            className="relative group overflow-hidden rounded-xl shadow-lg border border-border/50 h-[300px]"
          >
            <img
              src={img.src}
              alt={img.alt}
              className="object-cover w-full h-full transition-transform duration-700 group-hover:scale-110"
            />
            <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex items-center justify-center">
              <span className="text-white font-semibold tracking-wide text-lg border-2 border-white/80 px-6 py-2 rounded-full backdrop-blur-sm">
                {img.category}
              </span>
            </div>
          </motion.div>
        ))}
      </div>
    </PageSection>
  );
}



