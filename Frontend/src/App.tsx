import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import HomePage from "./Pages/homePage/HomePage.tsx";
import CoursePage from "./Pages/CoursePage/CoursePage.tsx";
import LoginPage from "./Pages/LoginPage/LoginPage.tsx";
import SignUpPage from "./Pages/SignUpPage/SignUpPage.tsx";

export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/courses/:id" element={<CoursePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/signup" element={<SignUpPage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}
