import { apiClient } from './client';

export const reportsApi = {
  getAll: (params?: { reportType?: string; fromDate?: string; toDate?: string }) => {
    return apiClient.get('/reports/Report', params);
  },

  getById: (id: number) => {
    return apiClient.get(`/reports/Report/${id}`);
  },

  generateFinancialSummary: (startDate?: string, endDate?: string) => {
    return apiClient.post('/reports/Report/financial-summary', {startDate, endDate  });
  },

  generatePatientStatistics: (startDate?: string, endDate?: string) => {
    return apiClient.post('/reports/Report/patient-statistics', {  startDate, endDate  });
  },

  generateAppointmentStatistics: (startDate?: string, endDate?: string) => {
    return apiClient.post('/reports/Report/appointment-statistics',  { startDate, endDate } );
  },

  generateInventoryReport: () => {
    return apiClient.post('/reports/Report/inventory-report');
  },

  generatePharmacyReport: (startDate?: string, endDate?: string) => {
    return apiClient.post('/reports/Report/pharmacy-report', {  startDate, endDate  });
  },

  generateLabStatistics: (startDate?: string, endDate?: string) => {
    return apiClient.post('/reports/Report/lab-statistics', { startDate, endDate  });
  },

  generateStaffReport: () => {
    return apiClient.post('/reports/Report/staff-report');
  },

  generateComprehensive: (startDate?: string, endDate?: string) => {
    return apiClient.post('/reports/Report/comprehensive', { startDate, endDate  });
  },
};