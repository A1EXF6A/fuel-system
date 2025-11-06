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
  TextField,
  Grid,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Tabs,
  Tab
} from '@mui/material';
import {
  People,
  DriveEta,
  LocalShipping,
  Route,
  Assessment,
  Logout,
  Edit,
  CheckCircle
} from '@mui/icons-material';

const drawerWidth = 240;

const Reports = () => {
  const { user, logout, loading } = useContext(AuthContext);
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('reportes');
  const [reports, setReports] = useState([]);
  const [vehiclePlacaFilter, setVehiclePlacaFilter] = useState('');
  const [estadoFilter, setEstadoFilter] = useState('');
  const [openUpdateLitersDialog, setOpenUpdateLitersDialog] = useState(false);
  const [openUpdateStatusDialog, setOpenUpdateStatusDialog] = useState(false);
  const [selectedReport, setSelectedReport] = useState(null);
  const [newActualLiters, setNewActualLiters] = useState('');
  const [newEstado, setNewEstado] = useState('');

  useEffect(() => {
    if (!loading && !user) {
      navigate('/login');
    } else if (user && user.role !== 'Admin') {
      return;
    } else if (user && user.role === 'Admin') {
      fetchReports();
    }
  }, [user, loading, navigate]);

  const fetchReports = async () => {
    try {
      const response = await axios.get('http://localhost:5010/api/fuel/reports');
      let reportsData = response.data.registros || [];
      // Fetch route names for each report
      const reportsWithNames = await Promise.all(reportsData.map(async (report) => {
        try {
          const routeResponse = await axios.get(`http://localhost:5010/api/routes/${report.routeId}`);
          const routeName = routeResponse.data.nombre || 'N/A';
          return { ...report, routeName };
        } catch (error) {
          console.error('Error fetching route:', error);
          return { ...report, routeName: 'N/A' };
        }
      }));
      setReports(reportsWithNames);
    } catch (error) {
      console.error('Error fetching reports:', error);
    }
  };

  const handleUpdateLiters = (report) => {
    setSelectedReport(report);
    setNewActualLiters(report.actualLiters.toString());
    setOpenUpdateLitersDialog(true);
  };

  const handleSubmitUpdateLiters = async () => {
    try {
      await axios.post('http://localhost:5010/api/fuel/register/', {
        planId: selectedReport.id,
        actualLiters: parseFloat(newActualLiters)
      });
      setOpenUpdateLitersDialog(false);
      fetchReports();
    } catch (error) {
      console.error('Error updating actual liters:', error);
    }
  };

  const handleUpdateStatus = (report) => {
    setSelectedReport(report);
    setNewEstado(report.estado);
    setOpenUpdateStatusDialog(true);
  };

  const handleSubmitUpdateStatus = async () => {
    try {
      await axios.post(`http://localhost:5010/api/fuel/reports/${selectedReport.id}/status/`, {
        estado: newEstado
      });
      setOpenUpdateStatusDialog(false);
      fetchReports();
    } catch (error) {
      console.error('Error updating status:', error);
    }
  };

  useEffect(() => {
    if (!loading && !user) {
      navigate('/login');
    } else if (user && user.role !== 'Admin') {
      return;
    } else if (user && user.role === 'Admin') {
      fetchReports();
    }
  }, [user, loading, navigate]);

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
            Fuel System - Reportes
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
        <Tabs value={activeTab} onChange={(e, newValue) => setActiveTab(newValue)} sx={{ mb: 2 }}>
          <Tab label="Reportes" value="reportes" />
          <Tab label="Comparación" value="comparacion" />
        </Tabs>
        {activeTab === 'reportes' && (
          <>
            <Typography variant="h4" gutterBottom>
              Reportes de Combustible
            </Typography>
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Filtrar por Vehículo (Placa)"
              value={vehiclePlacaFilter}
              onChange={(e) => setVehiclePlacaFilter(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
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
                 <TableCell>Vehículo</TableCell>
                 <TableCell>Nombre de la Ruta</TableCell>
                 <TableCell>Estimado (L)</TableCell>
                 <TableCell>Actual (L)</TableCell>
                 <TableCell>Estado</TableCell>
                 <TableCell>Acciones</TableCell>
               </TableRow>
             </TableHead>
            <TableBody>
               {reports.filter(report =>
                 (!vehiclePlacaFilter || report.vehiclePlaca.toLowerCase().includes(vehiclePlacaFilter.toLowerCase())) &&
                 (!estadoFilter || report.estado.toLowerCase().includes(estadoFilter.toLowerCase()))
               ).map((report) => (
                 <TableRow key={report.id}>
                   <TableCell>{report.id}</TableCell>
                   <TableCell>{report.vehiclePlaca}</TableCell>
                   <TableCell>{report.routeName}</TableCell>
                    <TableCell>{Math.round(report.estimatedLiters)}</TableCell>
                   <TableCell>{report.actualLiters}</TableCell>
                   <TableCell>{report.estado}</TableCell>
                   <TableCell>
                     <IconButton onClick={() => handleUpdateLiters(report)}>
                       <Edit />
                     </IconButton>
                     <IconButton onClick={() => handleUpdateStatus(report)}>
                       <CheckCircle />
                     </IconButton>
                   </TableCell>
                 </TableRow>
               ))}
            </TableBody>
          </Table>
        </TableContainer>
          </>
        )}
        {activeTab === 'comparacion' && (
          <>
            <Typography variant="h4" gutterBottom>
              Comparación de Consumo Estimado vs Real
            </Typography>
            <Grid container spacing={2} sx={{ mb: 2 }}>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Filtrar por Vehículo (Placa)"
                  value={vehiclePlacaFilter}
                  onChange={(e) => setVehiclePlacaFilter(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
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
                    <TableCell>Vehículo</TableCell>
                    <TableCell>Nombre de la Ruta</TableCell>
                    <TableCell>Estimado (L)</TableCell>
                    <TableCell>Actual (L)</TableCell>
                    <TableCell>Diferencia (L)</TableCell>
                    <TableCell>Estado</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {reports.filter(report =>
                    (!vehiclePlacaFilter || report.vehiclePlaca.toLowerCase().includes(vehiclePlacaFilter.toLowerCase())) &&
                    (!estadoFilter || report.estado.toLowerCase().includes(estadoFilter.toLowerCase()))
                  ).map((report) => {
                    const estimated = Math.round(report.estimatedLiters);
                    const actual = report.actualLiters;
                    const difference = actual - estimated;
                    return (
                      <TableRow key={report.id}>
                        <TableCell>{report.id}</TableCell>
                        <TableCell>{report.vehiclePlaca}</TableCell>
                        <TableCell>{report.routeName}</TableCell>
                        <TableCell>{estimated}</TableCell>
                        <TableCell>{actual}</TableCell>
                        <TableCell style={{ color: difference > 0 ? 'red' : difference < 0 ? 'green' : 'black' }}>
                          {difference > 0 ? '+' : ''}{difference}
                        </TableCell>
                        <TableCell>{report.estado}</TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}

        <Dialog open={openUpdateLitersDialog} onClose={() => setOpenUpdateLitersDialog(false)}>
          <DialogTitle>Actualizar Litros Actuales</DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              label="Litros Actuales"
              type="number"
              fullWidth
              value={newActualLiters}
              onChange={(e) => setNewActualLiters(e.target.value)}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenUpdateLitersDialog(false)}>Cancelar</Button>
            <Button onClick={handleSubmitUpdateLiters}>Actualizar</Button>
          </DialogActions>
        </Dialog>

        <Dialog open={openUpdateStatusDialog} onClose={() => setOpenUpdateStatusDialog(false)}>
          <DialogTitle>Actualizar Estado</DialogTitle>
          <DialogContent>
            <FormControl fullWidth sx={{ mt: 2 }}>
              <InputLabel>Estado</InputLabel>
              <Select
                value={newEstado}
                onChange={(e) => setNewEstado(e.target.value)}
                label="Estado"
              >
                <MenuItem value="Planificado">Planificado</MenuItem>
                <MenuItem value="EnProgreso">EnProgreso</MenuItem>
                <MenuItem value="Completado">Completado</MenuItem>
              </Select>
            </FormControl>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenUpdateStatusDialog(false)}>Cancelar</Button>
            <Button onClick={handleSubmitUpdateStatus}>Actualizar</Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Box>
  );
};

export default Reports;