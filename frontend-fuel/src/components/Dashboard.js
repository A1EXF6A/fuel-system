import React, { useContext, useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import axios from 'axios';
import { API_ENDPOINTS } from '../config/api';
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
  IconButton,
  Chip,
  LinearProgress,
  Avatar,
  Divider,
  Paper,
  Stack,
  Badge
} from '@mui/material';
import {
  People,
  DriveEta,
  LocalShipping,
  Route,
  Assessment,
  Logout,
  TrendingUp,
  DirectionsCar,
  Person,
  Map,
  BarChart,
  Timeline,
  Speed,
  LocationOn,
  CheckCircle,
  Schedule,
  Assignment,
  Dashboard as DashboardIcon,
  Engineering,
  SupervisorAccount
} from '@mui/icons-material';
import ThemeToggle from './ThemeToggle';

const drawerWidth = 240;

// Componente para cards métricas modernas
const MetricCard = ({ title, value, subtitle, icon, trend }) => (
  <Card
    sx={{
      height: '100%',
      bgcolor: 'background.paper',
      color: 'text.primary',
      position: 'relative',
      overflow: 'hidden',
      transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
      border: (theme) => `1px solid ${theme.palette.divider}`,
      '&:hover': {
        transform: 'translateY(-4px)',
        boxShadow: (theme) => theme.shadows[4],
      }
    }}
  >
    <CardContent sx={{ p: 3, position: 'relative', zIndex: 1 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
        <Avatar sx={{ bgcolor: 'action.hover', color: 'text.primary', width: 48, height: 48 }}>
          {icon}
        </Avatar>
        {trend && (
          <Chip
            label={`${trend > 0 ? '+' : ''}${trend}%`}
            size="small"
            color={trend > 0 ? 'success' : 'error'}
            variant="outlined"
          />
        )}
      </Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 1 }}>
        {value}
      </Typography>
      <Typography variant="h6" sx={{ fontWeight: 600, mb: 1 }}>
        {title}
      </Typography>
      {subtitle && (
        <Typography variant="body2" color="text.secondary">
          {subtitle}
        </Typography>
      )}
    </CardContent>
  </Card>
);

// Componente para cards de información del operador
const OperatorInfoCard = ({ title, value, subtitle, icon, status, statusColor = 'default' }) => (
  <Card
    sx={{
      height: '100%',
      borderRadius: 3,
      transition: 'all 0.3s ease',
      '&:hover': {
        transform: 'translateY(-2px)',
        boxShadow: '0 8px 25px rgba(0,0,0,0.1)',
      }
    }}
  >
    <CardContent sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
        <Avatar sx={{ bgcolor: 'primary.main', mr: 2 }}>
          {icon}
        </Avatar>
        <Box sx={{ flex: 1 }}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            {title}
          </Typography>
          {status && (
            <Chip
              label={status}
              size="small"
              color={statusColor}
              sx={{ mt: 0.5 }}
            />
          )}
        </Box>
      </Box>
      <Typography variant="h5" sx={{ fontWeight: 700, color: 'primary.main', mb: 1 }}>
        {value}
      </Typography>
      {subtitle && (
        <Typography variant="body2" color="text.secondary">
          {subtitle}
        </Typography>
      )}
    </CardContent>
  </Card>
);

