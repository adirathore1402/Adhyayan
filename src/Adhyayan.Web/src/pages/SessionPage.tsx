import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { getQuestions, generateQuestions } from '../services/api';
import type { Question } from '../services/api';
import { useAuth } from '../context/AuthContext';
import Confetti from '../components/Confetti';

const encouragements = ['Amazing! 🌟', 'Brilliant! ✨', 'Superstar! ⭐', 'Wow! 🎉', 'Genius! 🧠', 'Perfect! 💎'];
const tryAgains = ['Almost there! 💪', 'Keep trying! 🌈', 'You got this! 🚀', 'So close! 🎯'];

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
  const [streak, setStreak] = useState(0);
  const [showConfetti, setShowConfetti] = useState(false);
  const [encouragement, setEncouragement] = useState('');

  useEffect(() => {
    if (!chapterId) return;
    loadQuestions();
  }, [chapterId]);

  async function loadQuestions() {
    setLoading(true);
    try {
      let qs = await getQuestions(chapterId, undefined, 10);
      if (qs.length === 0 && isLoggedIn) {
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
      const newStreak = streak + 1;
      setStreak(newStreak);
      setEncouragement(encouragements[Math.floor(Math.random() * encouragements.length)]);
      if (newStreak >= 3) {
        setShowConfetti(true);
        setTimeout(() => setShowConfetti(false), 3000);
      }
    } else {
      setStreak(0);
      setEncouragement(tryAgains[Math.floor(Math.random() * tryAgains.length)]);
    }
  }

  function handleNext() {
    if (currentIndex + 1 >= questions.length) {
      setFinished(true);
      setShowConfetti(true);
      setTimeout(() => setShowConfetti(false), 4000);
    } else {
      setCurrentIndex(prev => prev + 1);
      setSelectedAnswer(null);
      setShowResult(false);
      setEncouragement('');
    }
  }

  if (loading) {
    return (
      <div className="loading">
        <div className="loading-spinner" />
        <div className="loading-text">
          {generating ? '🤖 AI is creating questions just for you...' : 'Loading questions...'}
        </div>
      </div>
    );
  }

  if (questions.length === 0) {
    return (
      <div className="page container" style={{ textAlign: 'center' }}>
        <div style={{ fontSize: '4rem', marginBottom: 16 }}>😕</div>
        <h2>No questions available</h2>
        <p className="page-subtitle" style={{ maxWidth: 400, margin: '16px auto' }}>
          {isLoggedIn
            ? 'Could not generate questions. Check Azure OpenAI configuration and try again.'
            : 'Login to unlock AI-powered question generation!'}
        </p>
        <button className="btn btn-primary btn-lg" onClick={() => navigate('/practice')}>← Back to Practice</button>
      </div>
    );
  }

  if (finished) {
    const accuracy = totalAnswered > 0 ? Math.round((score / totalAnswered) * 100) : 0;
    let emoji = '🎉';
    let message = 'Great effort!';
    if (accuracy >= 90) { emoji = '🏆'; message = 'Outstanding! You\'re a champion!'; }
    else if (accuracy >= 70) { emoji = '🌟'; message = 'Excellent work! Keep it up!'; }
    else if (accuracy >= 50) { emoji = '👍'; message = 'Good job! Practice makes perfect!'; }
    else { emoji = '💪'; message = 'Keep practicing! You\'ll get better!'; }

    return (
      <div className="page container completion-page">
        <Confetti active={showConfetti} />
        <span className="completion-emoji">{emoji}</span>
        <h2 style={{ fontFamily: 'var(--font-display)', fontSize: '1.8rem' }}>Practice Complete!</h2>
        <p className="page-subtitle">{chapterName}</p>
        <div className="card score-card">
          <div className="stat-value" style={{ fontSize: '3.5rem' }}>{accuracy}%</div>
          <div className="stat-label" style={{ fontSize: '1rem', marginBottom: 12 }}>{message}</div>
          <div className="progress-bar" style={{ marginTop: 16, height: 16 }}>
            <div className="progress-bar-fill" style={{ width: `${accuracy}%` }} />
          </div>
          <p style={{ marginTop: 14, color: 'var(--text-light)', fontWeight: 700 }}>
            {score} correct out of {totalAnswered} questions
          </p>
        </div>
        <div style={{ display: 'flex', gap: 16, justifyContent: 'center', marginTop: 24, flexWrap: 'wrap' }}>
          <button className="btn btn-primary btn-lg" onClick={() => navigate('/practice')}>📖 Practice More</button>
          <button className="btn btn-outline btn-lg" onClick={() => {
            setFinished(false); setCurrentIndex(0); setScore(0); setTotalAnswered(0);
            setSelectedAnswer(null); setShowResult(false); setStreak(0); setEncouragement('');
          }}>
            🔄 Retry
          </button>
        </div>
      </div>
    );
  }

  const currentQ = questions[currentIndex];
  const isCorrect = selectedAnswer === currentQ.correctAnswer;

  return (
    <div className="page container">
      <Confetti active={showConfetti} />

      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
        <span style={{ color: 'var(--text-light)', fontWeight: 700, fontSize: '0.9rem' }}>📖 {chapterName}</span>
        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          {streak >= 2 && (
            <span className="streak-badge">🔥 {streak} streak!</span>
          )}
          <span style={{ color: 'var(--text-light)', fontWeight: 700 }}>
            {currentIndex + 1} / {questions.length}
          </span>
        </div>
      </div>

      <div className="progress-bar" style={{ marginBottom: 24 }}>
        <div className="progress-bar-fill" style={{ width: `${((currentIndex + 1) / questions.length) * 100}%` }} />
      </div>

      <div className="question-card">
        <div style={{ marginBottom: 12, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <span className={`badge badge-${currentQ.difficulty}`}>{currentQ.difficulty}</span>
          <span style={{ color: 'var(--text-light)', fontWeight: 700, fontSize: '0.85rem' }}>
            Score: {score}/{totalAnswered}
          </span>
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
            {encouragement && (
              <div style={{
                textAlign: 'center',
                fontSize: '1.3rem',
                fontFamily: 'var(--font-display)',
                color: isCorrect ? 'var(--success)' : 'var(--primary)',
                marginTop: 16,
                animation: 'scale-in 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)'
              }}>
                {encouragement}
              </div>
            )}
            <div className="explanation-box" style={{ marginTop: 12 }}>
              <strong style={{ color: isCorrect ? 'var(--success)' : 'var(--danger)' }}>
                {isCorrect ? '✅ Correct!' : '❌ Incorrect'}
              </strong>
              <p style={{ marginTop: 8, fontWeight: 600 }}>{currentQ.explanation}</p>
            </div>
            <div style={{ textAlign: 'center', marginTop: 20 }}>
              <button className="btn btn-primary btn-lg" onClick={handleNext}>
                {currentIndex + 1 >= questions.length ? '🏁 See Results' : 'Next Question →'}
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
