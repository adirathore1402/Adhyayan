import { Link } from 'react-router-dom';
import { useTheme } from '../context/ThemeContext';

const mascots: Record<string, string> = {
  candy: '🦄',
  ocean: '🐬',
  jungle: '🐯',
  space: '👩‍🚀',
  sunset: '🦋',
};

const taglines: Record<string, string> = {
  candy: 'Sweet Learning Adventures Await!',
  ocean: 'Dive Into a Sea of Knowledge!',
  jungle: 'Explore the Wild World of Learning!',
  space: 'Blast Off to Brilliant Learning!',
  sunset: 'Let Your Knowledge Shine Bright!',
};

export default function HomePage() {
  const { theme } = useTheme();
  const mascot = mascots[theme] || '📚';
  const tagline = taglines[theme] || 'Learn, Grow, Shine!';

  return (
    <div className="hero">
      <div style={{ fontSize: '4rem', marginBottom: 16, animation: 'mega-bounce 2s ease infinite' }}>
        {mascot}
      </div>
      <h1>Adhyayan अध्ययन</h1>
      <p className="hero-subtitle">{tagline}</p>
      <p className="hero-subtitle" style={{ fontSize: '0.95rem', marginTop: -16, opacity: 0.8 }}>
        AI-powered learning for Classes 1–5 · CBSE + 5 State Boards · Math · English · EVS
      </p>
      <div className="hero-actions">
        <Link to="/practice" className="btn btn-primary btn-lg">
          🚀 Start Practicing
        </Link>
        <Link to="/register" className="btn btn-outline btn-lg">
          ✨ Create Account
        </Link>
      </div>

      <div className="card-grid" style={{ maxWidth: 960, margin: '48px auto 0' }}>
        <div className="card feature-card anim-slide-up-1">
          <span className="feature-icon">📖</span>
          <h3>Chapter Practice</h3>
          <p style={{ color: 'var(--text-light)', fontWeight: 600, marginTop: 8 }}>
            Pick your board, class, subject & chapter. AI creates questions just for your syllabus!
          </p>
        </div>
        <div className="card feature-card anim-slide-up-2">
          <span className="feature-icon">🌟</span>
          <h3>Daily Adventure</h3>
          <p style={{ color: 'var(--text-light)', fontWeight: 600, marginTop: 8 }}>
            Fun daily challenges that build confidence. Earn streaks and celebrate every win!
          </p>
        </div>
        <div className="card feature-card anim-slide-up-3">
          <span className="feature-icon">📊</span>
          <h3>Parent Dashboard</h3>
          <p style={{ color: 'var(--text-light)', fontWeight: 600, marginTop: 8 }}>
            Track progress by chapter. See accuracy scores and find where extra practice helps.
          </p>
        </div>
      </div>
    </div>
  );
}
