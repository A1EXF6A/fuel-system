import React, { useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import axios from 'axios';
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  List,
  Typography,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Grid,
  Card,
  CardContent,
  IconButton
} from '@mui/material';
import {
  People,
  DriveEta,
  LocalShipping,
  Route,
  Assessment,
  Logout
} from '@mui/icons-material';

const drawerWidth = 240;

const Dashboard = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
  const [stats, setStats] = useState({ drivers: 0, vehicles: 0, users: 0, routes: 0 });

   useEffect(() => {
     if (!loading && !user) {
       navigate('/login');
     } else if (user && user.role === 'Operador') {
       return;
     } else if (user && (user.role === 'Admin' || user.role === 'Supervisor')) {
       fetchStats();
     }
   }, [user, loading, navigate]);

  const fetchStats = async () => {
    try {
      const [driversRes, vehiclesRes, usersRes, routesRes] = await Promise.all([
        axios.get('http://localhost:5010/api/drivers'),
        axios.get('http://localhost:5010/api/vehicles'),
        axios.get('http://localhost:5010/api/auth/users'),
        axios.get('http://localhost:5010/api/routes')
      ]);
      setStats({
        drivers: driversRes.data.length,
        vehicles: vehiclesRes.data.Vehicles?.length || 0,
        users: usersRes.data.length,
        routes: routesRes.data.Routes?.length || 0
      });
    } catch (error) {
      console.error('Error fetching stats:', error);
    }
  };

  if (loading) return <div>Loading...</div>;

  if (!user) return null;

   if (user.role === 'Operador') {
     return (
       <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
         <Typography variant="h4">Se está implementando</Typography>
       </Box>
     );
   }

  const menuItems = [
    { text: 'Usuarios', icon: <People />, path: '/users' },
    { text: 'Choferes', icon: <DriveEta />, path: '/drivers' },
    { text: 'Vehículos', icon: <LocalShipping />, path: '/vehicles' },
    { text: 'Rutas', icon: <Route />, path: '/routes' },
    { text: 'Reportes', icon: <Assessment />, path: '/reports' }
  ];

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            Fuel System - Admin Dashboard
          </Typography>
          <IconButton color="inherit" onClick={logout}>
            <Logout />
          </IconButton>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            {menuItems.map((item) => (
              <ListItem key={item.text} disablePadding>
                <ListItemButton onClick={() => navigate(item.path)}>
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
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        <Typography variant="h4" gutterBottom>
          Dashboard
        </Typography>
        <Grid container spacing={3}>
          <Grid item xs={12} sm={6} md={3}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Conductores
                </Typography>
                <Typography variant="h5">
                  {stats.drivers}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Vehículos
                </Typography>
                <Typography variant="h5">
                  {stats.vehicles}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Usuarios
                </Typography>
                <Typography variant="h5">
                  {stats.users}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Rutas
                </Typography>
                <Typography variant="h5">
                  {stats.routes}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Box>
    </Box>
  );
};

export default Dashboard;