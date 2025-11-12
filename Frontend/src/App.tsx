import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import HomePage from "./pages/homePage/HomePage.tsx";
import CoursePage from "./pages/coursePage/CoursePage.tsx";

export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/courses/:id" element={<CoursePage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}
