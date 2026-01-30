import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import HttpApi from "i18next-http-backend";

// Import locale files directly for better reliability
import translationEN from "./locales/en/translation.json";
import translationHE from "./locales/he/translation.json";
import authEN from "./features/auth/locales/en/auth.json";
import authHE from "./features/auth/locales/he/auth.json";

// Note: courses and lessons locales are loaded dynamically via HttpApi
// They are located in dist/locales/en/courses.json and dist/locales/en/lessons.json
// Define resources with feature-based structure
const resources = {
  en: {
    translation: translationEN,
    auth: authEN,
  },
  he: {
    translation: translationHE,
    auth: authHE,
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
    ns: ["translation", "courses", "lessons", "auth"], // Add namespaces
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
