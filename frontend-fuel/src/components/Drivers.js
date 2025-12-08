import React, { useContext, useEffect, useState } from 'react';
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
  Dashboard as DashboardIcon
} from '@mui/icons-material';
import ThemeToggle from './ThemeToggle';

const drawerWidth = 240;

const Drivers = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
  const [drivers, setDrivers] = useState([]);
  const [documentFilter, setDocumentFilter] = useState('');
  const [nameFilter, setNameFilter] = useState('');
  const [vehicleFilter, setVehicleFilter] = useState('');

  const [openEdit, setOpenEdit] = useState(false);
  const [openDelete, setOpenDelete] = useState(false);
  const [editingDriver, setEditingDriver] = useState(null);
  const [deletingDriver, setDeletingDriver] = useState(null);
  const [deleteForm, setDeleteForm] = useState({
    deletedBy: '',
    reason: ''
  });
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    documentNumber: '',
    phoneNumber: '',
    email: '',
    licenseNumber: '',
    licenseCategory: '',
    licenseExpiryDate: '',
    driverType: '',
    hireDate: '',
    status: ''
  });
  const [alert, setAlert] = useState(null);

  useEffect(() => {
    if (!loading && !user) {
      navigate('/login');
    } else if (user && user.role !== 'Admin' && user.role !== 'Supervisor') {
      return;
    } else if (user && (user.role === 'Admin' || user.role === 'Supervisor')) {
      fetchDrivers();
    }
  }, [user, loading, navigate]);

  const fetchDrivers = async () => {
    try {
      const response = await axios.get(API_ENDPOINTS.DRIVERS.BASE);
      let drivers = [];
      if (Array.isArray(response.data)) {
        drivers = response.data;
      } else if (response.data.Drivers) {
        drivers = response.data.Drivers;
      } else if (response.data.drivers) {
        drivers = response.data.drivers;
      } else if (typeof response.data === 'object' && response.data.id) {
        drivers = [response.data];
      }
      setDrivers(drivers);
    } catch (error) {
      console.error('Error fetching Drivers:', error);
    }
  };



  const handleEdit = (driver) => {
    setEditingDriver(driver);
    setFormData({
      id: driver.id,
      firstName: driver.firstName || '',
      lastName: driver.lastName || '',
      documentNumber: driver.documentNumber || '',
      phoneNumber: driver.phoneNumber || '',
      email: driver.email || '',
      licenseNumber: driver.licenseNumber || '',
      licenseCategory: driver.licenseCategory ? parseInt(driver.licenseCategory) : 2,
      licenseExpiryDate: driver.licenseExpiryDate ? new Date(driver.licenseExpiryDate).toISOString().slice(0, 16) : '',
      driverType: driver.driverType ? parseInt(driver.driverType) : 1,
      hireDate: driver.hireDate ? new Date(driver.hireDate).toISOString().slice(0, 16) : '',
      status: driver.status ? parseInt(driver.status) : 1
    });
    setOpenEdit(true);
  };

  const handleDelete = (driver) => {
    setDeletingDriver(driver);
    setDeleteForm({
      deletedBy: 'admin@example.com',
      reason: 'No longer with company'
    });
    setOpenDelete(true);
  };



  const handleSubmitEdit = async () => {
    try {
      await axios.put(`${API_ENDPOINTS.DRIVERS.BASE}/${editingDriver.id}`, formData);
      setAlert({ type: 'success', message: 'Chofer actualizado exitosamente' });
      setOpenEdit(false);
      fetchDrivers();
    } catch (error) {
      setAlert({ type: 'error', message: 'Error al actualizar chofer' });
    }
  };

  const handleConfirmDelete = async () => {
    try {
      await axios.delete(`${API_ENDPOINTS.DRIVERS.BASE}/${deletingDriver.id}`, {
        data: deleteForm
      });
      setDrivers(drivers.filter(d => d.id !== deletingDriver.id));
      setAlert({ type: 'success', message: 'Chofer eliminado exitosamente' });
      setOpenDelete(false);
    } catch (error) {
      setAlert({ type: 'error', message: 'Error al eliminar chofer' });
    }
  };

  const handleCloseAlert = () => {
    setAlert(null);
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
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
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
             FuelSense
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
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h5" sx={{ fontWeight: 600 }}>Choferes</Typography>
            <Typography variant="body2" color="text.secondary">Administración y seguimiento de choferes</Typography>
          </Box>
        </Box>
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={12} sm={4}>
            <Paper variant="outlined" sx={{ p: 2 }}>
              <TextField
                fullWidth
                label="Filtrar por Documento"
                value={documentFilter}
                onChange={(e) => setDocumentFilter(e.target.value)}
              />
            </Paper>
          </Grid>
          <Grid item xs={12} sm={4}>
            <Paper variant="outlined" sx={{ p: 2 }}>
              <TextField
                fullWidth
                label="Filtrar por Nombre"
                value={nameFilter}
                onChange={(e) => setNameFilter(e.target.value)}
              />
            </Paper>
          </Grid>

          <Grid item xs={12} sm={4}>
            <Paper variant="outlined" sx={{ p: 2 }}>
              <TextField
                fullWidth
                label="Filtrar por Vehículo"
                value={vehicleFilter}
                onChange={(e) => setVehicleFilter(e.target.value)}
              />
            </Paper>
          </Grid>
        </Grid>
        <TableContainer component={Paper} sx={{ borderRadius: 3 }}>
          <Table>
             <TableHead>
               <TableRow>
                 <TableCell>ID</TableCell>
                 <TableCell>Nombre</TableCell>
                 <TableCell>Documento</TableCell>
                 <TableCell>Estado</TableCell>
                 <TableCell>Asignado</TableCell>
                 {user.role === 'Admin' && <TableCell>Acciones</TableCell>}
               </TableRow>
             </TableHead>
            <TableBody>
              {drivers.filter(driver =>
                !driver.isDeleted &&
                (!documentFilter || driver.documentNumber.toLowerCase().includes(documentFilter.toLowerCase())) &&
                (!nameFilter || `${driver.firstName} ${driver.lastName}`.toLowerCase().includes(nameFilter.toLowerCase())) &&
                (!vehicleFilter || driver.assignedVehiclePlaca?.toLowerCase().includes(vehicleFilter.toLowerCase()))
              ).map((driver) => (
                <TableRow key={driver.id}>
                  <TableCell>{driver.id}</TableCell>
                  <TableCell>{driver.firstName} {driver.lastName}</TableCell>
                  <TableCell>{driver.documentNumber}</TableCell>
                  <TableCell>{driver.status}</TableCell>
                  <TableCell>{driver.isAssigned ? 'Sí' : 'No'}</TableCell>
                   <TableCell>
                     {user.role === 'Admin' && (
                       <>
                         <IconButton onClick={() => handleEdit(driver)}>
                           <Edit />
                         </IconButton>
                         <IconButton onClick={() => handleDelete(driver)}>
                           <Delete />
                         </IconButton>
                       </>
                     )}
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



      {/* Edit Dialog */}
      <Dialog open={openEdit} onClose={() => setOpenEdit(false)} fullWidth maxWidth="sm">
        <DialogTitle sx={{ fontWeight: 600 }}>Editar Chofer</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Nombre"
            fullWidth
            value={formData.firstName}
            onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Apellido"
            fullWidth
            value={formData.lastName}
            onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Número de Documento"
            fullWidth
            value={formData.documentNumber}
            onChange={(e) => setFormData({ ...formData, documentNumber: e.target.value })}
            disabled
          />
          <TextField
            margin="dense"
            label="Teléfono"
            fullWidth
            value={formData.phoneNumber}
            onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Email"
            fullWidth
            type="email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Número de Licencia"
            fullWidth
            value={formData.licenseNumber}
            onChange={(e) => setFormData({ ...formData, licenseNumber: e.target.value })}
          />
          <FormControl fullWidth margin="dense">
            <InputLabel>Categoría de Licencia</InputLabel>
            <Select
              value={formData.licenseCategory}
              onChange={(e) => setFormData({ ...formData, licenseCategory: e.target.value })}
              label="Categoría de Licencia"
            >
              <MenuItem value={1}>Motocicleta</MenuItem>
              <MenuItem value={2}>Automóvil</MenuItem>
              <MenuItem value={3}>Camión</MenuItem>
              <MenuItem value={4}>Transporte de pasajeros</MenuItem>
              <MenuItem value={5}>Maquinaria pesada especial</MenuItem>
            </Select>
          </FormControl>
          <TextField
            margin="dense"
            label="Fecha de Expiración de Licencia"
            fullWidth
            type="datetime-local"
            value={formData.licenseExpiryDate}
            onChange={(e) => setFormData({ ...formData, licenseExpiryDate: e.target.value })}
          />
          <FormControl fullWidth margin="dense">
            <InputLabel>Tipo de Chofer</InputLabel>
            <Select
              value={formData.driverType}
              onChange={(e) => setFormData({ ...formData, driverType: e.target.value })}
              label="Tipo de Chofer"
            >
              <MenuItem value={1}>Maquinaria Liviana</MenuItem>
              <MenuItem value={2}>Maquinaria Pesada</MenuItem>
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense">
            <InputLabel>Estado</InputLabel>
            <Select
              value={formData.status}
              onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              label="Estado"
            >
              <MenuItem value={1}>Activo</MenuItem>
              <MenuItem value={2}>Inactivo</MenuItem>
              <MenuItem value={3}>De Vacaciones</MenuItem>
              <MenuItem value={4}>Suspendido</MenuItem>
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button variant="outlined" onClick={() => setOpenEdit(false)}>Cancelar</Button>
          <Button variant="contained" onClick={handleSubmitEdit}>Actualizar</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Dialog */}
      <Dialog open={openDelete} onClose={() => setOpenDelete(false)} fullWidth maxWidth="sm">
        <DialogTitle sx={{ fontWeight: 600 }}>Eliminar Chofer</DialogTitle>
        <DialogContent>
          <Typography>¿Estás seguro de que quieres eliminar al chofer {deletingDriver?.firstName} {deletingDriver?.lastName}?</Typography>
          <TextField
            autoFocus
            margin="dense"
            label="Eliminado por"
            fullWidth
            value={deleteForm.deletedBy}
            onChange={(e) => setDeleteForm({ ...deleteForm, deletedBy: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Razón"
            fullWidth
            value={deleteForm.reason}
            onChange={(e) => setDeleteForm({ ...deleteForm, reason: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button variant="outlined" onClick={() => setOpenDelete(false)}>Cancelar</Button>
          <Button variant="contained" color="error" onClick={handleConfirmDelete}>Eliminar</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Drivers;