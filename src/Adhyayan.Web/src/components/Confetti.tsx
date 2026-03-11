import { useEffect, useState, useCallback } from 'react';

interface ConfettiPiece {
  id: number;
  left: number;
  color: string;
  size: number;
  duration: number;
  delay: number;
  shape: 'circle' | 'square' | 'star';
}

const COLORS = ['#e84393', '#a29bfe', '#55efc4', '#fdcb6e', '#ff7675', '#74b9ff', '#fd79a8', '#00cec9'];

export default function Confetti({ active }: { active: boolean }) {
  const [pieces, setPieces] = useState<ConfettiPiece[]>([]);

  const generatePieces = useCallback(() => {
    const newPieces: ConfettiPiece[] = [];
    for (let i = 0; i < 50; i++) {
      newPieces.push({
        id: i,
        left: Math.random() * 100,
        color: COLORS[Math.floor(Math.random() * COLORS.length)],
        size: Math.random() * 12 + 6,
        duration: Math.random() * 2 + 1.5,
        delay: Math.random() * 0.8,
        shape: ['circle', 'square', 'star'][Math.floor(Math.random() * 3)] as ConfettiPiece['shape'],
      });
    }
    return newPieces;
  }, []);

  useEffect(() => {
    if (active) {
      setPieces(generatePieces());
      const timer = setTimeout(() => setPieces([]), 4000);
      return () => clearTimeout(timer);
    } else {
      setPieces([]);
    }
  }, [active, generatePieces]);

  if (pieces.length === 0) return null;

  return (
    <div className="confetti-container">
      {pieces.map(p => (
        <div
          key={p.id}
          className="confetti-piece"
          style={{
            left: `${p.left}%`,
            width: p.size,
            height: p.size,
            backgroundColor: p.color,
            borderRadius: p.shape === 'circle' ? '50%' : p.shape === 'star' ? '2px' : '2px',
            animationDuration: `${p.duration}s`,
            animationDelay: `${p.delay}s`,
            transform: p.shape === 'star' ? 'rotate(45deg)' : undefined,
          }}
        />
      ))}
    </div>
  );
}
