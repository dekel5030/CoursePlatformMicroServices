export default function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-100 py-8 px-12 text-sm mt-12">
      <div className="max-w-7xl mx-auto text-center">
        <div className="font-bold text-xl mb-4 text-gray-50">CoursePlatform</div>

        <nav className="flex justify-center flex-wrap gap-6 mb-4">
          <a href="#" className="text-gray-400 hover:text-white transition-colors">About</a>
          <a href="#" className="text-gray-400 hover:text-white transition-colors">Contact</a>
          <a href="#" className="text-gray-400 hover:text-white transition-colors">Terms</a>
          <a href="#" className="text-gray-400 hover:text-white transition-colors">Privacy</a>
        </nav>

        <p className="text-gray-500 text-xs">
          © {new Date().getFullYear()} CoursePlatform. All rights reserved.
        </p>
      </div>
    </footer>
  );
}
