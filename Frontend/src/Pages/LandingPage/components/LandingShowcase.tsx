import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";

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
    <section className="py-24 bg-muted/30">
      <div className="container px-4 md:px-6 mx-auto">
        <div className="text-center mb-16 space-y-4">
          <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
            {t('landing.showcase.title')}
          </h2>
          <p className="text-muted-foreground text-lg max-w-2xl mx-auto">
            {t('landing.showcase.subtitle')}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-5xl mx-auto">
          {images.map((img, idx) => (
            <motion.div
              key={idx}
              initial={{ opacity: 0, scale: 0.95 }}
              whileInView={{ opacity: 1, scale: 1 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5, delay: idx * 0.1 }}
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
      </div>
    </section>
  );
}
