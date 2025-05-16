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
