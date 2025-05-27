# Configuración de Frontend para ServiPuntosUy

Este documento proporciona una guía detallada para configurar una aplicación frontend (React) que trabaje con el sistema multi-tenant de ServiPuntosUy, enfocándose especialmente en cómo simular y trabajar con dominios de usuario final.

## Introducción

ServiPuntosUy utiliza un enfoque de multi-tenancy basado en subdominios, donde cada tenant (cadena de estaciones de servicio) tiene su propio subdominio. Para los usuarios finales, el patrón de URL es:

- `{tenant-id}.app.servipuntos.uy` - Para acceder a la aplicación de un tenant específico
- `app.servipuntos.uy` - Para la aplicación genérica sin tenant específico

Esta guía te ayudará a configurar una aplicación frontend que pueda trabajar con estos patrones de URL y obtener la información específica del tenant para personalizar la experiencia del usuario.

## Configuración del Entorno de Desarrollo

### Creación de un Proyecto React

Puedes utilizar cualquiera de los siguientes frameworks/herramientas para crear tu aplicación React:

- [Create React App](https://create-react-app.dev/docs/getting-started)
- [Vite](https://vitejs.dev/guide/)
- [Next.js](https://nextjs.org/docs/getting-started)

Ejemplo básico con Create React App:

```bash
npx create-react-app servipuntos-frontend
cd servipuntos-frontend
```

### Configuración del Archivo Hosts

Para simular subdominios en desarrollo local, necesitas modificar tu archivo hosts. Esto es similar a lo que se describe en el README principal del proyecto.

#### Windows (C:\Windows\System32\drivers\etc\hosts):
```
127.0.0.1    petrobras.app.servipuntos.local
127.0.0.1    shell.app.servipuntos.local
```

#### macOS/Linux (/etc/hosts):
```
127.0.0.1    petrobras.app.servipuntos.local
127.0.0.1    shell.app.servipuntos.local
```

### Configuración del Servidor de Desarrollo

La mayoría de los servidores de desarrollo de React no están configurados por defecto para manejar subdominios. Necesitarás realizar algunas modificaciones:

### Para Create React App

Crea un archivo `.env` en la raíz del proyecto:

```
HOST=app.servipuntos.local
PORT=3000
```

Luego, modifica el script de inicio en `package.json`:

```json
"scripts": {
  "start": "HTTPS=true react-scripts start",
  // otros scripts...
}
```

### Para Vite

Crea un archivo `vite.config.js` en la raíz del proyecto:

```javascript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    host: 'app.servipuntos.local',
    port: 3000,
    https: true,
  },
});
```

### Para Next.js

Crea o modifica el archivo `next.config.js`:

```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  async rewrites() {
    return [
      {
        source: '/:path*',
        destination: '/:path*',
      },
    ];
  },
};

module.exports = nextConfig;
```

Y ejecuta el servidor con:

```bash
next dev -H app.servipuntos.local -p 3000
```