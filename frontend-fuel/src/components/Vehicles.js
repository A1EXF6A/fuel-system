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
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Alert,
  Select,
  MenuItem,
  FormControl,
  InputLabel
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
  Delete
} from '@mui/icons-material';

const drawerWidth = 240;

const Vehicles = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
  const [vehicles, setVehicles] = useState([]);
  const [placaFilter, setPlacaFilter] = useState('');
  const [marcaFilter, setMarcaFilter] = useState('');
  const [modeloFilter, setModeloFilter] = useState('');
  const [estadoFilter, setEstadoFilter] = useState('');
  const [openCreate, setOpenCreate] = useState(false);
  const [openEdit, setOpenEdit] = useState(false);
  const [openDelete, setOpenDelete] = useState(false);
  const [editingVehicle, setEditingVehicle] = useState(null);
  const [deletingVehicle, setDeletingVehicle] = useState(null);
  const [formData, setFormData] = useState({
    placa: '',
    chasis: '',
    marca: '',
    modelo: '',
    anio: '',
    vehicleTypeId: '',
    estado: '',
    km: '',
    assignedDriverDocument: ''
  });
  const [alert, setAlert] = useState(null);

  useEffect(() => {
    if (!loading && !user) {
      navigate('/login');
    } else if (user && user.role === 'Operador') {
      fetchDriverAndVehicles();
    } else if (user && (user.role === 'Admin' || user.role === 'Supervisor')) {
      fetchVehicles();
    }
  }, [user, loading, navigate]);

  const fetchVehicles = async () => {
    try {
      const response = await axios.get('http://localhost:5010/api/vehicles');
      setVehicles(response.data.vehicles || []);
    } catch (error) {
      console.error('Error fetching vehicles:', error);
    }
  };

  const fetchDriverAndVehicles = async () => {
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
        // Fetch the specific vehicle
        const vehicleResponse = await axios.get('http://localhost:5010/api/vehicles');
        const allVehicles = vehicleResponse.data.vehicles || [];
        const assignedVehicle = allVehicles.find(v => v.placa === driver.assignedVehiclePlaca);
        setVehicles(assignedVehicle ? [assignedVehicle] : []);
      } else {
        setVehicles([]);
      }
    } catch (error) {
      console.error('Error fetching driver and vehicles:', error);
      setVehicles([]);
    }
  };

  const handleCreate = () => {
    setFormData({
      placa: '',
      chasis: '',
      marca: '',
      modelo: '',
      anio: '',
      vehicleTypeId: '',
      estado: '',
      km: '',
      assignedDriverDocument: ''
    });
    setOpenCreate(true);
  };

  const handleEdit = (vehicle) => {
    setEditingVehicle(vehicle);
    setFormData({
      id: vehicle.id,
      placa: vehicle.placa || '',
      chasis: vehicle.chasis || '',
      marca: vehicle.marca || '',
      modelo: vehicle.modelo || '',
      anio: vehicle.anio ? vehicle.anio.toString() : '',
      vehicleTypeId: vehicle.vehicleTypeId ? parseInt(vehicle.vehicleTypeId) : 1,
      estado: vehicle.estado || '',
      km: vehicle.km ? vehicle.km.toString() : '',
      assignedDriverDocument: vehicle.assignedDriverDocument || ''
    });
    setOpenEdit(true);
  };

  const handleDelete = (vehicle) => {
    setDeletingVehicle(vehicle);
    setOpenDelete(true);
  };

  const handleSubmitCreate = async () => {
    try {
      await axios.post('http://localhost:5010/api/vehicles', formData);
      setAlert({ type: 'success', message: 'Vehículo creado exitosamente' });
      setOpenCreate(false);
      fetchVehicles();
    } catch (error) {
      setAlert({ type: 'error', message: 'Error al crear vehículo' });
    }
  };

  const handleSubmitEdit = async () => {
    try {
      await axios.put(`http://localhost:5010/api/vehicles/${editingVehicle.id}`, formData);
      setAlert({ type: 'success', message: 'Vehículo actualizado exitosamente' });
      setOpenEdit(false);
      fetchVehicles();
    } catch (error) {
      setAlert({ type: 'error', message: 'Error al actualizar vehículo' });
    }
  };

  const handleConfirmDelete = async () => {
    try {
      await axios.delete(`http://localhost:5010/api/vehicles/${deletingVehicle.id}`);
      setAlert({ type: 'success', message: 'Vehículo eliminado exitosamente' });
      setOpenDelete(false);
      fetchVehicles();
    } catch (error) {
      setAlert({ type: 'error', message: 'Error al eliminar vehículo' });
    }
  };

  const handleCloseAlert = () => {
    setAlert(null);
  };

  if (loading) return <div>Loading...</div>;

  if (!user) return null;

  if (user.role !== 'Admin') {
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
            Fuel System - Vehículos
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
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Typography variant="h4">Vehículos</Typography>
          {user && user.role !== 'Operador' && (
            <Button variant="contained" startIcon={<Add />} onClick={handleCreate}>
              Nuevo Vehículo
            </Button>
          )}
        </Box>
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              label="Filtrar por Placa"
              value={placaFilter}
              onChange={(e) => setPlacaFilter(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              label="Filtrar por Marca"
              value={marcaFilter}
              onChange={(e) => setMarcaFilter(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              label="Filtrar por Modelo"
              value={modeloFilter}
              onChange={(e) => setModeloFilter(e.target.value)}
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
                <TableCell>Placa</TableCell>
                <TableCell>Marca</TableCell>
                <TableCell>Modelo</TableCell>
                <TableCell>Estado</TableCell>
                <TableCell>Acciones</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {vehicles.filter(vehicle =>
                (!placaFilter || vehicle.placa.toLowerCase().includes(placaFilter.toLowerCase())) &&
                (!marcaFilter || vehicle.marca.toLowerCase().includes(marcaFilter.toLowerCase())) &&
                (!modeloFilter || vehicle.modelo.toLowerCase().includes(modeloFilter.toLowerCase())) &&
                (!estadoFilter || vehicle.estado.toLowerCase().includes(estadoFilter.toLowerCase()))
              ).map((vehicle) => (
                <TableRow key={vehicle.id}>
                  <TableCell>{vehicle.id}</TableCell>
                  <TableCell>{vehicle.placa}</TableCell>
                  <TableCell>{vehicle.marca}</TableCell>
                  <TableCell>{vehicle.modelo}</TableCell>
                  <TableCell>{vehicle.estado}</TableCell>
                  <TableCell>
                    <IconButton onClick={() => handleEdit(vehicle)}>
                      <Edit />
                    </IconButton>
                    <IconButton onClick={() => handleDelete(vehicle)}>
                      <Delete />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>

      {/* Alert */}
      {alert && (
        <Alert severity={alert.type} onClose={handleCloseAlert} sx={{ mt: 2 }}>
          {alert.message}
        </Alert>
      )}

      {/* Create Dialog */}
      <Dialog open={openCreate} onClose={() => setOpenCreate(false)}>
        <DialogTitle>Crear Nuevo Vehículo</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Placa"
            fullWidth
            value={formData.placa}
            onChange={(e) => setFormData({ ...formData, placa: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Chasis"
            fullWidth
            value={formData.chasis}
            onChange={(e) => setFormData({ ...formData, chasis: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Marca"
            fullWidth
            value={formData.marca}
            onChange={(e) => setFormData({ ...formData, marca: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Modelo"
            fullWidth
            value={formData.modelo}
            onChange={(e) => setFormData({ ...formData, modelo: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Año"
            fullWidth
            type="number"
            value={formData.anio}
            onChange={(e) => setFormData({ ...formData, anio: e.target.value })}
          />
          <FormControl fullWidth margin="dense">
            <InputLabel>Tipo de Vehículo</InputLabel>
            <Select
              value={formData.vehicleTypeId}
              onChange={(e) => setFormData({ ...formData, vehicleTypeId: e.target.value })}
              label="Tipo de Vehículo"
            >
              <MenuItem value={1}>Liviano</MenuItem>
              <MenuItem value={2}>Pesado</MenuItem>
            </Select>
          </FormControl>
          <TextField
            margin="dense"
            label="Estado"
            fullWidth
            value={formData.estado}
            onChange={(e) => setFormData({ ...formData, estado: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Kilómetros"
            fullWidth
            type="number"
            value={formData.km}
            onChange={(e) => setFormData({ ...formData, km: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenCreate(false)}>Cancelar</Button>
          <Button onClick={handleSubmitCreate}>Crear</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Dialog */}
      <Dialog open={openEdit} onClose={() => setOpenEdit(false)}>
        <DialogTitle>Editar Vehículo</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Placa"
            fullWidth
            value={formData.placa}
            onChange={(e) => setFormData({ ...formData, placa: e.target.value })}
            disabled
          />
          <TextField
            margin="dense"
            label="Chasis"
            fullWidth
            value={formData.chasis}
            onChange={(e) => setFormData({ ...formData, chasis: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Marca"
            fullWidth
            value={formData.marca}
            onChange={(e) => setFormData({ ...formData, marca: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Modelo"
            fullWidth
            value={formData.modelo}
            onChange={(e) => setFormData({ ...formData, modelo: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Año"
            fullWidth
            type="number"
            value={formData.anio}
            onChange={(e) => setFormData({ ...formData, anio: e.target.value })}
          />
          <FormControl fullWidth margin="dense">
            <InputLabel>Tipo de Vehículo</InputLabel>
            <Select
              value={formData.vehicleTypeId}
              onChange={(e) => setFormData({ ...formData, vehicleTypeId: e.target.value })}
              label="Tipo de Vehículo"
            >
              <MenuItem value={1}>Liviano</MenuItem>
              <MenuItem value={2}>Pesado</MenuItem>
            </Select>
          </FormControl>
          <TextField
            margin="dense"
            label="Estado"
            fullWidth
            value={formData.estado}
            onChange={(e) => setFormData({ ...formData, estado: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Kilómetros"
            fullWidth
            type="number"
            value={formData.km}
            onChange={(e) => setFormData({ ...formData, km: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEdit(false)}>Cancelar</Button>
          <Button onClick={handleSubmitEdit}>Actualizar</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Dialog */}
      <Dialog open={openDelete} onClose={() => setOpenDelete(false)}>
        <DialogTitle>Eliminar Vehículo</DialogTitle>
        <DialogContent>
          <Typography>¿Estás seguro de que quieres eliminar el vehículo con placa {deletingVehicle?.placa}?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDelete(false)}>Cancelar</Button>
          <Button onClick={handleConfirmDelete} color="error">Eliminar</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Vehicles;