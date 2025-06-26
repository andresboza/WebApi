\*\*# Proyecto WebApi

Este repositorio contiene una API RESTful desarrollada en .NET Core 8 con C# y Oracle, se usaron patrones de arquitectura en capas (Controller, Service, Repository), CQRS e inyección de dependencias.

---

## 📦 Estructura

- **WebApi/**: proyecto principal con controladores, servicios, repositorios y middleware.
- **WebApi.Tests/**: pruebas unitarias (xUnit + Moq + FluentAssertions).
- **WebApi.IntegrationTests/**: pruebas de integración (WebApplicationFactory).
- **Database/**: scripts SQL para creación de tablas y esquemas en Oracle.
- **swagger.json**: definición OpenAPI exportada.

---

## 🚀 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Oracle XE Container ya configurado (siguiente sección)

---

## 🐳 Base de datos Oracle en Docker

1. En la carpeta Database/ encontrarás el script Create.sql con el DDL para crear la tabla POSITIONS.
2. Descargar y ejecutar Oracle XE:
   bash
   docker pull gvenzl/oracle-xe
   docker run -d --name oracle-xe -p 1521:1521 -p 5500:5500 gvenzl/oracle-xe
   
3. Confirmar que el contenedor está arriba:
   bash
   docker ps
   
4. Conexión en appsettings.json:
   "ConnectionStrings": {
     "OracleDb": "User Id=system;Password=<tu contraseña>;Data Source=localhost:1521/XE"
   }
   <tu contraseña> responde a la contraseña de tu cliente oracle, para el caso del proyecto se estableció 123456
---

## ⚙️ Configuración y ejecución

1. Clonar el repositorio:
   bash
   git clone https://github.com/andresboza/WebApi.git
   cd WebApi
   
2. Restaurar paquetes y compilar:
   bash
   dotnet restore
   dotnet build
   
3. Ejecutar la API:
   bash
   dotnet run --project WebApi/WebApi.csproj
   
4. Acceder a Swagger UI: https://localhost:7023/swagger/index.html

---

## 📑 Definición OpenAPI

El archivo swagger.json en la raíz contiene la definición OpenAPI v3. Este archivo debe importarlo en Postman o Insomnia.

---

## 🔄 Pruebas

### Unitarias

bash
cd WebApi.Tests
dotnet test

### Integración

bash
cd WebApi.IntegrationTests
dotnet test

---

## 📖 Endpoints principales

Estos son los endpoints expuestos por el controlador PositionsController:

| Acción                        | Método HTTP | Ruta                       | Descripción                                                      |
| ----------------------------- | ----------- | -------------------------- | ---------------------------------------------------------------- |
| Listar todas las posiciones   | GET         | /api/Positions/GetAll      | Obtiene todas las posiciones.                                    |
| Obtener posición por ID       | GET         | /api/Positions/Get/{id}    | Recupera la posición con el id especificado.                     |
| Crear nueva posición          | POST        | /api/Positions/Create      | Crea una nueva posición.                                         |
| Actualizar posición existente | PUT         | /api/Positions/Update/{id} | Actualiza la posición con el id dado.                            |
| Eliminar posición             | DELETE      | /api/Positions/Delete/{id} | Elimina la posición con el id dado.                              |
| Filtrar y paginar posiciones  | GET         | /api/Positions/Filter      | Aplica filtros (status, location) y paginación (page, pageSize). |

Cada petición requiere la cabecera de autenticación (API Key) para este caso:
X-Api-Key: 123456SECRET
Si desea usar otra, puede cambiarla en appsettings.json en la sección
"ApiKeySettings": {
  "Key": "123456SECRET"
}