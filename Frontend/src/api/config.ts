/**
 * API Configuration
 * 
 * This module contains API base URLs and client configuration.
 * Currently, API calls are proxied through Vite (see vite.config.ts).
 */

export const API_BASE_URL = '/api';

/**
 * API endpoints configuration
 */
export const API_ENDPOINTS = {
  COURSES: `${API_BASE_URL}/courses`,
  LESSONS: `${API_BASE_URL}/lessons`,
  USERS: `${API_BASE_URL}/users`,
} as const;
