-- Migration: Add RefreshToken support to AuthUsers table
-- Date: 2025-01-17
-- Description: Adds RefreshToken and RefreshTokenExpiresAt columns to support refresh token functionality

ALTER TABLE auth_users
ADD COLUMN refresh_token VARCHAR(500) NULL,
ADD COLUMN refresh_token_expires_at TIMESTAMP WITH TIME ZONE NULL;

-- Optional: Add index on refresh_token for faster lookups
CREATE INDEX IF NOT EXISTS idx_auth_users_refresh_token 
ON auth_users(refresh_token) 
WHERE refresh_token IS NOT NULL;
