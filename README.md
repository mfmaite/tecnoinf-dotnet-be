# Getting Started

En este repositorio se aloja el código del proyecto backend correspondiente a la tarea de la materia **.NET**, dictada en el Tecnólogo en Informática, año 2025.

## 👣 Primeros Pasos

Si es la primera vez que inicias este proyecto, debes seguir los siguientes pasos:

### 🔧 Precondiciones

- Tener instalado Docker y Docker Compose.
- Tener instalado .NET.
- Haber clonado el repositorio en tu máquina local.

### 📦 Instalar dependencias

1. Navega a la raíz del proyecto y ejecuta:
```bash
   cd ServiPuntosUy
```

2. Una vez dentro de la carpeta `ServiPuntosUy`, ejecuta:
 ```bash
   dotnet restore
```
Esto descargará las librerías necesarias para el proyecto.

3. Aplicar las migraciones pendientes:
```bash
dotnet ef database update <db_context>
```

4. Chequear que la connection string de `appsettings.json` sea la aceduada
```json
...
   "ConnectionStrings": {
    "CentralConnection": "Server=localhost,1433;Database=ServiPuntosCentral;User Id=tecnoinf;Password=password123;TrustServerCertificate=True;"
  },
  ...
```

> 💡 El DbContext de la base Central es `CentralDbContext`

### 🚀 Inicialización

1. Inicia los contenedores:
```bash
docker compose up -d
```

2. Verificar que los contenedores estén corriendo:
```bash
docker compose ps
```

3. Posicionado en la carpeta `ServiPuntosUy`, ejecuta:
```bash
dotnet run
```

## 📚 Documentación API (Swagger)

Una vez que la aplicación esté corriendo, puedes acceder a la documentación de la API en:

```
http://localhost:5162/swagger
```


## ⚙️ Troubleshooting

#### 🔌 Conectarse a la base de datos por línea de comandos

1. Ingresa al contenedor:
```bash
docker compose exec <container_name> bash
```

> 💡 Puedes obtener el nombre del contenedor ejecutando `docker compose ps`

2. Conectarse a SQL Server usando `sqlcmd`:
```bash
/opt/mssql-tools18/bin/sqlcmd -S localhost -U <user_name> -C
```

Después de ejecutar este comando, la consola pedirá la contraseña correspondiente.

#### 🧩 Conexión desde herramientas externas

Puedes conectarte usando estas credenciales desde cualquier herramienta SQL (como Azure Data Studio, SQL Server Management Studio, etc.) usando:

- Servidor: `localhost`
- Puerto: `1433`
- Usuario y contraseña: las definidas en el archivo `.env`


## 🖥️ Configuración del Archivo Hosts

Para probar el sistema multi-tenant en desarrollo local, configura el archivo hosts:

#### Windows (C:\Windows\System32\drivers\etc\hosts):
```
127.0.0.1    admin.servipuntos.local
127.0.0.1    petrobras.admin.servipuntos.local
127.0.0.1    shell.admin.servipuntos.local
127.0.0.1    petrobras.branch.admin.servipuntos.local
127.0.0.1    shell.branch.admin.servipuntos.local
127.0.0.1    app.servipuntos.local
127.0.0.1    api.servipuntos.local
```

#### macOS/Linux (/etc/hosts):
```
127.0.0.1    admin.servipuntos.local
127.0.0.1    petrobras.admin.servipuntos.local
127.0.0.1    shell.admin.servipuntos.local
127.0.0.1    petrobras.branch.admin.servipuntos.local
127.0.0.1    shell.branch.admin.servipuntos.local
127.0.0.1    app.servipuntos.local
127.0.0.1    api.servipuntos.local
```

Esto debido a que la resolución de tenant y rol se basa en el dominio y subdominio de las requests.
Podemos agregar tantos tenants como se nos de la gana, no fomentamos el uso de wildcard (*) porque no en todos los sistemas opeartivos funciona. 

Ej: si quisiera agregar ancap, sería agregar: 
```
127.0.0.1    ancap.admin.servipuntos.local
127.0.0.1    ancap.branch.admin.servipuntos.local
```



## Estructura del Proyecto

```
ServiPuntosUy/
├── Controllers/           # Controladores de la API
│   ├── Base/              # Controladores base
│   └── Response/          # Clases de respuesta de la API
├── DAL/                   # Capa de acceso a datos
├── DAO/                   # Objetos de acceso a datos
│   ├── Data/              # Contextos de base de datos
│   │   ├── Central/       # Contexto para la plataforma central
│   │   └── ...
│   ├── Migrations/        # Migraciones de Entity Framework
│   └── Models/            # Modelos de datos
│       ├── Central/       # Modelos para la plataforma central
│       └── ...
├── DataServices/          # Servicios de datos
│   ├── Services/          # Interfaces de servicios
│   │   ├── Central/       # Implementaciones para administrador central
│   │   ├── Tenant/        # Implementaciones para administrador de tenant
│   │   ├── Branch/        # Implementaciones para administrador de estación
│   │   ├── EndUser/       # Implementaciones para usuario final
│   │   └── CommonLogic/   # Lógica común para todos los servicios
│   └── ...
├── DTO/                   # Objetos de transferencia de datos
├── Enums/                 # Enumeraciones
├── Middlewares/           # Middlewares personalizados
└── Views/                 # Vistas MVC para el backoffice
```

## Sistema Multi-Tenant

ServiPuntos.uy utiliza un enfoque de multi-tenancy basado en subdominios, donde cada tenant (cadena de estaciones de servicio) tiene su propio subdominio. Además, cada tipo de usuario tiene un patrón de URL específico:

### Patrones de URL

- **Administrador Central**: `admin.servipuntos.uy`
- **Administrador de Tenant**: `{tenant-id}.admin.servipuntos.uy`
- **Administrador de Estación**: `{tenant-id}.branch.admin.servipuntos.uy`
- **Usuario Final**: `app.servipuntos.uy`
- **API**: `api.servipuntos.uy`

Para más detalles sobre el sistema multi-tenant, consulta la [documentación de multi-tenancy](ServiPuntosUy/Docs/MultiTenancy.md).

## 📚 Documentación Técnica

Para facilitar el desarrollo y mantenimiento del proyecto, se ha creado la siguiente documentación técnica:

### Arquitectura y Diseño

- [**Multi-Tenancy**](ServiPuntosUy/Docs/MultiTenancy.md): Explica cómo funciona el sistema multi-tenant, la resolución de tenants y tipos de usuario.
- [**BaseController**](ServiPuntosUy/Docs/BaseControllerGuide.md): Guía sobre el controlador base, sus propiedades y métodos heredados, y mejores prácticas para su uso.

### Seguridad

- [**Autenticación**](ServiPuntosUy/Docs/AuthenticationGuide.md): Guía sobre el sistema de autenticación JWT, cómo funciona y cómo usarlo.
- [**Configuración CORS**](ServiPuntosUy/Docs/CorsConfiguration.md): Explica la configuración de CORS para desarrollo y producción.