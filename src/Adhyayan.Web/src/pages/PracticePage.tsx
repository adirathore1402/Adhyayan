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

  if (loading) return <div className="loading">Loading curriculum...</div>;

  return (
    <div className="page container">
      <h1 className="page-title">Chapter Practice</h1>
      <p className="page-subtitle">Choose your board, class, subject, and chapter to start practicing.</p>

      {/* Step 1: Choose Board */}
      <h3>1. Choose Board</h3>
      <div className="selector-grid">
        {boards.map(b => (
          <div
            key={b.id}
            className={`selector-item ${selectedBoard?.id === b.id ? 'selected' : ''}`}
            onClick={() => setSelectedBoard(b)}
          >
            {b.name}
            {b.isPrimary && <span className="badge badge-easy" style={{ marginLeft: 8 }}>Primary</span>}
          </div>
        ))}
      </div>

      {/* Step 2: Choose Grade */}
      {selectedBoard && (
        <>
          <h3>2. Choose Class</h3>
          <div className="selector-grid">
            {grades.map(g => (
              <div
                key={g.id}
                className={`selector-item ${selectedGrade?.id === g.id ? 'selected' : ''}`}
                onClick={() => setSelectedGrade(g)}
              >
                {g.displayName}
              </div>
            ))}
          </div>
        </>
      )}

      {/* Step 3: Choose Subject */}
      {selectedGrade && (
        <>
          <h3>3. Choose Subject</h3>
          <div className="selector-grid">
            {subjects.map(s => (
              <div
                key={s.id}
                className={`selector-item ${selectedSubject?.id === s.id ? 'selected' : ''}`}
                onClick={() => setSelectedSubject(s)}
              >
                {s.name}
              </div>
            ))}
          </div>
        </>
      )}

      {/* Step 4: Choose Chapter */}
      {selectedSubject && chapters.length > 0 && (
        <>
          <h3>4. Choose Chapter</h3>
          <div className="selector-grid">
            {chapters.map(c => (
              <div
                key={c.id}
                className={`selector-item ${selectedChapter?.id === c.id ? 'selected' : ''}`}
                onClick={() => setSelectedChapter(c)}
              >
                <div>Ch {c.chapterNumber}: {c.name}</div>
                {c.topics && c.topics.length > 0 && (
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-light)', marginTop: 4 }}>
                    {c.topics.length} topic{c.topics.length > 1 ? 's' : ''}
                  </div>
                )}
              </div>
            ))}
          </div>
        </>
      )}

      {/* Start Button */}
      {selectedChapter && (
        <div style={{ textAlign: 'center', marginTop: 32 }}>
          <button className="btn btn-primary" onClick={handleStartPractice} style={{ fontSize: '1.1rem', padding: '14px 40px' }}>
            Start Practice — {selectedChapter.name}
          </button>
        </div>
      )}
    </div>
  );
}
