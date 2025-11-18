#!/bin/bash
# Script to generate RSA key pair for JWT signing

echo "Generating RSA key pair for JWT signing..."

# Generate private key (2048 bits)
openssl genrsa -out jwt_private.pem 2048

# Extract public key from private key
openssl rsa -in jwt_private.pem -pubout -out jwt_public.pem

echo ""
echo "Keys generated successfully!"
echo "Private key: jwt_private.pem"
echo "Public key: jwt_public.pem"
echo ""
echo "To use in appsettings.json:"
echo "1. Copy the content of jwt_private.pem to Jwt:PrivateKey"
echo "2. Copy the content of jwt_public.pem to Jwt:PublicKey"
echo ""
echo "Note: For production, use environment variables or a secure key vault instead of storing keys in appsettings.json"
