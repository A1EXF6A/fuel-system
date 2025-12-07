// API configuration
const API_BASE_URL =
  process.env.REACT_APP_API_BASE_URL || "http://localhost:5010/api";

export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    LOGIN: `${API_BASE_URL}/auth/login`,
    REGISTER: `${API_BASE_URL}/auth/register`,
    USERS: `${API_BASE_URL}/auth/users`,
  },

  // Drivers endpoints
  DRIVERS: {
    BASE: `${API_BASE_URL}/drivers`,
    ASSIGN: (id) => `${API_BASE_URL}/drivers/${id}/assign`,
    UNASSIGN: (id) => `${API_BASE_URL}/drivers/${id}/unassign`,
  },

  // Vehicles endpoints
  VEHICLES: {
    BASE: `${API_BASE_URL}/vehicles`,
    BY_ID: (id) => `${API_BASE_URL}/vehicles/${id}`,
  },

  // Routes endpoints
  ROUTES: {
    BASE: `${API_BASE_URL}/routes`,
    BY_ID: (id) => `${API_BASE_URL}/routes/${id}`,
    STATUS: (id) => `${API_BASE_URL}/routes/${id}/status`,
  },

  // Fuel endpoints
  FUEL: {
    REGISTER: `${API_BASE_URL}/fuel/register`,
    PLAN: `${API_BASE_URL}/fuel/plan`,
    REPORTS: `${API_BASE_URL}/fuel/reports`,
    REPORT_STATUS: (id) => `${API_BASE_URL}/fuel/reports/${id}/status/`,
  },
};

export default API_BASE_URL;
