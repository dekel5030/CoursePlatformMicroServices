import { Dropdown, SearchBox, ProfileMenu } from "@/components/common";
import { Button } from "@/components/ui";
import { useAuth } from "react-oidc-context";

export default function Navbar() {
  const auth = useAuth();

  const handleLoginClick = () => {
    void auth.signinRedirect();
  };

  const handleSignUpClick = () => {
    void auth.signinRedirect({
      extraQueryParams: { kc_action: "register" },
    });
  };

  return (
    <header className="flex items-center justify-between px-8 py-2 bg-background border-b border-border">
      <div className="text-2xl font-bold text-primary">CourseHub</div>

      <nav className="flex-1 ml-8 hidden md:block">
        <ul className="flex items-center gap-6 list-none">
          <Dropdown label="Explore">
            <li className="hover:bg-accent px-2 py-1 rounded">Learn AI</li>
            <li className="hover:bg-accent px-2 py-1 rounded">Launch a Career</li>
            <li className="hover:bg-accent px-2 py-1 rounded">Certification Prep</li>
          </Dropdown>
          <SearchBox />
          <Dropdown label="Development">
            <li className="hover:bg-accent px-2 py-1 rounded">Web Development</li>
            <li className="hover:bg-accent px-2 py-1 rounded">Mobile Apps</li>
            <li className="hover:bg-accent px-2 py-1 rounded">Data Science</li>
          </Dropdown>
          <li className="relative">Design</li>
          <li className="relative">Marketing</li>
        </ul>
      </nav>

      <div className="flex gap-4">
        {auth.isAuthenticated ? (
          <ProfileMenu />
        ) : (
          <>
            <Button variant="outline" onClick={handleLoginClick}>
              Log in
            </Button>
            <Button variant="default" onClick={handleSignUpClick}>
              Sign up
            </Button>
          </>
        )}
      </div>
    </header>
  );
}
