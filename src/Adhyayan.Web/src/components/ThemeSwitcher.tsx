import { useState } from 'react';
import { useTheme } from '../context/ThemeContext';

export default function ThemeSwitcher() {
  const { theme, setTheme, themes } = useTheme();
  const [open, setOpen] = useState(false);

  const current = themes.find(t => t.name === theme);

  return (
    <div className="theme-picker">
      {open && (
        <div className="theme-picker-menu">
          {themes.map(t => (
            <div
              key={t.name}
              className={`theme-option ${theme === t.name ? 'active' : ''}`}
              onClick={() => { setTheme(t.name); setOpen(false); }}
            >
              <span className="theme-option-emoji">{t.emoji}</span>
              <span>{t.label}</span>
            </div>
          ))}
        </div>
      )}
      <button
        className="theme-picker-toggle"
        onClick={() => setOpen(!open)}
        title="Change Theme"
        aria-label="Change theme"
      >
        {current?.emoji ?? '🎨'}
      </button>
    </div>
  );
}
