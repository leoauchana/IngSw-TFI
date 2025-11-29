# 🏥 Sistema de Gestión de Urgencias - Backend

Backend del sistema de gestión de urgencias desarrollado con .NET y arquitectura en capas.

## 📋 Tabla de Contenidos

- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Instalación](#instalación)
- [Base de Datos](#base-de-datos)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura en capas (Clean Architecture)** con separación de responsabilidades:

```
┌─────────────────────────────────────┐
│   IngSw-Tfi.Api (Presentation)     │  ← Controllers, Middlewares
├─────────────────────────────────────┤
│   IngSw-Tfi.Application (Use Cases)│  ← Services, DTOs, Interfaces
├─────────────────────────────────────┤
│   IngSw-Tfi.Domain (Business Logic)│  ← Entities, Value Objects, Enums
├─────────────────────────────────────┤
│   IngSw-Tfi.Data (Infrastructure)  │  ← Repositories, DAOs, DB Access
├─────────────────────────────────────┤
│   IngSw-Tfi.Transversal (Shared)   │  ← External Services, Utilities
└─────────────────────────────────────┘
```

### Capas del Proyecto

#### 1. **IngSw-Tfi.Api** (Capa de Presentación)
- Controllers: `PatientsController`, `IncomesController`
- Middlewares: Manejo global de excepciones
- Configuración y punto de entrada de la aplicación

#### 2. **IngSw-Tfi.Application** (Capa de Aplicación)
- **Services**: Lógica de negocio
  - `PatientsService`: Gestión de pacientes
  - `IncomesService`: Gestión de ingresos/admisiones
  - `AuthService`: Autenticación
- **DTOs**: Data Transfer Objects
- **Exceptions**: Excepciones personalizadas

#### 3. **IngSw-Tfi.Domain** (Capa de Dominio)
- **Entities**: Modelos del dominio
  - `Patient`, `Doctor`, `Nurse`, `Income`, `Consultation`
- **Value Objects**: CUIL, Frecuencias (cardíaca, respiratoria, etc.)
- **Enums**: `EmergencyLevel`, `IncomeStatus`
- **Repository Interfaces**: Contratos para acceso a datos

#### 4. **IngSw-Tfi.Data** (Capa de Infraestructura)
- **Repositories**: Implementación de acceso a datos
- **DAOs**: Data Access Objects
- **Database**: Gestión de conexiones

#### 5. **IngSw-Tfi.Transversal** (Capa Transversal)
- Servicios externos (APIs de obras sociales)
- Utilidades compartidas

## 🛠️ Tecnologías

- **.NET 8.0** (o superior)
- **MySQL** - Base de datos relacional
- **Entity Framework Core** (opcional/ADO.NET)
- **C# 12**
- **REST API**

## 📁 Estructura del Proyecto

```
IngSw-TFI/
│
├── IngSw-Tfi.sln                    # Solución principal
│
├── IngSw-Tfi.Api/                   # 🌐 API Web
│   ├── Controllers/
│   │   ├── PatientsController.cs
│   │   └── IncomesController.cs
│   ├── Middlewares/
│   │   └── ExceptionMiddleware.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── IngSw-Tfi.Application/           # 💼 Lógica de Aplicación
│   ├── Services/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Exceptions/
│
├── IngSw-Tfi.Domain/                # 🎯 Lógica de Negocio
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Enums/
│   └── Repository/
│
├── IngSw-Tfi.Data/                  # 🗄️ Acceso a Datos
│   ├── Repositories/
│   ├── DAOs/
│   └── Database/
│
├── IngSw-Tfi.Transversal/           # 🔧 Servicios Compartidos
│   └── Services/
│
├── DbScriptIngSw.sql                # Script de creación de DB
├── DbIngSw.sql                      # Datos iniciales
├── setup-database.sh                # Script de instalación
└── DATABASE_SETUP.md                # Documentación de DB
```

## 🚀 Instalación

### Prerrequisitos

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- IDE: Visual Studio, Rider o VS Code

### Pasos

1. **Clonar el repositorio**
```bash
cd ~/Desktop/Proyecto/Modulo_Urgencias/IngSw-TFI
```

2. **Restaurar dependencias**
```bash
dotnet restore
```

3. **Configurar la base de datos**

Ver [DATABASE_SETUP.md](./DATABASE_SETUP.md) para instrucciones detalladas.

**Método rápido:**
```bash
./setup-database.sh
```

4. **Configurar conexión a la base de datos**

Edita `IngSw-Tfi.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mydb;User=root;Password=tu_password;"
  }
}
```

5. **Ejecutar la aplicación**
```bash
cd IngSw-Tfi.Api
dotnet run
```

La API estará disponible en: `https://localhost:5001` o `http://localhost:5000`

## 🗄️ Base de Datos

### Configuración Rápida

```bash
# Opción 1: Script automatizado
./setup-database.sh

# Opción 2: Manual
mysql -u root -p < DbScriptIngSw.sql
mysql -u root -p < DbIngSw.sql
```

### Esquema Principal

- **user** - Usuarios del sistema (doctores, enfermeras)
- **doctor** - Información de médicos
- **nurse** - Información de enfermeras
- **patient** - Pacientes
- **health_insurance** - Obras sociales
- **admission** - Ingresos a urgencias
- **consultation** - Consultas médicas

Ver [DATABASE_SETUP.md](./DATABASE_SETUP.md) para más detalles.

## 🌐 API Endpoints

### Pacientes

```http
GET    /api/patients              # Listar todos los pacientes
GET    /api/patients/{id}         # Obtener paciente por ID
POST   /api/patients              # Crear nuevo paciente
PUT    /api/patients/{id}         # Actualizar paciente
DELETE /api/patients/{id}         # Eliminar paciente
```

### Ingresos (Admisiones)

```http
GET    /api/incomes               # Listar todos los ingresos
GET    /api/incomes/{id}          # Obtener ingreso por ID
POST   /api/incomes               # Crear nuevo ingreso
PUT    /api/incomes/{id}          # Actualizar ingreso
DELETE /api/incomes/{id}          # Eliminar ingreso
```

### Documentación Completa

📚 **Postman Collection**: [Ver en Postman](https://devchana.postman.co/workspace/IngSwTfi~734ad8a4-647b-42f3-bec0-6276a5b60097/collection/43184413-9b6032ff-0cee-439c-9237-bf73eafa06c7)

## 🧪 Testing

```bash
# Ejecutar tests (cuando estén implementados)
dotnet test
```

## 🔐 Usuarios de Prueba

### Doctores:
- **Email**: marcos.medina@clinica.com | **Password**: marcos123
- **Email**: laura.sanchez@clinica.com | **Password**: laura123

### Enfermeras:
- **Email**: carla.enfermera@clinica.com | **Password**: carla123
- **Email**: roberto.enfermero@clinica.com | **Password**: roberto123
- **Email**: melisa.enfermera@clinica.com | **Password**: melisa123

## 📝 Configuración de Desarrollo

### Visual Studio / Rider

1. Abrir `IngSw-Tfi.sln`
2. Configurar `IngSw-Tfi.Api` como proyecto de inicio
3. Presionar F5 para ejecutar

### VS Code

1. Instalar extensión de C#
2. Abrir la carpeta del proyecto
3. Ejecutar: `dotnet run --project IngSw-Tfi.Api`

## 🐛 Solución de Problemas

### Error de conexión a MySQL
```bash
# Verificar que MySQL esté corriendo
brew services list | grep mysql

# Iniciar MySQL si no está corriendo
brew services start mysql
```

### Error de compilación
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

### Puertos en uso
Si el puerto 5000/5001 está ocupado, modifica `Properties/launchSettings.json`

## 📚 Recursos Adicionales

- [Documentación de .NET](https://docs.microsoft.com/dotnet/)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 👥 Equipo

Proyecto desarrollado para la materia de Ingeniería de Software - TFI

---

**¿Necesitas ayuda?** Consulta [DATABASE_SETUP.md](./DATABASE_SETUP.md) o revisa la [Postman Collection](https://devchana.postman.co/workspace/IngSwTfi~734ad8a4-647b-42f3-bec0-6276a5b60097/collection/43184413-9b6032ff-0cee-439c-9237-bf73eafa06c7)


