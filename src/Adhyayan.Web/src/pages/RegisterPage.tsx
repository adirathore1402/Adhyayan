import { useState, FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { register } from '../services/api';

export default function RegisterPage() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { loginUser } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const result = await register({ name, email, phone, password });
      loginUser(result.token, result.name, result.parentId);
      navigate('/dashboard');
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Registration failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="card auth-card">
        <div style={{ textAlign: 'center', fontSize: '3rem', marginBottom: 8 }}>🌟</div>
        <h2>Join Adhyayan!</h2>
        {error && <div className="error-msg">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>👤 Parent Name</label>
            <input type="text" required value={name} onChange={e => setName(e.target.value)} placeholder="Your full name" />
          </div>
          <div className="form-group">
            <label>📧 Email</label>
            <input type="email" required value={email} onChange={e => setEmail(e.target.value)} placeholder="parent@example.com" />
          </div>
          <div className="form-group">
            <label>📱 Phone</label>
            <input type="tel" value={phone} onChange={e => setPhone(e.target.value)} placeholder="+91 98765 43210" />
          </div>
          <div className="form-group">
            <label>🔒 Password</label>
            <input type="password" required minLength={6} value={password} onChange={e => setPassword(e.target.value)} placeholder="Min 6 characters" />
          </div>
          <button type="submit" className="btn btn-primary btn-lg" style={{ width: '100%' }} disabled={loading}>
            {loading ? '⏳ Creating account...' : '✨ Sign Up'}
          </button>
        </form>
        <p style={{ textAlign: 'center', marginTop: 20, color: 'var(--text-light)', fontWeight: 600 }}>
          Already have an account? <Link to="/login" style={{ color: 'var(--primary)', fontWeight: 700 }}>Login 🚀</Link>
        </p>
      </div>
    </div>
  );
}
