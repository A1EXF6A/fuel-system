import React, { createContext, useContext, useState, useEffect } from 'react';
import { createTheme } from '@mui/material/styles';

const ThemeContext = createContext();

export const useTheme = () => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};

export const ThemeProvider = ({ children }) => {
  const [mode, setMode] = useState('light');

  // Load theme preference from localStorage
  useEffect(() => {
    const savedMode = localStorage.getItem('themeMode');
    if (savedMode) {
      setMode(savedMode);
    } else {
      // Check system preference
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      setMode(prefersDark ? 'dark' : 'light');
    }
  }, []);

  // Save theme preference to localStorage
  useEffect(() => {
    localStorage.setItem('themeMode', mode);
  }, [mode]);

  const toggleTheme = () => {
    setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
  };

  const theme = createTheme({
    palette: {
      mode,
      ...(mode === 'light'
        ? {
            // Neutral, modern light palette
            primary: { main: '#2F3B4A' }, // deep neutral
            secondary: { main: '#6B7280' }, // cool gray
            background: { default: '#F5F7FA', paper: '#FFFFFF' },
            text: { primary: '#1F2937', secondary: '#6B7280' },
            divider: '#E5E7EB'
          }
        : {
            // Neutral, modern dark palette
            primary: { main: '#9AA4B2' },
            secondary: { main: '#A3A3A3' },
            background: { default: '#0F172A', paper: '#111827' },
            text: { primary: '#F9FAFB', secondary: '#D1D5DB' },
            divider: '#374151'
          }),
    },
    typography: {
      fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
      h4: { fontWeight: 700, letterSpacing: 0.2 },
      h5: { fontWeight: 600 },
      h6: { fontWeight: 600 },
      body1: { lineHeight: 1.6 },
      body2: { lineHeight: 1.6 }
    },
    components: {
      MuiAppBar: {
        styleOverrides: {
          root: {
            backgroundColor: mode === 'light' ? '#FFFFFF' : '#111827',
            color: mode === 'light' ? '#1F2937' : '#F9FAFB',
            borderBottom: `1px solid ${mode === 'light' ? '#E5E7EB' : '#374151'}`,
            boxShadow: 'none',
          },
        },
      },
      MuiDrawer: {
        styleOverrides: {
          paper: {
            backgroundColor: mode === 'light' ? '#FFFFFF' : '#111827',
            borderRight: `1px solid ${mode === 'light' ? '#E5E7EB' : '#374151'}`,
            boxShadow: 'none',
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 16,
            boxShadow: mode === 'light'
              ? '0 1px 3px rgba(0,0,0,0.08), 0 1px 2px rgba(0,0,0,0.06)'
              : '0 1px 3px rgba(0,0,0,0.5)',
            transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
            border: mode === 'light' ? '1px solid #E5E7EB' : '1px solid #374151',
            '&:hover': {
              boxShadow: mode === 'light'
                ? '0 4px 12px rgba(0,0,0,0.12)'
                : '0 4px 12px rgba(0,0,0,0.6)',
              transform: 'translateY(-2px)',
            },
          },
        },
      },
      MuiButton: {
        styleOverrides: {
          root: {
            borderRadius: 10,
            textTransform: 'none',
            fontWeight: 500,
            padding: '8px 16px',
            transition: 'all 0.2s ease-in-out',
          },
          contained: {
            boxShadow: mode === 'light'
              ? '0 2px 4px rgba(0,0,0,0.08)'
              : '0 2px 4px rgba(0,0,0,0.5)',
            '&:hover': {
              boxShadow: mode === 'light'
                ? '0 4px 12px rgba(0,0,0,0.12)'
                : '0 4px 12px rgba(0,0,0,0.6)',
              transform: 'translateY(-1px)',
            },
          },
          outlined: {
            borderWidth: 1.5,
            '&:hover': {
              borderWidth: 1.5,
              backgroundColor: mode === 'light'
                ? 'rgba(31, 41, 55, 0.04)'
                : 'rgba(249, 250, 251, 0.04)',
            },
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            borderRadius: 16,
            transition: 'box-shadow 0.3s ease-in-out',
          },
          elevation1: {
            boxShadow: mode === 'light'
              ? '0 2px 8px rgba(0,0,0,0.08)'
              : '0 2px 8px rgba(0,0,0,0.5)',
          },
        },
      },
      MuiTextField: {
        styleOverrides: {
          root: {
            '& .MuiOutlinedInput-root': {
              borderRadius: 12,
              transition: 'all 0.2s ease-in-out',
              '&:hover .MuiOutlinedInput-notchedOutline': {
                borderColor: mode === 'light' ? '#9AA4B2' : '#D1D5DB',
              },
              '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                borderWidth: 2,
              },
            },
          },
        },
      },
      MuiTableContainer: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            boxShadow: mode === 'light'
              ? '0 2px 8px rgba(0,0,0,0.06)'
              : '0 2px 8px rgba(0,0,0,0.5)',
          },
        },
      },
      MuiListItemButton: {
        styleOverrides: {
          root: {
            borderRadius: 8,
            margin: '4px 8px',
            '&:hover': {
              backgroundColor: mode === 'light'
                ? 'rgba(31, 41, 55, 0.06)'
                : 'rgba(255, 255, 255, 0.06)',
            },
            '&.Mui-selected': {
              backgroundColor: mode === 'light'
                ? 'rgba(31, 41, 55, 0.12)'
                : 'rgba(255, 255, 255, 0.12)',
              '&:hover': {
                backgroundColor: mode === 'light'
                  ? 'rgba(31, 41, 55, 0.16)'
                  : 'rgba(255, 255, 255, 0.16)',
              },
            },
          },
        },
      },
    },
  });

  return (
    <ThemeContext.Provider value={{ mode, toggleTheme, theme }}>
      {children}
    </ThemeContext.Provider>
  );
};