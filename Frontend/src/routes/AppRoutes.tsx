import { Routes, Route } from "react-router-dom";
import HomePage from "../Pages/homePage/HomePage";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
    </Routes>
  );
}
