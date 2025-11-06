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
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  IconButton,
  Alert,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Grid
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

const Users = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [open, setOpen] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [usernameFilter, setUsernameFilter] = useState('');
  const [roleFilter, setRoleFilter] = useState('');
  const [formData, setFormData] = useState({
     username: '',
     password: '',
     role: '',
     firstName: '',
     lastName: '',
     phoneNumber: '',
     email: '',
     licenseNumber: '',
     licenseCategory: 2,
     licenseExpiryDate: '',
     driverType: 1,
     hireDate: '',
     status: 1
   });
  const [driverData, setDriverData] = useState(null);
  const [error, setError] = useState('');

   useEffect(() => {
     if (!loading && !user) {
       navigate('/login');
     } else if (user && user.role === 'Operador') {
       return;
     } else if (user && (user.role === 'Admin' || user.role === 'Supervisor')) {
       fetchUsers();
     }
   }, [user, loading, navigate]);

  const fetchUsers = async () => {
    try {
      const response = await axios.get('http://localhost:5010/api/auth/users');
      setUsers(response.data);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  const handleCreate = () => {
    setEditingUser(null);
    setDriverData(null);
     setFormData({
       username: '',
       password: '',
       role: 'Admin',
       firstName: '',
       lastName: '',
       phoneNumber: '',
       email: '',
       licenseNumber: '',
       licenseCategory: 2,
       licenseExpiryDate: '',
       driverType: 1,
       hireDate: new Date().toISOString().slice(0, 16),
       status: 1
     });
    setOpen(true);
  };

  const handleEdit = async (user) => {
    setEditingUser(user);
     setFormData({
       username: user.username,
       password: '',
       role: user.role,
       firstName: '',
       lastName: '',
       phoneNumber: '',
       email: '',
       licenseNumber: '',
       licenseCategory: 2,
       licenseExpiryDate: '',
       driverType: 1,
       hireDate: '',
       status: 1
     });
    if (user.role === 'Operador') {
      try {
        const response = await axios.get(`http://localhost:5010/api/drivers`);
        const drivers = response.data.Drivers || response.data;
        const driver = Array.isArray(drivers) ? drivers.find(d => d.documentNumber === user.username) : null;
        if (driver) {
          setDriverData(driver);
          setFormData(prev => ({
            ...prev,
            firstName: driver.firstName || '',
            lastName: driver.lastName || '',
            phoneNumber: driver.phoneNumber || '',
            email: driver.email || '',
            licenseNumber: driver.licenseNumber || '',
            licenseCategory: driver.licenseCategory ? parseInt(driver.licenseCategory) : 2,
            licenseExpiryDate: driver.licenseExpiryDate ? new Date(driver.licenseExpiryDate).toISOString().slice(0, 16) : '',
            driverType: driver.driverType ? parseInt(driver.driverType) : 1,
            hireDate: driver.hireDate ? new Date(driver.hireDate).toISOString().slice(0, 16) : ''
          }));
        }
      } catch (error) {
        console.error('Error fetching driver:', error);
      }
    }
    setOpen(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar este usuario?')) {
      try {
        await axios.delete(`http://localhost:5010/api/auth/users/${id}`);
        fetchUsers();
      } catch (error) {
        console.error('Error deleting user:', error);
      }
    }
  };

  const handleSubmit = async () => {
    setError('');
    // Validation
    if (!formData.username || !formData.role) {
      setError('Username and Role are required');
      return;
    }
    if (!editingUser && !formData.password) {
      setError('Password is required for new users');
      return;
    }
    if (formData.role === 'Operador') {
      if (!formData.firstName || !formData.lastName || !formData.phoneNumber || !formData.email || !formData.licenseNumber || !formData.licenseExpiryDate || !formData.hireDate) {
        setError('All driver fields are required for Operador role');
        return;
      }
    }
    try {
      let userResponse;
      if (editingUser) {
        userResponse = await axios.put(`http://localhost:5010/api/auth/users/${editingUser.id}`, {
          username: formData.username,
          password: formData.password || undefined,
          role: formData.role
        });
      } else {
        userResponse = await axios.post('http://localhost:5010/api/auth/register', {
          username: formData.username,
          password: formData.password,
          role: formData.role
        });
      }

      if (formData.role === 'Operador') {
        const driverPayload = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          documentNumber: formData.username,
          phoneNumber: formData.phoneNumber,
          email: formData.email,
          licenseNumber: formData.licenseNumber,
          licenseCategory: formData.licenseCategory,
          licenseExpiryDate: formData.licenseExpiryDate,
          driverType: formData.driverType,
          hireDate: formData.hireDate,
          status: formData.status
        };

        if (editingUser && driverData) {
          await axios.put(`http://localhost:5010/api/drivers/${driverData.id}`, driverPayload);
        } else {
          await axios.post('http://localhost:5010/api/drivers', driverPayload);
        }
      }

      setOpen(false);
      fetchUsers();
    } catch (error) {
      setError(error.response?.data?.message || 'Error saving user or driver');
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
            Fuel System - Usuarios
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
           <Typography variant="h4">Usuarios</Typography>
           {user.role === 'Admin' && (
             <Button variant="contained" startIcon={<Add />} onClick={handleCreate}>
               Nuevo Usuario
             </Button>
           )}
         </Box>
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Filtrar por Username"
              value={usernameFilter}
              onChange={(e) => setUsernameFilter(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Filtrar por Role"
              value={roleFilter}
              onChange={(e) => setRoleFilter(e.target.value)}
            />
          </Grid>
        </Grid>
        <TableContainer component={Paper}>
          <Table>
             <TableHead>
               <TableRow>
                 <TableCell>ID</TableCell>
                 <TableCell>Username</TableCell>
                 <TableCell>Role</TableCell>
                 {user.role === 'Admin' && <TableCell>Acciones</TableCell>}
               </TableRow>
             </TableHead>
            <TableBody>
               {users.filter(u =>
                 (!usernameFilter || u.username.toLowerCase().includes(usernameFilter.toLowerCase())) &&
                 (!roleFilter || u.role.toLowerCase().includes(roleFilter.toLowerCase()))
               ).map((rowUser) => (
                 <TableRow key={rowUser.id}>
                   <TableCell>{rowUser.id}</TableCell>
                   <TableCell>{rowUser.username}</TableCell>
                   <TableCell>{rowUser.role}</TableCell>
                   <TableCell>
                     {user.role === 'Admin' && (
                       <>
                         <IconButton onClick={() => handleEdit(rowUser)}>
                           <Edit />
                         </IconButton>
                         <IconButton onClick={() => handleDelete(rowUser.id)}>
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

        <Dialog open={open} onClose={() => setOpen(false)}>
          <DialogTitle>{editingUser ? 'Editar Usuario' : 'Nuevo Usuario'}</DialogTitle>
          <DialogContent>
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
            <TextField
              autoFocus
              margin="dense"
              label="Username"
              fullWidth
              value={formData.username}
              onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            />
            <TextField
              margin="dense"
              label="Password"
              type="password"
              fullWidth
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
            />
             <FormControl fullWidth margin="dense">
               <InputLabel>Role</InputLabel>
               <Select
                 value={formData.role}
                 onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                 label="Role"
               >
                 <MenuItem value="Admin">Admin</MenuItem>
                 <MenuItem value="Supervisor">Supervisor</MenuItem>
                 <MenuItem value="Operador">Operador</MenuItem>
               </Select>
             </FormControl>
             {formData.role === 'Operador' && (
               <>
                 <TextField
                   margin="dense"
                   label="Número de Documento"
                   fullWidth
                   value={formData.username}
                   InputProps={{
                     readOnly: true,
                   }}
                 />
                 <TextField
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
                 <TextField
                   margin="dense"
                   label="Fecha de Contratación"
                   fullWidth
                   type="datetime-local"
                   value={formData.hireDate}
                   onChange={(e) => setFormData({ ...formData, hireDate: e.target.value })}
                 />
               </>
             )}
           </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpen(false)}>Cancel</Button>
            <Button onClick={handleSubmit}>Save</Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Box>
  );
};

export default Users;