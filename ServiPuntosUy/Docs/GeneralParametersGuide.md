# Guía de Uso de Parámetros Generales

Esta guía explica cómo utilizar los endpoints de parámetros generales desde el frontend, según el tipo de usuario.

## Introducción

Los parámetros generales son configuraciones a nivel de aplicación que pueden ser utilizadas por todas las partes del sistema. Ejemplos de parámetros generales incluyen:

- `Currency`: Moneda utilizada en la aplicación
- `DefaultLanguage`: Idioma predeterminado
- `ContactEmail`: Email de contacto para soporte
- `MaintenanceMode`: Indica si la aplicación está en modo mantenimiento

## Endpoints Disponibles

| Método | Endpoint | Descripción | Acceso |
|--------|----------|-------------|--------|
| GET | `/api/generalparameter` | Obtiene todos los parámetros | Central Admin, End User |
| GET | `/api/generalparameter/{key}` | Obtiene un parámetro específico | Central Admin, End User |
| PUT | `/api/generalparameter/{key}` | Actualiza un parámetro existente | Solo Central Admin |
| POST | `/api/generalparameter` | Crea un nuevo parámetro | Solo Central Admin |

## Uso por Tipo de Usuario

### Central Admin

El administrador central tiene acceso completo a la gestión de parámetros generales.

#### Obtener todos los parámetros

**Request:**
```http
GET /api/generalparameter
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "error": false,
  "message": "Parámetros obtenidos con éxito",
  "data": [
    {
      "id": 1,
      "key": "Currency",
      "value": "USD",
      "description": "Moneda por defecto para la aplicación"
    },
    {
      "id": 2,
      "key": "DefaultLanguage",
      "value": "es",
      "description": "Idioma predeterminado de la aplicación"
    }
  ]
}
```

#### Obtener un parámetro específico

**Request:**
```http
GET /api/generalparameter/Currency
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "error": false,
  "message": "Parámetro obtenido con éxito",
  "data": {
    "id": 1,
    "key": "Currency",
    "value": "USD",
    "description": "Moneda por defecto para la aplicación"
  }
}
```

#### Crear un nuevo parámetro

**Request:**
```http
POST /api/generalparameter
Authorization: Bearer {token}
Content-Type: application/json

{
  "key": "ContactEmail",
  "value": "support@servipuntos.com",
  "description": "Email de contacto para soporte"
}
```

**Respuesta exitosa (201 Created):**
```json
{
  "error": false,
  "message": "Parámetro creado con éxito",
  "data": {
    "id": 3,
    "key": "ContactEmail",
    "value": "support@servipuntos.com",
    "description": "Email de contacto para soporte"
  }
}
```

#### Actualizar un parámetro existente

**Request:**
```http
PUT /api/generalparameter/Currency
Authorization: Bearer {token}
Content-Type: application/json

{
  "value": "EUR",
  "description": "Moneda por defecto para la aplicación (Euro)"
}
```

**Respuesta exitosa (200 OK):**
```json
{
  "error": false,
  "message": "Parámetro actualizado con éxito",
  "data": {
    "id": 1,
    "key": "Currency",
    "value": "EUR",
    "description": "Moneda por defecto para la aplicación (Euro)"
  }
}
```

### End User

El usuario final solo tiene acceso de lectura a los parámetros generales.

#### Obtener todos los parámetros

**Request:**
```http
GET /api/generalparameter
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "error": false,
  "message": "Parámetros obtenidos con éxito",
  "data": [
    {
      "id": 1,
      "key": "Currency",
      "value": "USD",
      "description": "Moneda por defecto para la aplicación"
    },
    {
      "id": 2,
      "key": "DefaultLanguage",
      "value": "es",
      "description": "Idioma predeterminado de la aplicación"
    }
  ]
}
```

#### Obtener un parámetro específico

**Request:**
```http
GET /api/generalparameter/Currency
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "error": false,
  "message": "Parámetro obtenido con éxito",
  "data": {
    "id": 1,
    "key": "Currency",
    "value": "USD",
    "description": "Moneda por defecto para la aplicación"
  }
}
```

## Implementación en el Frontend

### Para Central Admin

1. **Panel de Administración de Parámetros**:
   - Crear una sección en el panel de administración para gestionar los parámetros generales.
   - Mostrar una tabla con todos los parámetros existentes.
   - Incluir botones para crear, editar y ver detalles de cada parámetro.

2. **Formulario de Creación**:
   - Implementar un formulario con campos para Key, Value y Description.
   - Validar que la clave sea única antes de enviar la solicitud.
   - Mostrar mensajes de éxito o error según la respuesta del servidor.

3. **Formulario de Edición**:
   - Cargar los datos actuales del parámetro en el formulario.
   - Permitir modificar Value y Description (la Key no debe ser editable).
   - Actualizar la vista después de una edición exitosa.

### Para End User

1. **Carga Inicial de Parámetros**:
   - Al iniciar la aplicación, realizar una solicitud para obtener todos los parámetros generales.
   - Almacenar estos parámetros en el almacenamiento local del navegador (localStorage o sessionStorage).

   Ejemplo de almacenamiento:
   ```
   // Después de recibir los parámetros del servidor
   localStorage.setItem('appParameters', JSON.stringify(parametersData));
   ```

2. **Uso de Parámetros en la Aplicación**:
   - Crear un servicio o utilidad que proporcione acceso a los parámetros almacenados.
   - Utilizar estos parámetros para configurar aspectos de la interfaz de usuario.
   
   Por ejemplo, para mostrar precios en la moneda correcta:
   ```
   // Obtener la moneda de los parámetros almacenados
   const parameters = JSON.parse(localStorage.getItem('appParameters'));
   const currency = parameters.find(p => p.key === 'Currency')?.value || 'USD';
   
   // Usar la moneda en la interfaz
   // Ejemplo: $10.00 USD
   ```

3. **Actualización Periódica**:
   - Considerar actualizar los parámetros periódicamente o en eventos específicos (como login).
   - Implementar una estrategia de caché para reducir las solicitudes al servidor.

## Consideraciones Importantes

1. **Seguridad**:
   - Asegurarse de que el token de autenticación esté incluido en todas las solicitudes.
   - Validar en el frontend los permisos del usuario antes de mostrar opciones de edición.

2. **Manejo de Errores**:
   - Implementar manejo adecuado de errores para casos como parámetros no encontrados o problemas de conexión.
   - Mostrar mensajes de error amigables al usuario.

3. **Rendimiento**:
   - Almacenar los parámetros en caché para reducir las solicitudes al servidor.
   - Considerar implementar un mecanismo de actualización basado en eventos o tiempo.

4. **Validación**:
   - Validar los datos de entrada en el frontend antes de enviarlos al servidor.
   - Verificar que las claves de parámetros sean únicas al crear nuevos parámetros.
