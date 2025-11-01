import { apiClient } from './client';
import { Donor } from '../types/models';

export const donorsApi = {
  getAll: (params?: { bloodGroup?: string; isAvailable?: boolean; search?: string }) => {
    return apiClient.get<Donor[]>('/donors/Donor', params);
  },

  getById: (id: number) => {
    return apiClient.get<Donor>(`/donors/Donor/${id}`);
  },

  create: (data: Partial<Donor>) => {
    return apiClient.post<Donor>('/donors/Donor', data);
  },

  update: (id: number, data: Partial<Donor>) => {
    return apiClient.put<Donor>(`/donors/Donor/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/donors/Donor/${id}`);
  },

  getByBloodGroup: (bloodGroup: string) => {
    return apiClient.get(`/donors/Donor/blood-group/${bloodGroup}`);
  },

  getEligible: () => {
    return apiClient.get('/donors/Donor/eligible');
  },

  recordDonation: (id: number, donation: any) => {
    return apiClient.post(`/donors/Donor/${id}/record-donation`, donation);
  },

  getBloodInventory: () => {
    return apiClient.get('/donors/BloodInventory');
  },

  getBloodInventoryByGroup: (bloodGroup: string) => {
    return apiClient.get(`/donors/BloodInventory/${bloodGroup}`);
  },

  updateBloodInventory: (id: number, data: any) => {
    return apiClient.put(`/donors/BloodInventory/${id}`, data);
  },

  useBlood: (bloodGroup: string, units: number) => {
    return apiClient.post(`/donors/BloodInventory/${bloodGroup}/use`, units);
  },

  getLowStockAlerts: (threshold?: number) => {
    return apiClient.get('/donors/BloodInventory/low-stock', { threshold });
  },
};