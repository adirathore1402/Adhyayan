import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { getQuestions, generateQuestions } from '../services/api';
import type { Question } from '../services/api';
import { useAuth } from '../context/AuthContext';

export default function SessionPage() {
  const [searchParams] = useSearchParams();
  const chapterId = parseInt(searchParams.get('chapterId') ?? '0');
  const chapterName = searchParams.get('chapterName') ?? 'Practice';
  const navigate = useNavigate();
  const { isLoggedIn } = useAuth();

  const [questions, setQuestions] = useState<Question[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [selectedAnswer, setSelectedAnswer] = useState<string | null>(null);
  const [showResult, setShowResult] = useState(false);
  const [score, setScore] = useState(0);
  const [totalAnswered, setTotalAnswered] = useState(0);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [finished, setFinished] = useState(false);

  useEffect(() => {
    if (!chapterId) return;
    loadQuestions();
  }, [chapterId]);

  async function loadQuestions() {
    setLoading(true);
    try {
      let qs = await getQuestions(chapterId, undefined, 10);
      if (qs.length === 0 && isLoggedIn) {
        // No questions in DB yet — generate via AI
        setGenerating(true);
        qs = await generateQuestions(chapterId, 'easy', 5);
        setGenerating(false);
      }
      setQuestions(qs);
    } catch {
      setQuestions([]);
    } finally {
      setLoading(false);
    }
  }

  function handleAnswer(answer: string) {
    if (showResult) return;
    setSelectedAnswer(answer);
    setShowResult(true);
    setTotalAnswered(prev => prev + 1);

    const currentQ = questions[currentIndex];
    if (answer === currentQ.correctAnswer) {
      setScore(prev => prev + 1);
    }
  }

  function handleNext() {
    if (currentIndex + 1 >= questions.length) {
      setFinished(true);
    } else {
      setCurrentIndex(prev => prev + 1);
      setSelectedAnswer(null);
      setShowResult(false);
    }
  }

  if (loading) {
    return (
      <div className="loading">
        {generating ? '🤖 Generating questions with AI...' : 'Loading questions...'}
      </div>
    );
  }

  if (questions.length === 0) {
    return (
      <div className="page container" style={{ textAlign: 'center' }}>
        <h2>No questions available</h2>
        <p style={{ color: 'var(--text-light)', margin: '16px 0' }}>
          {isLoggedIn
            ? 'Failed to generate questions. Please check OpenAI configuration and try again.'
            : 'Login to generate AI questions for this chapter.'}
        </p>
        <button className="btn btn-primary" onClick={() => navigate('/practice')}>Back to Practice</button>
      </div>
    );
  }

  if (finished) {
    const accuracy = totalAnswered > 0 ? Math.round((score / totalAnswered) * 100) : 0;
    return (
      <div className="page container" style={{ textAlign: 'center' }}>
        <h1 style={{ fontSize: '3rem', marginBottom: 16 }}>🎉</h1>
        <h2>Practice Complete!</h2>
        <p className="page-subtitle">{chapterName}</p>
        <div className="card" style={{ maxWidth: 400, margin: '24px auto', padding: 32 }}>
          <div className="stat-value">{accuracy}%</div>
          <div className="stat-label">Accuracy</div>
          <div className="progress-bar" style={{ marginTop: 16 }}>
            <div className="progress-bar-fill" style={{ width: `${accuracy}%` }} />
          </div>
          <p style={{ marginTop: 12, color: 'var(--text-light)' }}>
            {score} correct out of {totalAnswered} questions
          </p>
        </div>
        <div style={{ display: 'flex', gap: 16, justifyContent: 'center', marginTop: 24 }}>
          <button className="btn btn-primary" onClick={() => navigate('/practice')}>Practice More</button>
          <button className="btn btn-outline" onClick={() => { setFinished(false); setCurrentIndex(0); setScore(0); setTotalAnswered(0); setSelectedAnswer(null); setShowResult(false); }}>
            Retry
          </button>
        </div>
      </div>
    );
  }

  const currentQ = questions[currentIndex];
  const isCorrect = selectedAnswer === currentQ.correctAnswer;

  return (
    <div className="page container">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
        <span style={{ color: 'var(--text-light)', fontWeight: 600 }}>{chapterName}</span>
        <span style={{ color: 'var(--text-light)' }}>
          Question {currentIndex + 1} of {questions.length}
        </span>
      </div>

      <div className="progress-bar" style={{ marginBottom: 24 }}>
        <div className="progress-bar-fill" style={{ width: `${((currentIndex + 1) / questions.length) * 100}%` }} />
      </div>

      <div className="question-card">
        <div style={{ marginBottom: 8 }}>
          <span className={`badge badge-${currentQ.difficulty}`}>{currentQ.difficulty}</span>
        </div>
        <div className="question-text">{currentQ.questionText}</div>
        <ul className="options-list">
          {currentQ.options.map((opt, i) => {
            let className = 'option-btn';
            if (showResult) {
              if (opt.startsWith(currentQ.correctAnswer)) className += ' correct';
              else if (opt === selectedAnswer || opt.startsWith(selectedAnswer ?? '')) className += ' incorrect';
            } else if (selectedAnswer === opt) {
              className += ' selected';
            }

            return (
              <li key={i}>
                <button className={className} onClick={() => handleAnswer(opt.charAt(0))}>
                  {opt}
                </button>
              </li>
            );
          })}
        </ul>

        {showResult && (
          <>
            <div className="explanation-box" style={{ marginTop: 20 }}>
              <strong>{isCorrect ? '✅ Correct!' : '❌ Incorrect'}</strong>
              <p style={{ marginTop: 8 }}>{currentQ.explanation}</p>
            </div>
            <div style={{ textAlign: 'center', marginTop: 20 }}>
              <button className="btn btn-primary" onClick={handleNext}>
                {currentIndex + 1 >= questions.length ? 'See Results' : 'Next Question →'}
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
