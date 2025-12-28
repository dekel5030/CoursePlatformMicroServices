import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

export default function Footer() {
  const { t } = useTranslation();

  return (
    <footer className="py-12 border-t border-border bg-background">
      <div className="container px-4 md:px-6 mx-auto">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-8 mb-8">
          <div>
            <h3 className="font-semibold mb-4 text-sm tracking-wide text-foreground uppercase">
              {t('footer.product')}
            </h3>
            <ul className="space-y-3 text-sm text-muted-foreground">
              <li><Link to="/catalog" className="hover:text-foreground transition-colors">{t('footer.links.courses')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.pricing')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.enterprise')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.successStories')}</Link></li>
            </ul>
          </div>
          <div>
            <h3 className="font-semibold mb-4 text-sm tracking-wide text-foreground uppercase">
              {t('footer.resources')}
            </h3>
            <ul className="space-y-3 text-sm text-muted-foreground">
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.blog')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.documentation')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.community')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.helpCenter')}</Link></li>
            </ul>
          </div>
          <div>
            <h3 className="font-semibold mb-4 text-sm tracking-wide text-foreground uppercase">
              {t('footer.company')}
            </h3>
            <ul className="space-y-3 text-sm text-muted-foreground">
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.aboutUs')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.careers')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.contact')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.partners')}</Link></li>
            </ul>
          </div>
          <div>
            <h3 className="font-semibold mb-4 text-sm tracking-wide text-foreground uppercase">
              {t('footer.legal')}
            </h3>
            <ul className="space-y-3 text-sm text-muted-foreground">
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.privacyPolicy')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.termsOfService')}</Link></li>
              <li><Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.cookiePolicy')}</Link></li>
            </ul>
          </div>
        </div>
        <div className="pt-8 border-t border-border flex flex-col md:flex-row justify-between items-center gap-4 text-xs text-muted-foreground">
          <p>© {new Date().getFullYear()} {t('footer.rightsReserved')}</p>
          <div className="flex gap-4">
            <Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.accessibility')}</Link>
            <Link to="#" className="hover:text-foreground transition-colors">{t('footer.links.sitemap')}</Link>
          </div>
        </div>
      </div>
    </footer>
  );
}