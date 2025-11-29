# 🗄️ Guía para Levantar la Base de Datos

## ✅ Estado Actual
- ✅ MySQL instalado en `/opt/homebrew/bin/mysql`
- ✅ Servicio MySQL corriendo

## 📝 Pasos para Crear la Base de Datos

### Paso 1: Conectarse a MySQL

Abre una terminal y conéctate a MySQL:

```bash
mysql -u root -p
```

Si no tienes contraseña configurada:

```bash
mysql -u root
```

### Paso 2: Crear la Base de Datos desde Cero

Una vez dentro de MySQL, ejecuta el script principal:

```bash
# Desde la terminal de tu sistema (NO desde MySQL)
cd ~/Desktop/Proyecto/Modulo_Urgencias/IngSw-TFI
mysql -u root -p < DbScriptIngSw.sql
```

Esto creará:
- ✅ Schema `mydb`
- ✅ Todas las tablas (user, patient, doctor, nurse, admission, consultation, health_insurance)
- ✅ Relaciones y constraints

### Paso 3: Cargar Datos Iniciales (Opcional)

Si quieres cargar los datos de prueba (usuarios, enfermeras, doctores, pacientes):

```bash
mysql -u root -p < DbIngSw.sql
```

Este script incluye:
- 👤 5 usuarios (doctores y enfermeras)
- 👨‍⚕️ 2 doctores
- 👩‍⚕️ 3 enfermeras
- 🏥 3 obras sociales
- 🧑‍🦱 5 pacientes

### Paso 4: Verificar que Todo Funciona

Conéctate a MySQL y verifica:

```bash
mysql -u root -p
```

Dentro de MySQL:

```sql
USE mydb;
SHOW TABLES;

-- Ver usuarios
SELECT * FROM user;

-- Ver doctores
SELECT * FROM doctor;

-- Ver enfermeras
SELECT * FROM nurse;

-- Ver pacientes
SELECT * FROM patient;
```

## 🔑 Usuarios de Prueba

### Doctores:
1. **Marcos Medina**
   - Email: `marcos.medina@clinica.com`
   - Password: `marcos123`
   - DNI: 32654123
   - Licencia: 15001

2. **Laura Sánchez**
   - Email: `laura.sanchez@clinica.com`
   - Password: `laura123`
   - DNI: 35412897
   - Licencia: 15002

### Enfermeras:
1. **Carla Fernández**
   - Email: `carla.enfermera@clinica.com`
   - Password: `carla123`
   - DNI: 40236589

2. **Roberto Ponce**
   - Email: `roberto.enfermero@clinica.com`
   - Password: `roberto123`
   - DNI: 38965412

3. **Melisa Ríos**
   - Email: `melisa.enfermera@clinica.com`
   - Password: `melisa123`
   - DNI: 41523698

## 🔧 Configuración del Backend

Asegúrate de configurar la conexión en tu `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mydb;User=root;Password=tu_password;"
  }
}
```

## 📊 Estructura de la Base de Datos

### Tablas Principales:
- `user` - Usuarios del sistema
- `doctor` - Información de médicos
- `nurse` - Información de enfermeras
- `patient` - Pacientes
- `health_insurance` - Obras sociales
- `admission` - Ingresos/Admisiones a urgencias
- `consultation` - Consultas médicas

### Relaciones:
- Doctor → User (1:1)
- Nurse → User (1:1)
- Patient → Health Insurance (N:1, opcional)
- Admission → Patient (N:1)
- Admission → Nurse (N:1)
- Consultation → Doctor (N:1)
- Consultation → Admission (N:1)

## 🚨 Problemas Comunes

### Error: "Access denied for user 'root'@'localhost'"
```bash
# Resetear contraseña de MySQL
mysql.server stop
mysqld_safe --skip-grant-tables &
mysql -u root
# Dentro de MySQL:
FLUSH PRIVILEGES;
ALTER USER 'root'@'localhost' IDENTIFIED BY 'nueva_password';
```

### Error: "Can't connect to local MySQL server"
```bash
# Iniciar el servicio MySQL
brew services start mysql
```

### Error: "Database already exists"
```bash
# Eliminar y recrear
mysql -u root -p
DROP DATABASE mydb;
# Luego ejecutar el script nuevamente
```

## 🔗 Enlaces Útiles

- **Postman Collection**: [https://devchana.postman.co/workspace/IngSwTfi~734ad8a4-647b-42f3-bec0-6276a5b60097/collection/43184413-9b6032ff-0cee-439c-9237-bf73eafa06c7](https://devchana.postman.co/workspace/IngSwTfi~734ad8a4-647b-42f3-bec0-6276a5b60097/collection/43184413-9b6032ff-0cee-439c-9237-bf73eafa06c7)


