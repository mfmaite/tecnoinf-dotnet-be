# Integración de Canje de QR - Guía para Frontend

Este documento proporciona un ejemplo sencillo de las peticiones HTTP que un frontend debe realizar para implementar el flujo de canje de QR.

## Secuencia de Peticiones

### 1. Iniciar sesión como usuario final

```http
POST /api/Auth/login
Content-Type: application/json

{
  "Email": "cliente@ejemplo.com",
  "Password": "Admin123!"
}
```

**Respuesta:**
```json
{
  "error": false,
  "message": "Inicio de sesión correcto",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

### 2. Obtener la lista de sucursales (branches)

```http
GET /api/Branch
Authorization: Bearer {token_jwt}
```

**Respuesta:**
```json
{
  "error": false,
  "message": "Lista de branches obtenida correctamente",
  "data": [
    {
      "id": 1,
      "address": "Av. Italia 1234, Montevideo",
      "phone": "26001234",
      "openTime": "08:00:00",
      "closingTime": "22:00:00"
    }
  ]
}
```

### 3. Obtener la lista de productos

```http
GET /api/Product
Authorization: Bearer {token_jwt}
```

**Respuesta:**
```json
{
  "error": false,
  "message": "Lista de productos obtenida correctamente",
  "data": [
    {
      "id": 1,
      "name": "Combustible Premium",
      "description": "Combustible de alta calidad",
      "price": 75.50,
      "imageUrl": "https://example.com/premium-fuel.png",
      "ageRestricted": false
    },
    {
      "id": 2,
      "name": "Aceite de Motor",
      "description": "Aceite sintético para motores",
      "price": 1200.00,
      "imageUrl": "https://example.com/motor-oil.png",
      "ageRestricted": false
    }
  ]
}
```

### 4. Generar un token de canje (QR)

```http
POST /api/Redemption/generate-token
Content-Type: application/json
Authorization: Bearer {token_jwt}

{
  "BranchId": 1,
  "ProductId": 1
}
```

**Respuesta:**
```json
{
  "error": false,
  "message": "QR generado satisfactoriamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

## URL para el QR

La URL que debe codificarse en el QR es:

```
https://api.servipuntos.uy/api/Redemption/process/{token}
```

Donde `{token}` es el token recibido en la respuesta del paso 4.

## Flujo de Canje

1. El usuario inicia sesión en la aplicación móvil
2. El usuario navega a la sección de productos canjeables
3. El usuario selecciona un producto y una sucursal
4. La aplicación genera un código QR que contiene la URL de canje
5. En la sucursal, un empleado escanea el código QR
6. El sistema procesa el canje y muestra una página de confirmación
7. El empleado entrega el producto al usuario

## Notas Importantes

- El token generado tiene una validez limitada (típicamente algunas horas)
- El canje solo puede realizarse una vez por token
- El usuario debe tener suficientes puntos para canjear el producto
- La sucursal debe tener stock disponible del producto