const Dashboard = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
   const [stats, setStats] = useState({ drivers: 0, vehicles: 0, users: 0, routes: 0 });
   const [operatorData, setOperatorData] = useState({ vehicle: null, route: null, reports: [] });

   const fetchOperatorData = useCallback(async () => {
     try {
       // Fetch driver info to get assigned vehicle
       const driverResponse = await axios.get(API_ENDPOINTS.DRIVERS.BASE);
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
         const vehicleResponse = await axios.get(API_ENDPOINTS.VEHICLES.BASE);
         const allVehicles = vehicleResponse.data.vehicles || [];
         vehicle = allVehicles.find(v => v.placa === driver.assignedVehiclePlaca);

         // Fetch assigned route
         const routeResponse = await axios.get(API_ENDPOINTS.ROUTES.BASE);
         const allRoutes = routeResponse.data.routes || routeResponse.data.Routes || [];
         route = allRoutes.find(r => r.vehiclePlaca === driver.assignedVehiclePlaca);

         // Fetch reports for the vehicle
         const reportsResponse = await axios.get(API_ENDPOINTS.FUEL.REPORTS);
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
         axios.get(API_ENDPOINTS.DRIVERS.BASE),
         axios.get(API_ENDPOINTS.VEHICLES.BASE),
         axios.get(API_ENDPOINTS.AUTH.USERS),
         axios.get(API_ENDPOINTS.ROUTES.BASE)
       ]);
        setStats({
          drivers: driversRes.data.length,
          vehicles: vehiclesRes.data.vehicles?.length || 0,
          users: usersRes.data.length,
          routes: routesRes.data.routes?.length || routesRes.data.Routes?.length || 0
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
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
       <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
          <Toolbar>
            <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1, fontWeight: 700 }}>
              Fuel System
            </Typography>
            <ThemeToggle />
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
        <Box sx={{ overflow: 'auto', display: 'flex', flexDirection: 'column', height: '100%' }}>
          <Box sx={{ px: 2, py: 1 }}>
            <Typography variant="overline" color="text.secondary">Menú</Typography>
          </Box>
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
          <Box sx={{ mt: 'auto', px: 2, pb: 2 }}>
            <Paper variant="outlined" sx={{ p: 2, borderRadius: 2 }}>
              <Typography variant="body2" color="text.secondary">
                Sesión: {user.username}
              </Typography>
              <Chip label={user.role} size="small" sx={{ mt: 1 }} />
            </Paper>
          </Box>
        </Box>
      </Drawer>
       <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
         <Toolbar />
         <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
           <Box>
             <Typography variant="h4" sx={{ fontWeight: 700 }}>
               {user.role === 'Operador' ? 'Panel del Operador' : 'Dashboard'}
             </Typography>
             <Typography variant="body2" color="text.secondary">
               {user.role === 'Operador' ? 'Tus asignaciones y actividades' : 'Resumen general del sistema'}
             </Typography>
           </Box>
         </Box>
          {user.role === 'Operador' ? (
            operatorData.vehicle || operatorData.route || operatorData.reports.length > 0 ? (
              <>
                {/* Header del Panel del Operador */}
                <Box sx={{ mb: 4 }}>
                  <Typography variant="h4" sx={{ fontWeight: 700, mb: 1 }}>
                    <DashboardIcon sx={{ mr: 1, verticalAlign: 'middle' }} />
                    Panel del Operador
                  </Typography>
                  <Typography variant="body1" color="text.secondary">
                    Información de tus asignaciones y actividades actuales
                  </Typography>
                  <Divider sx={{ mt: 2 }} />
                </Box>

                 <Grid container spacing={4}>
                   {operatorData.vehicle && (
                     <Grid item xs={12} lg={6}>
                       <OperatorInfoCard
                         title="Vehículo Asignado"
                         value={operatorData.vehicle.placa}
                         subtitle={`${operatorData.vehicle.marca} ${operatorData.vehicle.modelo}`}
                         icon={<LocalShipping />}
                         status={operatorData.vehicle.estado}
                         statusColor={operatorData.vehicle.estado === 'Activo' ? 'success' : 'warning'}
                       />
                     </Grid>
                   )}
                   {operatorData.route && (
                     <Grid item xs={12} lg={6}>
                       <OperatorInfoCard
                         title="Ruta Asignada"
                         value={operatorData.route.nombre}
                         subtitle={`${operatorData.route.origen} → ${operatorData.route.destino}`}
                         icon={<Route />}
                         status={operatorData.route.estado}
                         statusColor={operatorData.route.estado === 'EnProgreso' ? 'warning' : operatorData.route.estado === 'Completada' ? 'success' : 'default'}
                       />
                     </Grid>
                   )}
                   <Grid item xs={12} lg={6}>
                     <OperatorInfoCard
                       title="Reportes de Combustible"
                       value={operatorData.reports.length}
                       subtitle="Reportes disponibles para gestión"
                       icon={<Assessment />}
                       status={operatorData.reports.length > 0 ? 'Disponible' : 'Sin reportes'}
                       statusColor={operatorData.reports.length > 0 ? 'info' : 'default'}
                     />
                   </Grid>

                   {/* Información adicional del operador */}
                   <Grid item xs={12} lg={6}>
                     <Card sx={{ borderRadius: 3, height: '100%' }}>
                       <CardContent sx={{ p: 3 }}>
                         <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                           <Person sx={{ mr: 1, verticalAlign: 'middle' }} />
                           Información Personal
                         </Typography>
                         <Stack spacing={2}>
                           <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                             <Typography variant="body2" color="text.secondary">Usuario:</Typography>
                             <Typography variant="body2" sx={{ fontWeight: 500 }}>{user.username}</Typography>
                           </Box>
                           <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                             <Typography variant="body2" color="text.secondary">Rol:</Typography>
                             <Chip label={user.role} size="small" color="primary" />
                           </Box>
                           <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                             <Typography variant="body2" color="text.secondary">Estado:</Typography>
                             <Chip label="Activo" size="small" color="success" />
                           </Box>
                         </Stack>
                       </CardContent>
                     </Card>
                   </Grid>
                 </Grid>

                {/* Panel de actividades recientes */}
                {operatorData.reports.length > 0 && (
                  <Paper sx={{ mt: 4, p: 3, borderRadius: 3 }}>
                    <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                      <Assignment sx={{ mr: 1, verticalAlign: 'middle' }} />
                      Actividad Reciente
                    </Typography>
                    <Grid container spacing={2}>
                      {operatorData.reports.slice(0, 3).map((report, index) => (
                        <Grid item xs={12} sm={4} key={index}>
                          <Card sx={{ borderRadius: 2 }}>
                            <CardContent sx={{ p: 2 }}>
                              <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                <Speed sx={{ mr: 1, color: 'primary.main', fontSize: 20 }} />
                                <Typography variant="body2" sx={{ fontWeight: 500 }}>
                                  Reporte #{report.id}
                                </Typography>
                              </Box>
                              <Typography variant="h6" color="primary" sx={{ fontWeight: 700 }}>
                                {report.actualLiters} L
                              </Typography>
                              <Typography variant="caption" color="text.secondary">
                                {new Date(report.createdAt || Date.now()).toLocaleDateString()}
                              </Typography>
                            </CardContent>
                          </Card>
                        </Grid>
                      ))}
                    </Grid>
                  </Paper>
                )}
              </>
            ) : (
              <Box sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                height: '60vh',
                textAlign: 'center'
              }}>
                <Avatar sx={{ bgcolor: 'grey.100', width: 80, height: 80, mb: 3 }}>
                  <Assignment sx={{ fontSize: 40, color: 'grey.400' }} />
                </Avatar>
                <Typography variant="h5" color="text.secondary" sx={{ mb: 2, fontWeight: 500 }}>
                  Sin Asignaciones Activas
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ mb: 3, maxWidth: 400 }}>
                  Actualmente no tienes vehículos, rutas o reportes asignados.
                  Contacta con tu supervisor para obtener nuevas asignaciones.
                </Typography>
                <Chip
                  label="Contactar Supervisor"
                  variant="outlined"
                  color="primary"
                  sx={{ cursor: 'pointer' }}
                  onClick={() => navigate('/users')}
                />
              </Box>
            )
          ) : (
            <>
              {/* Header del Dashboard */}
              <Box sx={{ mb: 3 }}>
                <Typography variant="h5" sx={{ fontWeight: 600, mb: 0.5 }}>
                  Dashboard Administrativo
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Resumen del sistema de gestión de combustible
                </Typography>
              </Box>

               {/* Métricas principales */}
               <Grid container spacing={3} sx={{ mb: 4 }}>
                 <Grid item xs={12} sm={6} lg={3}>
                   <MetricCard
                     title="Conductores"
                     value={stats.drivers}
                     subtitle="Activos en el sistema"
                     icon={<DriveEta />}
                     trend={5.2}
                   />
                 </Grid>
                 <Grid item xs={12} sm={6} lg={3}>
                   <MetricCard
                     title="Vehículos"
                     value={stats.vehicles}
                     subtitle="Registrados"
                     icon={<LocalShipping />}
                     trend={2.1}
                   />
                 </Grid>
                 <Grid item xs={12} sm={6} lg={3}>
                   <MetricCard
                     title="Usuarios"
                     value={stats.users}
                     subtitle="Total del sistema"
                     icon={<People />}
                     trend={8.7}
                   />
                 </Grid>
                 <Grid item xs={12} sm={6} lg={3}>
                   <MetricCard
                     title="Rutas"
                     value={stats.routes}
                     subtitle="Configuradas"
                     icon={<Route />}
                     trend={-1.3}
                   />
                 </Grid>
               </Grid>

               {/* Panel de estadísticas adicionales */}
               <Grid container spacing={4}>
                 <Grid item xs={12} lg={8}>
                   <Paper sx={{ p: 3, borderRadius: 3, height: '100%' }}>
                     <Typography variant="h6" sx={{ fontWeight: 600, mb: 4, display: 'flex', alignItems: 'center' }}>
                       <BarChart sx={{ mr: 1.5, verticalAlign: 'middle' }} />
                       Actividad Reciente del Sistema
                     </Typography>
                     <Grid container spacing={3}>
                       <Grid item xs={12} md={4}>
                         <Box sx={{
                           p: 3,
                           borderRadius: 2,
                           bgcolor: 'background.default',
                           color: 'text.primary',
                           border: (theme) => `1px solid ${theme.palette.divider}`,
                           height: '100%',
                           display: 'flex',
                           flexDirection: 'column',
                           justifyContent: 'center'
                         }}>
                           <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                             <Assessment sx={{ mr: 1.5, fontSize: 28, color: 'primary.main' }} />
                             <Typography variant="h6" sx={{ fontWeight: 600 }}>
                               Reportes
                             </Typography>
                           </Box>
                           <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                             Última actualización: hace 2 horas
                           </Typography>
                           <Chip label="Activo" color="success" size="small" sx={{ alignSelf: 'flex-start' }} />
                         </Box>
                       </Grid>

                       <Grid item xs={12} md={4}>
                         <Box sx={{
                           p: 3,
                           borderRadius: 2,
                           bgcolor: 'background.default',
                           color: 'text.primary',
                           border: (theme) => `1px solid ${theme.palette.divider}`,
                           height: '100%',
                           display: 'flex',
                           flexDirection: 'column',
                           justifyContent: 'center'
                         }}>
                           <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                            <Timeline sx={{ mr: 1.5, fontSize: 28, color: 'primary.main' }} />
                             <Typography variant="h6" sx={{ fontWeight: 600 }}>
                               Rutas
                             </Typography>
                           </Box>
                          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                             12 rutas en progreso
                           </Typography>
                          <Chip label="En progreso" color="warning" size="small" sx={{ alignSelf: 'flex-start' }} />
                         </Box>
                       </Grid>

                       <Grid item xs={12} md={4}>
                         <Box sx={{
                           p: 3,
                           borderRadius: 2,
                           bgcolor: 'background.default',
                           color: 'text.primary',
                           border: (theme) => `1px solid ${theme.palette.divider}`,
                           height: '100%',
                           display: 'flex',
                           flexDirection: 'column',
                           justifyContent: 'center'
                         }}>
                           <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                            <Engineering sx={{ mr: 1.5, fontSize: 28, color: 'primary.main' }} />
                             <Typography variant="h6" sx={{ fontWeight: 600 }}>
                               Mantenimiento
                             </Typography>
                           </Box>
                          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                             3 vehículos requieren atención
                           </Typography>
                          <Chip label="Pendiente" color="error" size="small" sx={{ alignSelf: 'flex-start' }} />
                         </Box>
                       </Grid>
                     </Grid>
                   </Paper>
                 </Grid>

                 <Grid item xs={12} lg={4}>
                   <Paper sx={{ p: 3, borderRadius: 3, height: '100%' }}>
                     <Typography variant="h6" sx={{ fontWeight: 600, mb: 4, display: 'flex', alignItems: 'center' }}>
                       <TrendingUp sx={{ mr: 1.5, verticalAlign: 'middle' }} />
                       Indicadores de Rendimiento
                     </Typography>

                     <Stack spacing={4}>
                       <Box>
                         <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                           <Typography variant="body2" sx={{ fontWeight: 500 }}>Eficiencia de Combustible</Typography>
                           <Typography variant="body2" sx={{ fontWeight: 700, color: 'primary.main' }}>87%</Typography>
                         </Box>
                         <LinearProgress variant="determinate" value={87} sx={{ height: 10, borderRadius: 5 }} />
                       </Box>

                       <Box>
                         <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                           <Typography variant="body2" sx={{ fontWeight: 500 }}>Cumplimiento de Rutas</Typography>
                           <Typography variant="body2" sx={{ fontWeight: 700, color: 'secondary.main' }}>92%</Typography>
                         </Box>
                         <LinearProgress variant="determinate" value={92} color="secondary" sx={{ height: 10, borderRadius: 5 }} />
                       </Box>

                       <Box>
                         <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                           <Typography variant="body2" sx={{ fontWeight: 500 }}>Disponibilidad de Vehículos</Typography>
                           <Typography variant="body2" sx={{ fontWeight: 700, color: 'success.main' }}>95%</Typography>
                         </Box>
                         <LinearProgress variant="determinate" value={95} color="success" sx={{ height: 10, borderRadius: 5 }} />
                       </Box>
                     </Stack>
                   </Paper>
                 </Grid>
               </Grid>
            </>
          )}
       </Box>
    </Box>
  );
};

export default Dashboard;