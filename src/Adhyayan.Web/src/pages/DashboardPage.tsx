import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { getChildren, addChild, getChildProgress, getBoards } from '../services/api';
import type { ChildInfo, ChildProgress, Board } from '../services/api';

export default function DashboardPage() {
  const { parentName } = useAuth();
  const [children, setChildren] = useState<ChildInfo[]>([]);
  const [boards, setBoards] = useState<Board[]>([]);
  const [selectedChild, setSelectedChild] = useState<ChildInfo | null>(null);
  const [progress, setProgress] = useState<ChildProgress | null>(null);
  const [showAddChild, setShowAddChild] = useState(false);
  const [newChildName, setNewChildName] = useState('');
  const [newChildGrade, setNewChildGrade] = useState(1);
  const [newChildBoard, setNewChildBoard] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([getChildren(), getBoards()]).then(([c, b]) => {
      setChildren(c);
      setBoards(b);
      if (b.length > 0) setNewChildBoard(b[0].id);
      setLoading(false);
    }).catch(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (selectedChild) {
      getChildProgress(selectedChild.id).then(setProgress);
    }
  }, [selectedChild]);

  async function handleAddChild() {
    if (!newChildName.trim()) return;
    try {
      await addChild({ name: newChildName, gradeNumber: newChildGrade, boardId: newChildBoard });
      const updated = await getChildren();
      setChildren(updated);
      setShowAddChild(false);
      setNewChildName('');
    } catch {
      // handle error
    }
  }

  if (loading) return <div className="loading">Loading dashboard...</div>;

  return (
    <div className="page container">
      <h1 className="page-title">Parent Dashboard</h1>
      <p className="page-subtitle">Welcome, {parentName}! Track your children's learning progress.</p>

      {/* Children List */}
      <div style={{ display: 'flex', gap: 16, alignItems: 'center', marginBottom: 24 }}>
        <h3>Children</h3>
        <button className="btn btn-outline btn-sm" onClick={() => setShowAddChild(!showAddChild)}>
          + Add Child
        </button>
      </div>

      {showAddChild && (
        <div className="card" style={{ marginBottom: 20, maxWidth: 500 }}>
          <div className="form-group">
            <label>Child's Name</label>
            <input type="text" value={newChildName} onChange={e => setNewChildName(e.target.value)} placeholder="Enter child's name" />
          </div>
          <div className="form-group">
            <label>Class</label>
            <select value={newChildGrade} onChange={e => setNewChildGrade(parseInt(e.target.value))}>
              {[1, 2, 3, 4, 5].map(g => <option key={g} value={g}>Class {g}</option>)}
            </select>
          </div>
          <div className="form-group">
            <label>Board</label>
            <select value={newChildBoard} onChange={e => setNewChildBoard(parseInt(e.target.value))}>
              {boards.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
          </div>
          <button className="btn btn-primary" onClick={handleAddChild}>Add Child</button>
        </div>
      )}

      {children.length === 0 ? (
        <div className="card" style={{ textAlign: 'center', padding: 40 }}>
          <p style={{ color: 'var(--text-light)' }}>No children added yet. Add a child to start tracking progress.</p>
        </div>
      ) : (
        <div className="selector-grid" style={{ marginBottom: 32 }}>
          {children.map(c => (
            <div
              key={c.id}
              className={`selector-item ${selectedChild?.id === c.id ? 'selected' : ''}`}
              onClick={() => setSelectedChild(c)}
            >
              <div style={{ fontSize: '1.1rem', fontWeight: 700 }}>{c.name}</div>
              <div style={{ fontSize: '0.85rem', color: 'var(--text-light)' }}>
                Class {c.gradeNumber} • {c.board.name}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Progress Display */}
      {selectedChild && progress && (
        <div>
          <h3 style={{ marginBottom: 16 }}>
            Progress: {progress.childName} — Class {progress.gradeNumber} ({progress.boardName})
          </h3>

          {progress.chapterProgress.length === 0 ? (
            <div className="card" style={{ textAlign: 'center', padding: 40 }}>
              <p style={{ color: 'var(--text-light)' }}>No practice sessions yet. Start practicing to see progress here!</p>
            </div>
          ) : (
            <div className="card-grid">
              {progress.chapterProgress.map(cp => (
                <div key={cp.chapterId} className="card">
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <div>
                      <div style={{ fontWeight: 700 }}>{cp.chapterName}</div>
                      <div style={{ fontSize: '0.85rem', color: 'var(--text-light)' }}>
                        Class {cp.gradeNumber} – {cp.subjectName}
                      </div>
                    </div>
                    <div className="stat-value" style={{ fontSize: '1.5rem' }}>{cp.accuracyPercent}%</div>
                  </div>
                  <div className="progress-bar" style={{ marginTop: 12 }}>
                    <div className="progress-bar-fill" style={{ width: `${cp.accuracyPercent}%` }} />
                  </div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-light)', marginTop: 6 }}>
                    {cp.correctAnswers}/{cp.totalQuestions} correct
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
