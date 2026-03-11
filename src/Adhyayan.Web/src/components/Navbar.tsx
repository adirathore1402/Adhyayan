import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useTheme } from '../context/ThemeContext';

const mascots: Record<string, string> = {
  candy: '🍭',
  ocean: '🐠',
  jungle: '🦁',
  space: '🚀',
  sunset: '🌅',
};

export default function Navbar() {
  const { isLoggedIn, parentName, logout } = useAuth();
  const { theme } = useTheme();
  const mascot = mascots[theme] || '📚';

  return (
    <nav className="navbar">
      <div className="container">
        <Link to="/" className="navbar-brand">
          {mascot} Adhyayan <span>अध्ययन</span>
        </Link>
        <div className="navbar-links">
          <Link to="/practice">📖 Practice</Link>
          {isLoggedIn ? (
            <>
              <Link to="/dashboard">📊 Dashboard</Link>
              <span className="navbar-user">Hi, {parentName} 👋</span>
              <button className="btn btn-sm" style={{ background: 'rgba(255,255,255,0.2)', color: 'var(--nav-text)', border: 'none' }} onClick={logout}>
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link>
              <Link to="/register" className="btn btn-sm" style={{ background: 'rgba(255,255,255,0.25)', color: 'var(--nav-text)' }}>
                Sign Up
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
