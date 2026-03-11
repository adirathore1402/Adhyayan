import { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface AuthContextType {
  isLoggedIn: boolean;
  parentName: string;
  parentId: number;
  token: string;
  loginUser: (token: string, name: string, parentId: number) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({
  isLoggedIn: false,
  parentName: '',
  parentId: 0,
  token: '',
  loginUser: () => {},
  logout: () => {},
});

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState(() => localStorage.getItem('adhyayan_token') ?? '');
  const [parentName, setParentName] = useState(() => localStorage.getItem('adhyayan_name') ?? '');
  const [parentId, setParentId] = useState(() => parseInt(localStorage.getItem('adhyayan_parentId') ?? '0'));

  const isLoggedIn = !!token;

  const loginUser = (newToken: string, name: string, id: number) => {
    setToken(newToken);
    setParentName(name);
    setParentId(id);
    localStorage.setItem('adhyayan_token', newToken);
    localStorage.setItem('adhyayan_name', name);
    localStorage.setItem('adhyayan_parentId', id.toString());
  };

  const logout = () => {
    setToken('');
    setParentName('');
    setParentId(0);
    localStorage.removeItem('adhyayan_token');
    localStorage.removeItem('adhyayan_name');
    localStorage.removeItem('adhyayan_parentId');
  };

  useEffect(() => {
    // Token already loaded from localStorage in state initializers
  }, []);

  return (
    <AuthContext.Provider value={{ isLoggedIn, parentName, parentId, token, loginUser, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
