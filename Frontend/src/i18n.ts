import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import HttpApi from "i18next-http-backend";

// Import locale files directly for better reliability
import translationEN from "./locales/en/translation.json";
import translationHE from "./locales/he/translation.json";
import coursesEN from "./features/courses/locales/en/courses.json";
import coursesHE from "./features/courses/locales/he/courses.json";
import lessonsEN from "./features/lessons/locales/en/lessons.json";
import lessonsHE from "./features/lessons/locales/he/lessons.json";

// Define resources with feature-based structure
const resources = {
  en: {
    translation: translationEN,
    courses: coursesEN,
    lessons: lessonsEN,
  },
  he: {
    translation: translationHE,
    courses: coursesHE,
    lessons: lessonsHE,
  },
};

i18n
  .use(HttpApi)
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: "en",
    debug: false, // Set to true for debugging during dev
    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },
    ns: ["translation", "courses", "lessons"], // Add namespaces
    defaultNS: "translation",
    fallbackNS: "translation",
    // Return key if translation is missing instead of showing translation key
    returnEmptyString: false,
    returnNull: false,
  });

// Set initial direction based on detected or stored language
const currentLanguage = i18n.language || "en";
document.documentElement.dir = i18n.dir(currentLanguage);
document.documentElement.lang = currentLanguage;

// Handle RTL/LTR updates
i18n.on("languageChanged", (lng) => {
  document.documentElement.dir = i18n.dir(lng);
  document.documentElement.lang = lng;
});

export default i18n;
