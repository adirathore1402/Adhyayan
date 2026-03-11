import { useState, useEffect } from 'react';
import { getBoards, getGrades, getSubjects, getChapters } from '../services/api';
import type { Board, Grade, Subject, Chapter } from '../services/api';
import { useNavigate } from 'react-router-dom';

export default function PracticePage() {
  const [boards, setBoards] = useState<Board[]>([]);
  const [grades, setGrades] = useState<Grade[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [chapters, setChapters] = useState<Chapter[]>([]);

  const [selectedBoard, setSelectedBoard] = useState<Board | null>(null);
  const [selectedGrade, setSelectedGrade] = useState<Grade | null>(null);
  const [selectedSubject, setSelectedSubject] = useState<Subject | null>(null);
  const [selectedChapter, setSelectedChapter] = useState<Chapter | null>(null);

  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    Promise.all([getBoards(), getSubjects()]).then(([b, s]) => {
      setBoards(b);
      setSubjects(s);
      setLoading(false);
    });
  }, []);

  useEffect(() => {
    if (selectedBoard) {
      setSelectedGrade(null);
      setSelectedSubject(null);
      setSelectedChapter(null);
      getGrades(selectedBoard.id).then(setGrades);
    }
  }, [selectedBoard]);

  useEffect(() => {
    if (selectedBoard && selectedGrade && selectedSubject) {
      setSelectedChapter(null);
      getChapters(selectedBoard.id, selectedGrade.id, selectedSubject.id).then(setChapters);
    }
  }, [selectedBoard, selectedGrade, selectedSubject]);

  function handleStartPractice() {
    if (selectedChapter) {
      navigate(`/practice/session?chapterId=${selectedChapter.id}&chapterName=${encodeURIComponent(selectedChapter.name)}`);
    }
  }

  if (loading) return (
    <div className="loading">
      <div className="loading-spinner" />
      <div className="loading-text">Loading curriculum...</div>
    </div>
  );

  return (
    <div className="page container">
      <h1 className="page-title">📖 Chapter Practice</h1>
      <p className="page-subtitle">Pick your path below — all choices in one place!</p>

      {/* All 4 selectors in a 2x2 grid — no scrolling needed */}
      <div className="practice-selectors">
        {/* Board */}
        <div className={`selector-section anim-slide-up-1 ${selectedBoard ? 'active' : ''}`}>
          <h3>🏫 Board</h3>
          <div className="selector-grid">
            {boards.map(b => (
              <div
                key={b.id}
                className={`selector-item ${selectedBoard?.id === b.id ? 'selected' : ''}`}
                onClick={() => setSelectedBoard(b)}
              >
                {b.name}
                {b.isPrimary && <span className="badge badge-easy" style={{ marginLeft: 6, fontSize: '0.65rem' }}>★</span>}
              </div>
            ))}
          </div>
        </div>

        {/* Grade */}
        <div className={`selector-section anim-slide-up-2 ${selectedGrade ? 'active' : ''}`}>
          <h3>🎓 Class</h3>
          <div className="selector-grid">
            {selectedBoard ? grades.map(g => (
              <div
                key={g.id}
                className={`selector-item ${selectedGrade?.id === g.id ? 'selected' : ''}`}
                onClick={() => setSelectedGrade(g)}
              >
                {g.displayName}
              </div>
            )) : (
              <div style={{ color: 'var(--text-light)', fontWeight: 600, fontSize: '0.85rem', padding: 8 }}>
                ← Pick a board first
              </div>
            )}
          </div>
        </div>

        {/* Subject */}
        <div className={`selector-section anim-slide-up-3 ${selectedSubject ? 'active' : ''}`}>
          <h3>📚 Subject</h3>
          <div className="selector-grid">
            {selectedGrade ? subjects.map(s => (
              <div
                key={s.id}
                className={`selector-item ${selectedSubject?.id === s.id ? 'selected' : ''}`}
                onClick={() => setSelectedSubject(s)}
              >
                {s.name === 'Mathematics' ? '🔢' : s.name === 'English' ? '🔤' : '🌿'} {s.name}
              </div>
            )) : (
              <div style={{ color: 'var(--text-light)', fontWeight: 600, fontSize: '0.85rem', padding: 8 }}>
                ← Pick class first
              </div>
            )}
          </div>
        </div>

        {/* Chapter */}
        <div className={`selector-section anim-slide-up-4 ${selectedChapter ? 'active' : ''}`}>
          <h3>📄 Chapter</h3>
          <div className="selector-grid" style={{ maxHeight: 200, overflowY: 'auto', gridTemplateColumns: '1fr' }}>
            {selectedSubject && chapters.length > 0 ? chapters.map(c => (
              <div
                key={c.id}
                className={`selector-item ${selectedChapter?.id === c.id ? 'selected' : ''}`}
                onClick={() => setSelectedChapter(c)}
                style={{ textAlign: 'left', padding: '8px 12px' }}
              >
                <div style={{ fontWeight: 700, fontSize: '0.85rem' }}>
                  Ch {c.chapterNumber}: {c.name}
                </div>
                {c.topics && c.topics.length > 0 && (
                  <div style={{ fontSize: '0.7rem', color: 'var(--text-light)', marginTop: 2 }}>
                    {c.topics.length} topic{c.topics.length > 1 ? 's' : ''}
                  </div>
                )}
              </div>
            )) : (
              <div style={{ color: 'var(--text-light)', fontWeight: 600, fontSize: '0.85rem', padding: 8 }}>
                {selectedSubject ? 'No chapters found' : '← Pick subject first'}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Start Button */}
      {selectedChapter && (
        <div style={{ textAlign: 'center', marginTop: 24, animation: 'scale-in 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)' }}>
          <button className="btn btn-primary btn-lg" onClick={handleStartPractice}>
            🚀 Start Practice — {selectedChapter.name}
          </button>
        </div>
      )}
    </div>
  );
}
