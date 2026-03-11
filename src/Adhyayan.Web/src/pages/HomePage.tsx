import { Link } from 'react-router-dom';

export default function HomePage() {
  return (
    <div className="hero">
      <h1>Welcome to Adhyayan 📚</h1>
      <p>
        AI-powered, curriculum-aligned learning for Indian school children.
        Aligned with CBSE, Maharashtra, Karnataka, Tamil Nadu, UP, and MP boards.
        Classes 1 to 5 — Math, English, EVS.
      </p>
      <div className="hero-actions">
        <Link to="/practice" className="btn btn-primary">Start Practicing</Link>
        <Link to="/register" className="btn btn-outline">Create Account</Link>
      </div>

      <div className="card-grid" style={{ maxWidth: 900, margin: '48px auto 0' }}>
        <div className="card" style={{ textAlign: 'left' }}>
          <h3>📖 Chapter Practice</h3>
          <p style={{ color: 'var(--text-light)', marginTop: 8 }}>
            Choose your board, class, subject, and chapter. Practice with AI-generated questions aligned to your syllabus.
          </p>
        </div>
        <div className="card" style={{ textAlign: 'left' }}>
          <h3>🌟 Daily Adventure</h3>
          <p style={{ color: 'var(--text-light)', marginTop: 8 }}>
            Fun daily practice with mostly easy questions to build confidence and keep learning enjoyable.
          </p>
        </div>
        <div className="card" style={{ textAlign: 'left' }}>
          <h3>📊 Parent Dashboard</h3>
          <p style={{ color: 'var(--text-light)', marginTop: 8 }}>
            Track your child's progress by chapter. See accuracy and identify areas that need more practice.
          </p>
        </div>
      </div>
    </div>
  );
}
