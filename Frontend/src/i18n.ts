import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

// Global (shared) translations
import translationEN from "./locales/en/translation.json";
import translationHE from "./locales/he/translation.json";

// Feature-based translations (each feature owns its locales)
import authEN from "./features/auth/locales/en/auth.json";
import authHE from "./features/auth/locales/he/auth.json";
import courseCatalogEN from "./features/course-catalog/locales/en.json";
import courseCatalogHE from "./features/course-catalog/locales/he.json";
import courseManagementEN from "./features/course-management/locales/en.json";
import courseManagementHE from "./features/course-management/locales/he.json";
import lessonViewerEN from "./features/lesson-viewer/locales/en.json";
import lessonViewerHE from "./features/lesson-viewer/locales/he.json";

const resources = {
  en: {
    translation: translationEN,
    auth: authEN,
    "course-catalog": courseCatalogEN,
    "course-management": courseManagementEN,
    "lesson-viewer": lessonViewerEN,
  },
  he: {
    translation: translationHE,
    auth: authHE,
    "course-catalog": courseCatalogHE,
    "course-management": courseManagementHE,
    "lesson-viewer": lessonViewerHE,
  },
};

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: "en",
    debug: false,
    interpolation: {
      escapeValue: false,
    },
    ns: ["translation", "auth", "course-catalog", "course-management", "lesson-viewer"],
    defaultNS: "translation",
    fallbackNS: "translation",
    returnEmptyString: false,
    returnNull: false,
  });

const currentLanguage = i18n.language || "en";
document.documentElement.dir = i18n.dir(currentLanguage);
document.documentElement.lang = currentLanguage;

i18n.on("languageChanged", (lng) => {
  document.documentElement.dir = i18n.dir(lng);
  document.documentElement.lang = lng;
});

export default i18n;
