# 🏥 Sistema de Gestión - Backend (.NET / C#)

![C# Logo](https://upload.wikimedia.org/wikipedia/commons/4/4f/Csharp_Logo.png)

Este repositorio contiene la implementación del **backend** del sistema desarrollado para gestionar ingresos, pacientes, empleados, autenticación y el módulo de urgencias.  
El proyecto está construido con **.NET**, utilizando una arquitectura limpia basada en principios **SOLID**, separación de responsabilidades y buenas prácticas de diseño.

---

## 🚀 Tecnologías Utilizadas

- **.NET 8 / .NET 9** (dependiendo del entorno del usuario)
- **C#**
- **MySQL** como base de datos
- **Dapper / MySqlConnector** para acceso a datos (si corresponde en tu proyecto)
- **Dependency Injection** con `Microsoft.Extensions.DependencyInjection`
- **BCrypt** para hashing de contraseñas
- **Reqnroll (BDD)** para pruebas Behavior Driven Development
- **xUnit** para pruebas unitarias
- **NSubstitute** para mocks

---

## 🏛️ Arquitectura

La solución está estructurada según una arquitectura por capas/layers que facilita la extensibilidad y el mantenimiento:
