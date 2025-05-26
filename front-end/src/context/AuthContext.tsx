import React, { createContext, useState, useContext, useEffect, useMemo } from 'react';

interface AuthContextType {
  isLoggedIn: boolean;
  login: (id: number) => void;
  logout: () => void;
  userId: number | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isLoggedIn, setIsLoggedIn] = useState(() => localStorage.getItem('isLoggedIn') === 'true');
  const [userId, setUserId] = useState<number | null>(() => {
    const savedUserId = localStorage.getItem('userId');
    return savedUserId ? Number(savedUserId) : null;
  });

  
  useEffect(() => {
    function handleStorageChange(event: StorageEvent) {
      if (event.key === 'isLoggedIn') {
        setIsLoggedIn(event.newValue === 'true');
      }
      if (event.key === 'userId') {
        setUserId(event.newValue ? Number(event.newValue) : null);
      }
    }
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  const login = (id: number) => {
    setIsLoggedIn(true);
    setUserId(id);
    localStorage.setItem('isLoggedIn', 'true');
    localStorage.setItem('userId', id.toString());
  };

  const logout = () => {
    setIsLoggedIn(false);
    setUserId(null);
    localStorage.removeItem('isLoggedIn');
    localStorage.removeItem('userId');
  };

  // Memoized value not have to log in again after page reload
  const value = useMemo(() => ({ isLoggedIn, login, logout, userId }), [isLoggedIn, userId]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
