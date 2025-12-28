import { Navbar } from "@/components/layout";
import { LandingHero } from "./components/LandingHero";
import { LandingStats } from "./components/LandingStats";
import { LandingFeatures } from "./components/LandingFeatures";
import { LandingAbout } from "./components/LandingAbout";
import { LandingShowcase } from "./components/LandingShowcase";
import { LandingFooter } from "./components/LandingFooter";

export default function LandingPage() {
  return (
    <div className="flex flex-col min-h-screen">
      <Navbar />
      <main className="flex-1">
        <LandingHero />
        <LandingStats />
        <LandingAbout />
        <LandingFeatures />
        <LandingShowcase />
      </main>
      <LandingFooter />
    </div>
  );
}
