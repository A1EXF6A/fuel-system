import React, { useContext, useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import axios from 'axios';
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Grid,
  Card,
  CardContent,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText
} from '@mui/material';
import LocalShipping from '@mui/icons-material/LocalShipping';
import Route from '@mui/icons-material/AltRoute';
import Assessment from '@mui/icons-material/Assessment';
import People from '@mui/icons-material/People';
import DriveEta from '@mui/icons-material/DriveEta';
import Logout from '@mui/icons-material/Logout';
import Sidebar from './Sidebar';
// ...existing code...

function Dashboard() {
  const { user, loading, logout } = useContext(AuthContext);
  const navigate = useNavigate();
  const [operatorData, setOperatorData] = useState({ vehicle: null, route: null, reports: [] });
  const [stats, setStats] = useState({ drivers: 0, vehicles: 0, users: 0, routes: 0 });
  const drawerWidth = 240;

  const fetchOperatorData = useCallback(async () => {
    try {
      // Example API calls for operator data
      const vehicleRes = await axios.get('http://localhost:5010/api/vehicles/assigned');
      const routeRes = await axios.get('http://localhost:5010/api/routes/assigned');
      const reportsRes = await axios.get('http://localhost:5010/api/reports/operator');
      setOperatorData({
        vehicle: vehicleRes.data.vehicle || null,
        route: routeRes.data.route || null,
        reports: reportsRes.data.reports || []
      });
    } catch (error) {
      console.error('Error fetching operator data:', error);
    }
  }, []);
  useEffect(() => {
    if (!loading && !user) {
      navigate('/login');
    } else if (user && user.role === 'Operador') {
      fetchOperatorData();
    } else if (user && (user.role === 'Admin' || user.role === 'Supervisor')) {
      fetchStats();
    }
  }, [user, loading, navigate, fetchOperatorData]);

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

  const menuItems = user.role === 'Operador' ? [
    { text: 'Vehículo', icon: <LocalShipping />, path: '/vehicles' },
    { text: 'Ruta', icon: <Route />, path: '/routes' },
    { text: 'Reporte', icon: <Assessment />, path: '/reports' }
  ] : [
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
             Fuel System - {user.role === 'Operador' ? 'Panel del Operador' : 'Admin Dashboard'}
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
           {user.role === 'Operador' ? 'Panel del Operador' : 'Dashboard'}
         </Typography>
         {user.role === 'Operador' ? (
           operatorData.vehicle || operatorData.route || operatorData.reports.length > 0 ? (
             <Grid container spacing={3}>
               {operatorData.vehicle && (
                 <Grid item xs={12} sm={6} md={4}>
                   <Card>
                     <CardContent>
                       <Typography color="textSecondary" gutterBottom>
                         Vehículo Asignado
                       </Typography>
                       <Typography variant="h6">
                         {operatorData.vehicle.placa}
                       </Typography>
                       <Typography variant="body2">
                         {operatorData.vehicle.marca} {operatorData.vehicle.modelo}
                       </Typography>
                       <Typography variant="body2">
                         Estado: {operatorData.vehicle.estado}
                       </Typography>
                     </CardContent>
                   </Card>
                 </Grid>
               )}
               {operatorData.route && (
                 <Grid item xs={12} sm={6} md={4}>
                   <Card>
                     <CardContent>
                       <Typography color="textSecondary" gutterBottom>
                         Ruta Asignada
                       </Typography>
                       <Typography variant="h6">
                         {operatorData.route.nombre}
                       </Typography>
                       <Typography variant="body2">
                         {operatorData.route.origen} → {operatorData.route.destino}
                       </Typography>
                       <Typography variant="body2">
                         Estado: {operatorData.route.estado}
                       </Typography>
                     </CardContent>
                   </Card>
                 </Grid>
               )}
               <Grid item xs={12} sm={6} md={4}>
                 <Card>
                   <CardContent>
                     <Typography color="textSecondary" gutterBottom>
                       Reportes de Combustible
                     </Typography>
                     <Typography variant="h5">
                       {operatorData.reports.length}
                     </Typography>
                     <Typography variant="body2">
                       Reportes disponibles
                     </Typography>
                   </CardContent>
                 </Card>
               </Grid>
             </Grid>
           ) : (
             <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '50vh' }}>
               <Typography variant="h6" color="textSecondary">
                 No tienes asignaciones activas. Contacta con tu supervisor para obtener una asignación.
               </Typography>
             </Box>
           )
         ) : (
           <Grid container spacing={3}>
             <Grid item xs={12} sm={6} md={3}>
               <Card>
                 <CardContent sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                   <DriveEta sx={{ fontSize: 40, color: '#6366f1', mb: 1 }} />
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
                 <CardContent sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                   <LocalShipping sx={{ fontSize: 40, color: '#6366f1', mb: 1 }} />
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
                 <CardContent sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                   <People sx={{ fontSize: 40, color: '#6366f1', mb: 1 }} />
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
                 <CardContent sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                   <Route sx={{ fontSize: 40, color: '#6366f1', mb: 1 }} />
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
         )}
       </Box>
    </Box>
  );
};

export default Dashboard;