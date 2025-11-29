#!/bin/bash

# Script para configurar la base de datos del proyecto
# Módulo de Urgencias - Ingeniería de Software

echo "🏥 Setup Base de Datos - Módulo de Urgencias"
echo "=============================================="
echo ""

# Colores para la terminal
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Verificar si MySQL está instalado
if ! command -v mysql &> /dev/null; then
    echo -e "${RED}❌ Error: MySQL no está instalado${NC}"
    echo "Por favor instala MySQL con: brew install mysql"
    exit 1
fi

echo -e "${GREEN}✅ MySQL encontrado${NC}"

# Verificar si el servicio está corriendo
if ! brew services list | grep mysql | grep started &> /dev/null; then
    echo -e "${YELLOW}⚠️  MySQL no está corriendo. Iniciando servicio...${NC}"
    brew services start mysql
    sleep 3
fi

echo -e "${GREEN}✅ Servicio MySQL corriendo${NC}"
echo ""

# Pedir credenciales
read -p "Usuario MySQL (default: root): " MYSQL_USER
MYSQL_USER=${MYSQL_USER:-root}

echo "Contraseña MySQL (presiona Enter si no tienes contraseña):"
read -s MYSQL_PASS

echo ""
echo "=============================================="
echo "Opciones de instalación:"
echo "1) Instalación completa (DB + datos de prueba)"
echo "2) Solo estructura de la DB (sin datos)"
echo "3) Solo cargar datos de prueba (DB ya existe)"
echo "=============================================="
read -p "Selecciona una opción (1-3): " OPTION

case $OPTION in
    1)
        echo ""
        echo -e "${YELLOW}📦 Creando base de datos y cargando datos...${NC}"
        
        if [ -z "$MYSQL_PASS" ]; then
            mysql -u "$MYSQL_USER" < DbScriptIngSw.sql 2>&1
            RESULT1=$?
            mysql -u "$MYSQL_USER" < DbIngSw.sql 2>&1
            RESULT2=$?
        else
            mysql -u "$MYSQL_USER" -p"$MYSQL_PASS" < DbScriptIngSw.sql 2>&1
            RESULT1=$?
            mysql -u "$MYSQL_USER" -p"$MYSQL_PASS" < DbIngSw.sql 2>&1
            RESULT2=$?
        fi
        
        if [ $RESULT1 -eq 0 ] && [ $RESULT2 -eq 0 ]; then
            echo -e "${GREEN}✅ Base de datos creada y datos cargados exitosamente${NC}"
        else
            echo -e "${RED}❌ Error al crear la base de datos${NC}"
            exit 1
        fi
        ;;
    2)
        echo ""
        echo -e "${YELLOW}📦 Creando estructura de la base de datos...${NC}"
        
        if [ -z "$MYSQL_PASS" ]; then
            mysql -u "$MYSQL_USER" < DbScriptIngSw.sql 2>&1
            RESULT=$?
        else
            mysql -u "$MYSQL_USER" -p"$MYSQL_PASS" < DbScriptIngSw.sql 2>&1
            RESULT=$?
        fi
        
        if [ $RESULT -eq 0 ]; then
            echo -e "${GREEN}✅ Estructura de la base de datos creada${NC}"
        else
            echo -e "${RED}❌ Error al crear la estructura${NC}"
            exit 1
        fi
        ;;
    3)
        echo ""
        echo -e "${YELLOW}📦 Cargando datos de prueba...${NC}"
        
        if [ -z "$MYSQL_PASS" ]; then
            mysql -u "$MYSQL_USER" < DbIngSw.sql 2>&1
            RESULT=$?
        else
            mysql -u "$MYSQL_USER" -p"$MYSQL_PASS" < DbIngSw.sql 2>&1
            RESULT=$?
        fi
        
        if [ $RESULT -eq 0 ]; then
            echo -e "${GREEN}✅ Datos de prueba cargados${NC}"
        else
            echo -e "${RED}❌ Error al cargar datos${NC}"
            exit 1
        fi
        ;;
    *)
        echo -e "${RED}❌ Opción inválida${NC}"
        exit 1
        ;;
esac

echo ""
echo "=============================================="
echo -e "${GREEN}🎉 ¡Setup completado!${NC}"
echo "=============================================="
echo ""
echo "📊 Para verificar la instalación:"
echo "   mysql -u $MYSQL_USER -p"
echo "   USE mydb;"
echo "   SHOW TABLES;"
echo ""
echo "👥 Usuarios de prueba disponibles:"
echo "   Doctor: marcos.medina@clinica.com / marcos123"
echo "   Enfermera: carla.enfermera@clinica.com / carla123"
echo ""
echo "📖 Ver DATABASE_SETUP.md para más información"
echo ""


