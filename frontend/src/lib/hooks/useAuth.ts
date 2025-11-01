'use client';

import { useEffect, useState } from 'react';
import { getKeycloak, initKeycloak, getUserRole, getToken } from '../auth/keycloak';
import { apiClient } from '../api/client';

export const useAuth = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState<any>(null);
  const [role, setRole] = useState<string | null>(null);

  useEffect(() => {
    const initAuth = async () => {
      try {
        const keycloak = initKeycloak();
        const authenticated = await keycloak.init({
          onLoad: 'check-sso',
          checkLoginIframe: false,
        });

        if (authenticated) {
          setIsAuthenticated(true);
          const userRole = getUserRole();
          setRole(userRole);
          
          const token = getToken();
          if (token) {
            apiClient.setToken(token);
          }

          if (keycloak.tokenParsed) {
            setUser({
              id: keycloak.subject,
              email: keycloak.tokenParsed.email,
              name: keycloak.tokenParsed.name,
              role: userRole,
            });
          }
        }
      } catch (error) {
        console.error('Auth initialization failed:', error);
      } finally {
        setIsLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = () => {
    const keycloak = getKeycloak();
    if (keycloak) {
      keycloak.login();
    }
  };

  const logout = () => {
    const keycloak = getKeycloak();
    if (keycloak) {
      keycloak.logout();
    }
  };

  return {
    isAuthenticated,
    isLoading,
    user,
    role,
    login,
    logout,
  };
};