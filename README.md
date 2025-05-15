# SQL Server con Docker

## Inicialización

1. Iniciar los contenedores:
```bash
docker compose up -d
```

2. Verificar que los contenedores estén corriendo:
```bash
docker compose ps
```

## Conectarse a la base de datos

### Desde la consola del contenedor

1. Entrar al contenedor:
```bash
docker compose exec sqlserver bash
```

2. Conectarse a SQL Server usando sqlcmd:
```bash
/opt/mssql-tools18/bin/sqlcmd -S localhost -U newuser -P "password123" -C
```

### Comandos SQL útiles

Una vez conectado, puedes usar estos comandos (recuerda terminar cada comando con `GO`):

- Ver bases de datos:
```sql
SELECT name FROM sys.databases; GO
```

- Ver tablas de la base de datos actual:
```sql
SELECT name FROM sys.tables; GO
```

- Cambiar a una base de datos específica:
```sql
USE NombreBaseDeDatos; GO
```

### Credenciales

- **Usuario Adicional**
  - Usuario: newuser
  - Contraseña: password123

### Conexión desde herramientas externas

Puedes conectarte usando estas credenciales desde cualquier herramienta SQL (como Azure Data Studio, SQL Server Management Studio, etc.) usando:

- Servidor: localhost
- Puerto: 1433
- Usuario y contraseña: (cualquiera de los mencionados arriba)
