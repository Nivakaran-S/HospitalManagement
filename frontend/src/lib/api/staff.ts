import { apiClient } from './client';
import { Staff } from '../types/models';

export const staffApi = {
  getAll: (params?: { roleId?: number; department?: string; activeOnly?: boolean }) => {
    return apiClient.get<Staff[]>('/staff/Staff', params);
  },

  getById: (id: number) => {
    return apiClient.get<Staff>(`/staff/Staff/${id}`);
  },

  getMyProfile: () => {
    return apiClient.get<Staff>('/staff/Staff/me');
  },

  create: (data: Partial<Staff>) => {
    return apiClient.post<Staff>('/staff/Staff', data);
  },

  update: (id: number, data: Partial<Staff>) => {
    return apiClient.put<Staff>(`/staff/Staff/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/staff/Staff/${id}`);
  },

  markAttendance: (id: number, attendance: any) => {
    return apiClient.post(`/staff/Staff/${id}/mark-attendance`, attendance);
  },

  requestLeave: (id: number, leave: any) => {
    return apiClient.post(`/staff/Staff/${id}/request-leave`, leave);
  },

  approveLeave: (leaveId: number) => {
    return apiClient.put<void>(`/staff/Staff/leave/${leaveId}/approve`);
  },

  rejectLeave: (leaveId: number) => {
    return apiClient.put<void>(`/staff/Staff/leave/${leaveId}/reject`);
  },

  getAttendanceReport: (fromDate?: string, toDate?: string) => {
    return apiClient.get('/staff/Staff/attendance/report', { fromDate, toDate });
  },

  getRoles: () => {
    return apiClient.get('/staff/StaffRole');
  },

  createRole: (role: any) => {
    return apiClient.post('/staff/StaffRole', role);
  },

  getRolePermissions: (roleId: number) => {
    return apiClient.get(`/staff/StaffRole/${roleId}/permissions`);
  },

  getMyPermissions: () => {
    return apiClient.get('/staff/StaffRole/my-permissions');
  },
};