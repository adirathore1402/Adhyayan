import { Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import FloatingShapes from './components/FloatingShapes';
import ThemeSwitcher from './components/ThemeSwitcher';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import PracticePage from './pages/PracticePage';
import SessionPage from './pages/SessionPage';
import DashboardPage from './pages/DashboardPage';

export default function App() {
  return (
    <>
      <FloatingShapes />
      <Navbar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/practice" element={<PracticePage />} />
        <Route path="/practice/session" element={<SessionPage />} />
        <Route path="/dashboard" element={<DashboardPage />} />
      </Routes>
      <ThemeSwitcher />
    </>
  );
}
