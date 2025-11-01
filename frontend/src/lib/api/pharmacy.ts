import { apiClient } from './client';
import { Medicine } from '../types/models';

export const pharmacyApi = {
  getAll: (params?: { search?: string; category?: string; inStock?: boolean }) => {
    return apiClient.get<Medicine[]>('/pharmacy/Pharmacy', params);
  },

  getById: (id: number) => {
    return apiClient.get<Medicine>(`/pharmacy/Pharmacy/${id}`);
  },

  create: (data: Partial<Medicine>) => {
    return apiClient.post<Medicine>('/pharmacy/Pharmacy', data);
  },

  update: (id: number, data: Partial<Medicine>) => {
    return apiClient.put<Medicine>(`/pharmacy/Pharmacy/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/pharmacy/Pharmacy/${id}`);
  },

  addStock: (id: number, stock: any) => {
    return apiClient.post(`/pharmacy/Pharmacy/${id}/stock`, stock);
  },

  recordSale: (id: number, sale: any) => {
    return apiClient.post(`/pharmacy/Pharmacy/${id}/sale`, sale);
  },

  getLowStock: () => {
    return apiClient.get('/pharmacy/Pharmacy/low-stock');
  },

  getExpiringSoon: (days: number = 30) => {
    return apiClient.get('/pharmacy/Pharmacy/expiring-soon', { days });
  },

  getSalesReport: (fromDate?: string, toDate?: string) => {
    return apiClient.get('/pharmacy/Pharmacy/sales-report', { fromDate, toDate });
  },

  getInventoryValue: () => {
    return apiClient.get('/pharmacy/Pharmacy/inventory-value');
  },
};