# Implementación de mejoras en UserManagmentAPI

## Objetivo

Se revisó la API de usuarios para verificar y mejorar:

- La validación de los datos recibidos.
- La gestión de errores durante las búsquedas.
- El rendimiento del endpoint `GET /api/users`.
- La forma de probar la API mediante OpenAPI.

## Cambios realizados

### Validación de datos

Se agregaron anotaciones de validación a los DTOs de entrada en `Models/UserModels.cs`:

- Campos obligatorios mediante `[Required]`.
- Límites de longitud mediante `[StringLength]`.
- Validación de formato de correo mediante `[EmailAddress]`.
- Contraseñas con una longitud mínima de 8 caracteres.
- Rechazo de valores vacíos o compuestos únicamente por espacios.

Como el proyecto utiliza `[ApiController]`, ASP.NET Core devuelve automáticamente una respuesta `400 Bad Request` con los detalles de validación cuando los datos no cumplen las reglas.

### Gestión de excepciones

El proyecto utiliza el manejo global de excepciones configurado en `Program.cs`:

- `AddProblemDetails()` habilita respuestas con formato RFC 7807.
- `UseExceptionHandler()` evita exponer detalles internos de excepciones no controladas.

Además, `UsersController` captura y registra los errores producidos durante las búsquedas:

- Se registra la excepción con `ILogger`.
- Se devuelve una respuesta `500 Internal Server Error`.
- El mensaje expuesto al cliente no revela información interna.

### Rendimiento de `GET /api/users`

El endpoint ahora acepta paginación:

```text
GET /api/users?page=1&pageSize=50
```

Restricciones:

- `page` debe estar entre 1 y 1.000.000.
- `pageSize` debe estar entre 1 y 100.

La lógica ordena, salta y limita los registros antes de convertirlos a objetos de respuesta. Esto evita transformar todos los usuarios cuando solo se solicita una página.

El `CancellationToken` también se propaga al servicio para poder cancelar el trabajo si el cliente abandona la solicitud.

## Archivos modificados

- `Models/UserModels.cs`
  - Reglas de validación de los DTOs.
- `Controllers/UsersController.cs`
  - Paginación del endpoint.
  - Registro y respuesta de errores de búsqueda.
- `Services/IUserService.cs`
  - Nuevo contrato paginado para obtener usuarios.
- `Services/InMemoryUserService.cs`
  - Implementación de la paginación y optimización del orden de operaciones.

## Pruebas realizadas

Se ejecutó:

```powershell
dotnet build .\UserManagmentAPI\UserManagmentAPI.csproj --no-restore
```

Resultado:

```text
Compilación correcta
0 advertencias
0 errores
```

También se verificó:

```text
GET /api/users?page=1&pageSize=1 -> 200 OK
GET /api/users?pageSize=101 -> 400 Bad Request
```

## Cómo probar la API

Desde PowerShell:

```powershell
cd "C:\Users\Juampi\Desktop\Proyectos .NET\UserManagmentAPI"
dotnet run
```

La API queda disponible normalmente en:

```text
http://localhost:5202
```

### Consultar usuarios

```text
http://localhost:5202/api/users?page=1&pageSize=10
```

### Iniciar sesión

Enviar un `POST` a:

```text
http://localhost:5202/api/auth/login
```

Con el cuerpo:

```json
{
  "username": "it.admin",
  "password": "ChangeMe123!"
}
```

La respuesta contiene un token JWT para las operaciones protegidas de creación, actualización y eliminación.

### OpenAPI

El documento OpenAPI está disponible en:

```text
http://localhost:5202/openapi/v1.json
```

El proyecto utiliza OpenAPI integrado de ASP.NET Core. No se agregó Swagger UI porque el proyecto apunta a .NET 10; el documento JSON puede abrirse directamente o importarse en [Swagger Editor](https://editor.swagger.io/).

## Nota sobre persistencia

Actualmente la API utiliza `InMemoryUserService`, por lo que los usuarios se almacenan en memoria y se pierden al reiniciar la aplicación. El manejo de errores y el contrato del servicio permiten reemplazar posteriormente esta implementación por una base de datos.
