import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';

export type ThemeName = 'candy' | 'ocean' | 'jungle' | 'space' | 'sunset';

interface ThemeContextType {
  theme: ThemeName;
  setTheme: (t: ThemeName) => void;
  themes: { name: ThemeName; label: string; emoji: string }[];
}

const themeList: { name: ThemeName; label: string; emoji: string }[] = [
  { name: 'candy', label: 'Candy Land', emoji: '🍭' },
  { name: 'ocean', label: 'Ocean World', emoji: '🐠' },
  { name: 'jungle', label: 'Jungle Safari', emoji: '🦁' },
  { name: 'space', label: 'Space Quest', emoji: '🚀' },
  { name: 'sunset', label: 'Sunset Glow', emoji: '🌅' },
];

const ThemeContext = createContext<ThemeContextType>({
  theme: 'candy',
  setTheme: () => {},
  themes: themeList,
});

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setThemeState] = useState<ThemeName>(() => {
    return (localStorage.getItem('adhyayan_theme') as ThemeName) || 'candy';
  });

  function setTheme(t: ThemeName) {
    setThemeState(t);
    localStorage.setItem('adhyayan_theme', t);
  }

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  return (
    <ThemeContext.Provider value={{ theme, setTheme, themes: themeList }}>
      {children}
    </ThemeContext.Provider>
  );
}

export function useTheme() {
  return useContext(ThemeContext);
}
