import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { isLoggedIn, parentName, logout } = useAuth();

  return (
    <nav className="navbar">
      <div className="container">
        <Link to="/" className="navbar-brand">
          Adhyayan <span>अध्ययन</span>
        </Link>
        <div className="navbar-links">
          <Link to="/practice">Practice</Link>
          {isLoggedIn ? (
            <>
              <Link to="/dashboard">Dashboard</Link>
              <span style={{ color: 'var(--text-light)' }}>Hi, {parentName}</span>
              <button className="btn btn-outline btn-sm" onClick={logout}>Logout</button>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link>
              <Link to="/register" className="btn btn-primary btn-sm">Sign Up</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
