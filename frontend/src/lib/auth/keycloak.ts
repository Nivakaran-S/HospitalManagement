import Keycloak from 'keycloak-js';

const keycloakConfig = {
  url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8080',
  realm: process.env.NEXT_PUBLIC_KEYCLOAK_REALM || 'hospitalrealm',
  clientId: process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'hospital-client',
};

let keycloakInstance: Keycloak | null = null;

export const initKeycloak = (): Keycloak => {
  if (!keycloakInstance) {
    keycloakInstance = new Keycloak(keycloakConfig);
  }
  return keycloakInstance;
};

export const getKeycloak = (): Keycloak | null => {
  return keycloakInstance;
};

export const login = async (): Promise<boolean> => {
  const keycloak = initKeycloak();
  try {
    const authenticated = await keycloak.init({
      onLoad: 'login-required',
      checkLoginIframe: false,
    });
    return authenticated;
  } catch (error) {
    console.error('Keycloak initialization failed:', error);
    return false;
  }
};

export const logout = (): void => {
  const keycloak = getKeycloak();
  if (keycloak) {
    keycloak.logout();
  }
};

export const getToken = (): string | undefined => {
  const keycloak = getKeycloak();
  return keycloak?.token;
};

export const getUserRole = (): string | null => {
  const keycloak = getKeycloak();
  if (keycloak?.tokenParsed?.realm_access?.roles) {
    const roles = keycloak.tokenParsed.realm_access.roles;
    if (roles.includes('admin')) return 'admin';
    if (roles.includes('doctor')) return 'doctor';
    if (roles.includes('patient')) return 'patient';
    if (roles.includes('staff')) return 'staff';
  }
  return null;
};

export const updateToken = async (minValidity: number = 30): Promise<boolean> => {
  const keycloak = getKeycloak();
  if (keycloak) {
    try {
      return await keycloak.updateToken(minValidity);
    } catch (error) {
      console.error('Failed to refresh token:', error);
      return false;
    }
  }
  return false;
};