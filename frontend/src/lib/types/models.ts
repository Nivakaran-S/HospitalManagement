export interface Patient {
  id: number;
  keycloakUserId: string;
  firstName: string;
  lastName: string;
  dob: string;
  gender: string;
  bloodGroup: string;
  contactNumber: string;
  email: string;
  address: string;
  emergencyContact: string;
  emergencyContactNumber: string;
  insuranceProvider?: string;
  insuranceNumber?: string;
  medicalHistory?: string;
  allergies?: string;
  chronicConditions?: string;
  currentMedications?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Doctor {
  id: number;
  keycloakUserId: string;
  name: string;
  speciality: string;
  contactNumber: string;
  email: string;
  department: string;
  qualifications: string;
  experienceYears: number;
  licenseNumber: string;
  consultationFee: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Appointment {
  id: number;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  appointmentDate: string;
  appointmentTime: string;
  durationMinutes: number;
  status: string;
  reason: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Prescription {
  id: number;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  prescribedDate: string;
  diagnosis: string;
  notes?: string;
  status: string;
  medicines: PrescriptionMedicine[];
}

export interface PrescriptionMedicine {
  id: number;
  prescriptionId: number;
  medicineName: string;
  dosage: string;
  frequency: string;
  duration: string;
  route: string;
  quantity: number;
  instructions: string;
  isDispensed: boolean;
}

export interface Billing {
  id: number;
  patientId: number;
  patientName: string;
  amount: number;
  billingDate: string;
  status: string;
  description: string;
  paidAmount: number;
  dueAmount: number;
  paidDate?: string;
  billingItems: BillingItem[];
}

export interface BillingItem {
  id: number;
  billingId: number;
  itemType: string;
  itemName: string;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Staff {
  id: number;
  keycloakUserId: string;
  firstName: string;
  lastName: string;
  staffRoleId: number;
  contactNumber: string;
  email: string;
  department: string;
  joiningDate: string;
  employeeId: string;
  salary: number;
  address: string;
  qualification: string;
  shiftTiming: string;
  isActive: boolean;
}

export interface Medicine {
  id: number;
  name: string;
  genericName: string;
  manufacturer: string;
  description: string;
  category: string;
  dosageForm: string;
  strength: string;
  quantityInStock: number;
  reorderLevel: number;
  unitPrice: number;
  expiryDate: string;
  batchNumber: string;
  requiresPrescription: boolean;
  isActive: boolean;
}

export interface LabTest {
  id: number;
  patientId: number;
  patientName: string;
  doctorId?: number;
  doctorName?: string;
  testName: string;
  testCategory: string;
  orderedDate: string;
  testDate?: string;
  completedDate?: string;
  result?: string;
  status: string;
  notes?: string;
  cost: number;
  isUrgent: boolean;
}

export interface InventoryItem {
  id: number;
  itemName: string;
  itemCode: string;
  category: string;
  quantity: number;
  unit: string;
  unitPrice: number;
  location: string;
  description: string;
  reorderLevel: number;
  supplier: string;
  isActive: boolean;
}

export interface Donor {
  id: number;
  name: string;
  bloodGroup: string;
  contactNumber: string;
  email: string;
  dateOfBirth: string;
  gender: string;
  address: string;
  lastDonationDate: string;
  isAvailable: boolean;
  weight: number;
  medicalHistory?: string;
}

export interface Notification {
  id: number;
  recipientId: number;
  recipientType: string;
  notificationType: string;
  channel: string;
  subject: string;
  message: string;
  priority: string;
  sentAt: string;
  isRead: boolean;
  readAt?: string;
  status: string;
}