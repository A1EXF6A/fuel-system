import React, { createContext, useState, useEffect } from 'react';
import axios from 'axios';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      validateToken(token);
    } else {
      setLoading(false);
    }
  }, []);

  const login = async (username, password) => {
    try {
      const response = await axios.post('http://localhost:5010/api/auth/login', {
        username,
        password
      });
      const { token, role } = response.data;
      localStorage.setItem('token', token);
      setUser({ username, role });
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      return { success: true };
    } catch (error) {
      return { success: false, message: error.response?.data?.message || 'Login failed' };
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    delete axios.defaults.headers.common['Authorization'];
  };

  const validateToken = async (token) => {
    try {
      const response = await axios.post('http://localhost:5010/api/auth/validate', { token });
      if (response.data.valid) {
        setUser({ role: response.data.role });
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      } else {
        logout();
      }
    } catch (error) {
      logout();
    }
    setLoading(false);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};