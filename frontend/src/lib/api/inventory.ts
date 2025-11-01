import { apiClient } from './client';
import { InventoryItem } from '../types/models';

export const inventoryApi = {
  getAll: (params?: { category?: string; search?: string }) => {
    return apiClient.get<InventoryItem[]>('/inventory/Inventory', params);
  },

  getById: (id: number) => {
    return apiClient.get<InventoryItem>(`/inventory/Inventory/${id}`);
  },

  create: (data: Partial<InventoryItem>) => {
    return apiClient.post<InventoryItem>('/inventory/Inventory', data);
  },

  update: (id: number, data: Partial<InventoryItem>) => {
    return apiClient.put<InventoryItem>(`/inventory/Inventory/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/inventory/Inventory/${id}`);
  },

  recordTransaction: (id: number, transaction: any) => {
    return apiClient.post(`/inventory/Inventory/${id}/transaction`, transaction);
  },

  createRequest: (request: any) => {
    return apiClient.post('/inventory/Inventory/request', request);
  },

  getRequests: (status?: string) => {
    return apiClient.get('/inventory/Inventory/requests', { status });
  },

  approveRequest: (id: number, approvedBy: string) => {
    return apiClient.post(`/inventory/Inventory/request/${id}/approve`, approvedBy);
  },

  rejectRequest: (id: number, reason: string) => {
    return apiClient.post(`/inventory/Inventory/request/${id}/reject`, reason);
  },

  getLowStock: () => {
    return apiClient.get('/inventory/Inventory/low-stock');
  },

  getInventoryValue: () => {
    return apiClient.get('/inventory/Inventory/report/value');
  },
};