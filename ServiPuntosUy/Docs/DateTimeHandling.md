# Manejo de Fechas y UTC en ServiPuntosUy

## Índice
1. [Introducción a UTC](#introducción-a-utc)
2. [Por qué usar UTC en aplicaciones](#por-qué-usar-utc-en-aplicaciones)
3. [Manejo de fechas UTC en .NET](#manejo-de-fechas-utc-en-net)
4. [Implementación en ServiPuntosUy](#implementación-en-servipuntosuy)
5. [Mejores prácticas](#mejores-prácticas)

## Introducción a UTC

UTC (Tiempo Universal Coordinado) es el estándar de tiempo principal por el cual el mundo regula los relojes y el tiempo. Es el sucesor del GMT (Greenwich Mean Time) y se basa en relojes atómicos altamente precisos.

Características principales:
- No cambia con las estaciones (no tiene horario de verano)
- Es el mismo en todo el mundo en un momento dado
- Se expresa como una hora base, y las zonas horarias locales se definen como desplazamientos de UTC (por ejemplo, UTC-3 para Uruguay)

## Por qué usar UTC en aplicaciones

### Consistencia global

Cuando una aplicación opera en múltiples zonas horarias o tiene usuarios distribuidos geográficamente, UTC proporciona un punto de referencia consistente. Esto es crucial para:

- **Operaciones secuenciales**: Garantizar que los eventos se procesen en el orden correcto
- **Registros y auditorías**: Mantener un registro preciso de cuándo ocurrieron los eventos
- **Sincronización**: Coordinar acciones entre diferentes sistemas

### Problemas que resuelve

1. **Cambios de horario de verano**: Los cambios de DST (Daylight Saving Time) pueden causar:
   - Horas duplicadas (cuando el reloj se atrasa)
   - Horas inexistentes (cuando el reloj se adelanta)
   - Cálculos incorrectos de duración

2. **Ambigüedad de zona horaria**: Sin UTC, es difícil saber si "3:00 PM" se refiere a la hora local del usuario, del servidor, o de alguna otra referencia.

3. **Operaciones distribuidas**: En sistemas que operan en múltiples regiones, UTC proporciona una referencia común para la coordinación.

### Casos de uso específicos

- **Transacciones financieras**: Registro preciso de cuándo ocurrió una transacción
- **Programación de eventos**: Evitar confusiones sobre cuándo comienza un evento
- **Expiración de datos**: Cálculo correcto de cuándo expira un token o recurso
- **Análisis de datos**: Comparación precisa de eventos a lo largo del tiempo

## Manejo de fechas UTC en .NET

### Creación de fechas UTC

```csharp
// Obtener la fecha y hora actual en UTC
DateTime utcNow = DateTime.UtcNow;

// Crear una fecha específica en UTC
DateTime specificUtcDate = new DateTime(2025, 5, 28, 15, 30, 0, DateTimeKind.Utc);

// Convertir una fecha local a UTC
DateTime localDate = DateTime.Now;
DateTime convertedToUtc = localDate.ToUniversalTime();
```

### Almacenamiento en base de datos

En SQL Server, los tipos de datos para fechas no almacenan información de zona horaria:

- `datetime2`: Almacena la fecha y hora sin información de zona horaria
- `datetimeoffset`: Almacena la fecha, hora y el desplazamiento de zona horaria

Cuando usamos `datetime2` (como en ServiPuntosUy), es crucial mantener la convención de que todos los valores son UTC.

### Conversión entre UTC y hora local

```csharp
// Convertir de UTC a hora local
DateTime utcDate = DateTime.UtcNow;
DateTime localDate = utcDate.ToLocalTime();

// Convertir a una zona horaria específica
TimeZoneInfo uruguayZone = TimeZoneInfo.FindSystemTimeZoneById("America/Montevideo");
DateTime uruguayTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate, uruguayZone);

// Formatear para mostrar al usuario
string formattedDate = uruguayTime.ToString("dd/MM/yyyy HH:mm:ss");
```

### Trabajando con TimeSpan y cálculos de tiempo

```csharp
// Calcular la diferencia entre dos fechas UTC
DateTime startUtc = DateTime.UtcNow;
DateTime endUtc = startUtc.AddHours(48);
TimeSpan duration = endUtc - startUtc; // Siempre 48 horas exactas

// Verificar si una fecha ha expirado
bool isExpired = DateTime.UtcNow > expiryDateUtc;
```

## Implementación en ServiPuntosUy

En ServiPuntosUy, ya estamos siguiendo las mejores prácticas para el manejo de fechas UTC:

### En los modelos

```csharp
// Ejemplo de User.cs
public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;

// Ejemplo de Transaction.cs
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

// Ejemplo de Promotion.cs
public DateTime StartDate { get; set; } = DateTime.UtcNow;
public DateTime EndDate { get; set; } = DateTime.UtcNow;
```

### En los servicios

```csharp
// Ejemplo de LoyaltyService.cs
var daysSinceLastLogin = (DateTime.UtcNow - user.LastLoginDate).TotalDays;

// Ejemplo de VEAIService.cs
var hoy = DateTime.UtcNow.Date;
```

### En la base de datos

En nuestras migraciones, los campos de fecha se definen como `datetime2`:

```csharp
// Ejemplo de migración
StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
```

## Mejores prácticas

### Consistencia en el manejo de fechas

1. **Siempre almacenar en UTC**: Todas las fechas en la base de datos deben estar en UTC.
   
2. **Convertir solo para presentación**: Convertir a la zona horaria local únicamente al mostrar fechas al usuario.

3. **Ser explícito con las zonas horarias**: Usar `DateTimeKind.Utc` al crear fechas manualmente.

4. **Documentar la convención**: Asegurarse de que todos los desarrolladores entiendan que las fechas se manejan en UTC.

### Errores comunes a evitar

1. **Mezclar UTC y hora local**: Nunca comparar directamente `DateTime.Now` con fechas almacenadas en UTC.

2. **Olvidar la conversión para presentación**: Las fechas UTC deben convertirse a la zona horaria del usuario antes de mostrarse.

3. **Ignorar los cambios de horario de verano**: Al convertir entre zonas horarias, usar las APIs de .NET que manejan correctamente DST.

4. **Asumir que todas las bases de datos manejan UTC igual**: Diferentes motores de base de datos tienen diferentes comportamientos con las fechas.

### Consideraciones para el frontend

1. **Enviar fechas en formato ISO 8601**: `2025-05-28T15:30:00Z` (la 'Z' indica UTC)

2. **Convertir en el cliente**: Usar JavaScript para convertir a la hora local del usuario:
   ```javascript
   // Fecha UTC desde el servidor
   const utcDate = new Date('2025-05-28T15:30:00Z');
   
   // Convertir a hora local para mostrar
   const localDateString = utcDate.toLocaleString();
   ```

3. **Enviar siempre en UTC al servidor**: Al enviar fechas desde el frontend, convertirlas a UTC antes de enviarlas.

---

Siguiendo estas prácticas, ServiPuntosUy mantendrá un manejo consistente y confiable de fechas, evitando problemas comunes relacionados con zonas horarias y garantizando la precisión en todas las operaciones basadas en tiempo.
