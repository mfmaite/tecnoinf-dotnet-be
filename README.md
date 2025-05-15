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
docker compose exec <container_name> bash
```

> 💡 Puedes obtener el nombre del contenedor ejecutando `docker compose ps`

2. Conectarse a SQL Server usando sqlcmd:
```bash
/opt/mssql-tools18/bin/sqlcmd -S localhost -U <user_name> -C
```

Después de ejecutar este comando, la consola pedirá la contraseña del mismo.

### Conexión desde herramientas externas

Puedes conectarte usando estas credenciales desde cualquier herramienta SQL (como Azure Data Studio, SQL Server Management Studio, etc.) usando:

- Servidor: localhost
- Puerto: 1433
- Usuario y contraseña: las definidas en el archivo .env
