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
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  IconButton,
  TextField,
  Grid,
  Tabs,
  Tab,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  Select,
  MenuItem
} from '@mui/material';
import {
  People,
  DriveEta,
  LocalShipping,
  Route,
  Assessment,
  Logout,
  Add,
  Edit,
  Delete,
  Update,
   LocalGasStation
} from '@mui/icons-material';

const drawerWidth = 240;

const RoutesComponent = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('rutas');
  const [routes, setRoutes] = useState([]);
  const [drivers, setDrivers] = useState([]);
  const [vehicles, setVehicles] = useState([]);
  const [nombreFilter, setNombreFilter] = useState('');
  const [origenFilter, setOrigenFilter] = useState('');
  const [destinoFilter, setDestinoFilter] = useState('');
  const [estadoFilter, setEstadoFilter] = useState('');
  const [openAssignDialog, setOpenAssignDialog] = useState(false);
  const [selectedDriver, setSelectedDriver] = useState(null);
  const [selectedPlaca, setSelectedPlaca] = useState('');
  const [openCreateRouteDialog, setOpenCreateRouteDialog] = useState(false);
  const [openEditRouteDialog, setOpenEditRouteDialog] = useState(false);
  const [openUpdateStatusDialog, setOpenUpdateStatusDialog] = useState(false);
  const [editingRoute, setEditingRoute] = useState(null);
  const [routeForm, setRouteForm] = useState({
    nombre: '',
    origen: '',
    destino: '',
    vehiclePlaca: '',
    driverId: ''
  });
  const [statusForm, setStatusForm] = useState({
    estado: ''
  });
   const [openFuelDialog, setOpenFuelDialog] = useState(false);
   const [fuelPlan, setFuelPlan] = useState(null);
   const [actualLiters, setActualLiters] = useState('');
   const [assignedPlaca, setAssignedPlaca] = useState('');

  useEffect(() => {
    if (!loading && !user) {
      navigate('/login');
    } else if (user && user.role === 'Admin') {
      fetchRoutes();
      fetchDrivers();
      fetchVehicles();
    } else if (user && user.role === 'Operador') {
      fetchDriverAndRoute();
    } else if (user && user.role === 'Supervisor') {
      fetchRoutes();
    }
  }, [user, loading, navigate]);

  const fetchRoutes = async () => {
    try {
      const response = await axios.get('http://localhost:5010/api/routes');
      setRoutes(response.data.routes || response.data.Routes || []);
    } catch (error) {
      console.error('Error fetching routes:', error);
    }
  };

  const fetchDrivers = async () => {
    try {
      const response = await axios.get('http://localhost:5010/api/drivers');
      let drivers = [];
      if (Array.isArray(response.data)) {
        drivers = response.data;
      } else if (response.data.Drivers) {
        drivers = response.data.Drivers;
      } else if (response.data.drivers) {
        drivers = response.data.drivers;
      }
      setDrivers(drivers);
    } catch (error) {
      console.error('Error fetching drivers:', error);
    }
  };

  const fetchVehicles = async () => {
    try {
      const response = await axios.get('http://localhost:5010/api/vehicles');
      setVehicles(response.data.vehicles || []);
    } catch (error) {
      console.error('Error fetching vehicles:', error);
    }
  };
  const fetchDriverAndRoute = async () => {
     try {
       // Fetch driver by username
       const driverResponse = await axios.get('http://localhost:5010/api/drivers');
       let drivers = [];
       if (Array.isArray(driverResponse.data)) {
         drivers = driverResponse.data;
       } else if (driverResponse.data.Drivers) {
         drivers = driverResponse.data.Drivers;
       }
       const driver = drivers.find(d => d.documentNumber === user.username);
       if (driver && driver.assignedVehiclePlaca) {
         setAssignedPlaca(driver.assignedVehiclePlaca);
         // Fetch routes and find the one assigned to the vehicle
         const routeResponse = await axios.get('http://localhost:5010/api/routes');
         const allRoutes = routeResponse.data.routes || routeResponse.data.Routes || [];
         const assignedRoute = allRoutes.find(r => r.vehiclePlaca === driver.assignedVehiclePlaca);
         setRoutes(assignedRoute ? [assignedRoute] : []);
       } else {
         setAssignedPlaca('');
         setRoutes([]);
       }
     } catch (error) {
       console.error('Error fetching driver and route:', error);
       setAssignedPlaca('');
       setRoutes([]);
     }
   };


  const handleAssign = (driver) => {
    setSelectedDriver(driver);
    setSelectedPlaca('');
    setOpenAssignDialog(true);
  };



  const handleUnassign = async (driver) => {
    try {
      await axios.post(`http://localhost:5010/api/drivers/${driver.id}/unassign`);
      fetchDrivers();
      fetchVehicles();
    } catch (error) {
      console.error('Error unassigning vehicle:', error);
    }
  };

  const handleAssignSubmit = async () => {
    try {
      await axios.post(`http://localhost:5010/api/drivers/${selectedDriver.id}/assign`, {
        VehiclePlaca: selectedPlaca
      });
      setOpenAssignDialog(false);
      fetchDrivers();
      fetchVehicles();
    } catch (error) {
      console.error('Error assigning vehicle:', error);
    }
  };

  const handleCreateRoute = () => {
    setRouteForm({
      nombre: '',
      origen: '',
      destino: '',
      vehiclePlaca: '',
      driverId: ''
    });
    setOpenCreateRouteDialog(true);
  };

  const handleEditRoute = (route) => {
    setEditingRoute(route);
    setRouteForm({
      nombre: route.nombre,
      origen: route.origen,
      destino: route.destino,
      vehiclePlaca: route.vehiclePlaca,
      driverId: route.driverId
    });
    setOpenEditRouteDialog(true);
  };

  const handleUpdateStatus = (route) => {
    setEditingRoute(route);
    setStatusForm({
      estado: route.estado
    });
    setOpenUpdateStatusDialog(true);
  };

  const handleSubmitRoute = async () => {
    try {
      if (editingRoute) {
        await axios.put(`http://localhost:5010/api/routes/${editingRoute.id}`, routeForm);
      } else {
        await axios.post('http://localhost:5010/api/routes', routeForm);
      }
      setOpenCreateRouteDialog(false);
      setOpenEditRouteDialog(false);
      fetchRoutes();
    } catch (error) {
      console.error('Error saving route:', error);
    }
  };

  const handleSubmitStatus = async () => {
     try {
       await axios.post(`http://localhost:5010/api/routes/${editingRoute.id}/status`, { Estado: statusForm.estado });
       setOpenUpdateStatusDialog(false);
       if (user.role === 'Operador') {
         fetchDriverAndRoute();
       } else {
         fetchRoutes();
       }
     } catch (error) {
       console.error('Error updating status:', error);
     }
   };

  const handleAddFuel = async (route) => {
    try {
      const response = await axios.post('http://localhost:5010/api/fuel/plan', {
        vehiclePlaca: "",
        driverId: 0,
        routeId: route.id
      });
      setFuelPlan(response.data);
      setActualLiters('');
      setOpenFuelDialog(true);
    } catch (error) {
      console.error('Error creating fuel plan:', error);
    }
  };

  const handleSubmitFuel = async () => {
    try {
      await axios.post('http://localhost:5010/api/fuel/register', {
        planId: fuelPlan.id,
        actualLiters: parseFloat(actualLiters)
      });
      setOpenFuelDialog(false);
      setFuelPlan(null);
      setActualLiters('');
    } catch (error) {
      console.error('Error registering fuel:', error);
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
            Fuel System - {user.role === 'Operador' ? 'Mi Ruta' : 'Rutas'}
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
        {user.role === 'Admin' ? (
          <>
            <Tabs value={activeTab} onChange={(e, newValue) => setActiveTab(newValue)} sx={{ mb: 2 }}>
              <Tab label="Asignar" value="asignar" />
              <Tab label="Desasignar" value="desasignar" />
              <Tab label="Rutas" value="rutas" />
            </Tabs>
            {activeTab === 'rutas' && (
              <>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                  <Typography variant="h4">Rutas</Typography>
                  <Button variant="contained" startIcon={<Add />} onClick={handleCreateRoute}>
                    Nueva Ruta
                  </Button>
                </Box>
                <Grid container spacing={2} sx={{ mb: 2 }}>
                  <Grid item xs={12} sm={3}>
                    <TextField
                      fullWidth
                      label="Filtrar por Nombre"
                      value={nombreFilter}
                      onChange={(e) => setNombreFilter(e.target.value)}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <TextField
                      fullWidth
                      label="Filtrar por Origen"
                      value={origenFilter}
                      onChange={(e) => setOrigenFilter(e.target.value)}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <TextField
                      fullWidth
                      label="Filtrar por Destino"
                      value={destinoFilter}
                      onChange={(e) => setDestinoFilter(e.target.value)}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <TextField
                      fullWidth
                      label="Filtrar por Estado"
                      value={estadoFilter}
                      onChange={(e) => setEstadoFilter(e.target.value)}
                    />
                  </Grid>
                </Grid>
                <TableContainer component={Paper}>
                  <Table>
                    <TableHead>
                      <TableRow>
                        <TableCell>ID</TableCell>
                        <TableCell>Nombre</TableCell>
                        <TableCell>Origen</TableCell>
                        <TableCell>Destino</TableCell>
                        <TableCell>Estado</TableCell>
                        <TableCell>Acciones</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                   {routes.filter(route =>
                     (!nombreFilter || route.nombre.toLowerCase().includes(nombreFilter.toLowerCase())) &&
                     (!origenFilter || route.origen.toLowerCase().includes(origenFilter.toLowerCase())) &&
                     (!destinoFilter || route.destino.toLowerCase().includes(destinoFilter.toLowerCase())) &&
                     (!estadoFilter || route.estado.toLowerCase().includes(estadoFilter.toLowerCase())) &&
                     (user.role !== 'Operador' || route.vehiclePlaca === assignedPlaca)
                   ).map((route) => (
                        <TableRow key={route.id}>
                          <TableCell>{route.id}</TableCell>
                          <TableCell>{route.nombre}</TableCell>
                          <TableCell>{route.origen}</TableCell>
                          <TableCell>{route.destino}</TableCell>
                          <TableCell>{route.estado}</TableCell>
                          <TableCell>
                            <IconButton onClick={() => handleEditRoute(route)}>
                              <Edit />
                            </IconButton>
                            <IconButton onClick={() => handleUpdateStatus(route)}>
                              <Update />
                            </IconButton>
                            <IconButton onClick={() => handleAddFuel(route)}>
                              <LocalGasStation />
                            </IconButton>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </>
            )}
            {activeTab === 'asignar' && (
              <>
                <Typography variant="h4" sx={{ mb: 2 }}>Asignar Vehículos a Choferes</Typography>
                <TableContainer component={Paper}>
                  <Table>
                    <TableHead>
                      <TableRow>
                        <TableCell>ID</TableCell>
                        <TableCell>Nombre</TableCell>
                        <TableCell>Documento</TableCell>
                        <TableCell>Vehículo Asignado</TableCell>
                        <TableCell>Acciones</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {drivers.filter(driver => !driver.isDeleted && !driver.isAssigned).map((driver) => (
                        <TableRow key={driver.id}>
                          <TableCell>{driver.id}</TableCell>
                          <TableCell>{driver.firstName} {driver.lastName}</TableCell>
                          <TableCell>{driver.documentNumber}</TableCell>
                          <TableCell>Ninguno</TableCell>
                          <TableCell>
                            <Button variant="contained" onClick={() => handleAssign(driver)}>
                              Asignar
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </>
            )}
            {activeTab === 'desasignar' && (
              <>
                <Typography variant="h4" sx={{ mb: 2 }}>Desasignar Vehículos de Choferes</Typography>
                <TableContainer component={Paper}>
                  <Table>
                    <TableHead>
                      <TableRow>
                        <TableCell>ID</TableCell>
                        <TableCell>Nombre</TableCell>
                        <TableCell>Documento</TableCell>
                        <TableCell>Vehículo Asignado</TableCell>
                        <TableCell>Acciones</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {drivers.filter(driver => !driver.isDeleted && driver.isAssigned).map((driver) => (
                        <TableRow key={driver.id}>
                          <TableCell>{driver.id}</TableCell>
                          <TableCell>{driver.firstName} {driver.lastName}</TableCell>
                          <TableCell>{driver.documentNumber}</TableCell>
                          <TableCell>{driver.assignedVehiclePlaca}</TableCell>
                          <TableCell>
                            <Button variant="outlined" color="secondary" onClick={() => handleUnassign(driver)}>
                              Desasignar
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </>
            )}
          </>
        ) : (
          <>
            <Typography variant="h4" sx={{ mb: 2 }}>{user.role === 'Operador' ? 'Mi Ruta' : 'Rutas'}</Typography>
            <Grid container spacing={2} sx={{ mb: 2 }}>
              <Grid item xs={12} sm={3}>
                <TextField
                  fullWidth
                  label="Filtrar por Nombre"
                  value={nombreFilter}
                  onChange={(e) => setNombreFilter(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <TextField
                  fullWidth
                  label="Filtrar por Origen"
                  value={origenFilter}
                  onChange={(e) => setOrigenFilter(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <TextField
                  fullWidth
                  label="Filtrar por Destino"
                  value={destinoFilter}
                  onChange={(e) => setDestinoFilter(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <TextField
                  fullWidth
                  label="Filtrar por Estado"
                  value={estadoFilter}
                  onChange={(e) => setEstadoFilter(e.target.value)}
                />
              </Grid>
            </Grid>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>ID</TableCell>
                    <TableCell>Nombre</TableCell>
                    <TableCell>Origen</TableCell>
                    <TableCell>Destino</TableCell>
                    <TableCell>Estado</TableCell>
                     {(user.role === 'Admin' || user.role === 'Operador') && <TableCell>Acciones</TableCell>}
                  </TableRow>
                </TableHead>
                <TableBody>
                  {routes.filter(route =>
                    (!nombreFilter || route.nombre.toLowerCase().includes(nombreFilter.toLowerCase())) &&
                    (!origenFilter || route.origen.toLowerCase().includes(origenFilter.toLowerCase())) &&
                    (!destinoFilter || route.destino.toLowerCase().includes(destinoFilter.toLowerCase())) &&
                    (!estadoFilter || route.estado.toLowerCase().includes(estadoFilter.toLowerCase()))
                  ).map((route) => (
                    <TableRow key={route.id}>
                      <TableCell>{route.id}</TableCell>
                      <TableCell>{route.nombre}</TableCell>
                      <TableCell>{route.origen}</TableCell>
                      <TableCell>{route.destino}</TableCell>
                      <TableCell>{route.estado}</TableCell>
                       {(user.role === 'Admin' || user.role === 'Operador') && (
                         <TableCell>
                           {user.role === 'Admin' && (
                             <>
                               <IconButton onClick={() => handleEditRoute(route)}>
                                 <Edit />
                               </IconButton>
                               <IconButton onClick={() => handleAddFuel(route)}>
                                 <LocalGasStation />
                               </IconButton>
                             </>
                           )}
                           <IconButton onClick={() => handleUpdateStatus(route)}>
                             <Update />
                           </IconButton>
                         </TableCell>
                       )}
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}
      </Box>

      <Dialog open={openAssignDialog} onClose={() => setOpenAssignDialog(false)}>
        <DialogTitle>Asignar Vehículo</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel>Seleccionar Placa</InputLabel>
            <Select
              value={selectedPlaca}
              onChange={(e) => setSelectedPlaca(e.target.value)}
              label="Seleccionar Placa"
            >
              {vehicles.filter(vehicle => !vehicle.assignedDriverDocument).map((vehicle) => (
                <MenuItem key={vehicle.placa} value={vehicle.placa}>
                  {vehicle.placa} - {vehicle.marca} {vehicle.modelo}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAssignDialog(false)}>Cancelar</Button>
          <Button onClick={handleAssignSubmit} disabled={!selectedPlaca}>Asignar</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openCreateRouteDialog} onClose={() => setOpenCreateRouteDialog(false)}>
        <DialogTitle>Crear Nueva Ruta</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Nombre"
            fullWidth
            value={routeForm.nombre}
            onChange={(e) => setRouteForm({ ...routeForm, nombre: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Origen"
            fullWidth
            value={routeForm.origen}
            onChange={(e) => setRouteForm({ ...routeForm, origen: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Destino"
            fullWidth
            value={routeForm.destino}
            onChange={(e) => setRouteForm({ ...routeForm, destino: e.target.value })}
          />
          <FormControl fullWidth margin="dense">
            <InputLabel>Driver Document (Asignado)</InputLabel>
            <Select
              value={routeForm.driverId}
              onChange={(e) => {
                const selectedDriver = drivers.find(d => d.id === e.target.value);
                setRouteForm({
                  ...routeForm,
                  driverId: e.target.value,
                  vehiclePlaca: selectedDriver ? selectedDriver.assignedVehiclePlaca : ''
                });
              }}
              label="Driver Document (Asignado)"
            >
              {drivers.filter(driver => !driver.isDeleted && driver.isAssigned).map((driver) => (
                <MenuItem key={driver.id} value={driver.id}>
                  {driver.documentNumber} - {driver.firstName} {driver.lastName}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenCreateRouteDialog(false)}>Cancelar</Button>
          <Button onClick={handleSubmitRoute}>Crear</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openEditRouteDialog} onClose={() => setOpenEditRouteDialog(false)}>
        <DialogTitle>Editar Ruta</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Nombre"
            fullWidth
            value={routeForm.nombre}
            onChange={(e) => setRouteForm({ ...routeForm, nombre: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Origen"
            fullWidth
            value={routeForm.origen}
            onChange={(e) => setRouteForm({ ...routeForm, origen: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Destino"
            fullWidth
            value={routeForm.destino}
            onChange={(e) => setRouteForm({ ...routeForm, destino: e.target.value })}
          />
          <FormControl fullWidth margin="dense">
            <InputLabel>Driver Document (Asignado)</InputLabel>
            <Select
              value={routeForm.driverId}
              onChange={(e) => {
                const selectedDriver = drivers.find(d => d.id === e.target.value);
                setRouteForm({
                  ...routeForm,
                  driverId: e.target.value,
                  vehiclePlaca: selectedDriver ? selectedDriver.assignedVehiclePlaca : ''
                });
              }}
              label="Driver Document (Asignado)"
            >
              {drivers.filter(driver => !driver.isDeleted && driver.isAssigned).map((driver) => (
                <MenuItem key={driver.id} value={driver.id}>
                  {driver.documentNumber} - {driver.firstName} {driver.lastName}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditRouteDialog(false)}>Cancelar</Button>
          <Button onClick={handleSubmitRoute}>Actualizar</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openUpdateStatusDialog} onClose={() => setOpenUpdateStatusDialog(false)}>
        <DialogTitle>Actualizar Estado de Ruta</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel>Estado</InputLabel>
            <Select
              value={statusForm.estado}
              onChange={(e) => setStatusForm({ ...statusForm, estado: e.target.value })}
              label="Estado"
            >
              <MenuItem value="Planificada">Planificada</MenuItem>
              <MenuItem value="EnCurso">EnCurso</MenuItem>
              <MenuItem value="Completada">Completada</MenuItem>
              <MenuItem value="Cancelada">Cancelada</MenuItem>
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenUpdateStatusDialog(false)}>Cancelar</Button>
          <Button onClick={handleSubmitStatus}>Actualizar</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openFuelDialog} onClose={() => setOpenFuelDialog(false)}>
        <DialogTitle>Registrar Combustible</DialogTitle>
        <DialogContent>
          {fuelPlan && (
            <>
              <Typography variant="body1" sx={{ mb: 2 }}>
                Litros Estimados: {fuelPlan.estimatedLiters}
              </Typography>
              <TextField
                autoFocus
                margin="dense"
                label="Litros Actuales"
                type="number"
                fullWidth
                value={actualLiters}
                onChange={(e) => setActualLiters(e.target.value)}
              />
            </>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenFuelDialog(false)}>Cancelar</Button>
          <Button onClick={handleSubmitFuel} disabled={!actualLiters}>Registrar</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default RoutesComponent;