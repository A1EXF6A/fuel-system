import React, { useContext } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import {
  Drawer,
  Toolbar,
  Box,
  Typography,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from '@mui/material';
import {
  People,
  DriveEta,
  LocalShipping,
  Route,
  Assessment,
  Logout,
  Person
} from '@mui/icons-material';

const drawerWidth = 240;

const menuItemsAdmin = [
  { text: 'Usuarios', icon: <People />, path: '/users' },
  { text: 'Choferes', icon: <DriveEta />, path: '/drivers' },
  { text: 'Vehículos', icon: <LocalShipping />, path: '/vehicles' },
  { text: 'Rutas', icon: <Route />, path: '/routes' },
  { text: 'Reportes', icon: <Assessment />, path: '/reports' }
];

const menuItemsOperador = [
  { text: 'Vehículo', icon: <LocalShipping />, path: '/vehicles' },
  { text: 'Ruta', icon: <Route />, path: '/routes' },
  { text: 'Reporte', icon: <Assessment />, path: '/reports' }
];

const Sidebar = () => {
  const { user, logout } = useContext(AuthContext);
  const navigate = useNavigate();
  const location = useLocation();

  if (!user) return null;

  const menuItems = user.role === 'Operador' ? menuItemsOperador : menuItemsAdmin;

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
      }}
    >
      <Toolbar />
      <Box sx={{ px: 2, py: 2, textAlign: 'center' }}>
        <Person sx={{ color: '#6366f1', fontSize: 40, mb: 2 }} />
        <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
          {user.username}
        </Typography>
        <Typography variant="body2" sx={{ color: '#6366f1', fontWeight: 500 }}>
          {user.role}
        </Typography>
      </Box>
      <Box sx={{ overflow: 'auto' }}>
        <List>
          {menuItems.map((item) => (
            <ListItem key={item.text} disablePadding>
              <ListItemButton
                selected={location.pathname === item.path}
                onClick={() => navigate(item.path)}
              >
                <ListItemIcon>
                  {item.icon}
                </ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Box>
    </Drawer>
  );
};

export default Sidebar;
