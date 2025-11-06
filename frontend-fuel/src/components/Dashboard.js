import React, { useContext, useEffect, useState, useCallback } from 'react';
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
   const [operatorData, setOperatorData] = useState({ vehicle: null, route: null, reports: [] });

   const fetchOperatorData = useCallback(async () => {
     try {
       // Fetch driver info to get assigned vehicle
       const driverResponse = await axios.get('http://localhost:5010/api/drivers');
       let drivers = [];
       if (Array.isArray(driverResponse.data)) {
         drivers = driverResponse.data;
       } else if (driverResponse.data.Drivers) {
         drivers = driverResponse.data.Drivers;
       }
       const driver = drivers.find(d => d.documentNumber === user.username);

       let vehicle = null;
       let route = null;
       let reports = [];

       if (driver && driver.assignedVehiclePlaca) {
         // Fetch assigned vehicle
         const vehicleResponse = await axios.get('http://localhost:5010/api/vehicles');
         const allVehicles = vehicleResponse.data.vehicles || [];
         vehicle = allVehicles.find(v => v.placa === driver.assignedVehiclePlaca);

         // Fetch assigned route
         const routeResponse = await axios.get('http://localhost:5010/api/routes');
         const allRoutes = routeResponse.data.routes || routeResponse.data.Routes || [];
         route = allRoutes.find(r => r.vehiclePlaca === driver.assignedVehiclePlaca);

         // Fetch reports for the vehicle
         const reportsResponse = await axios.get('http://localhost:5010/api/fuel/reports');
         reports = reportsResponse.data.registros || [];
         reports = reports.filter(r => r.vehiclePlaca === driver.assignedVehiclePlaca);
       }

       setOperatorData({ vehicle, route, reports });
     } catch (error) {
       console.error('Error fetching operator data:', error);
       setOperatorData({ vehicle: null, route: null, reports: [] });
     }
   }, [user]);

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
         )}
       </Box>
    </Box>
  );
};

export default Dashboard;