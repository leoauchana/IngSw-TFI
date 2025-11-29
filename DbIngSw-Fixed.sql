use mydb;

-- Primero: Obras Sociales (no tienen dependencias)
INSERT INTO health_insurance (
    id_health_insurance,
    name,
    member_number
) VALUES
('D0B94B17-0D8F-46AD-B5B8-3212E18D7098', 'OSDE', 451235),
('FA3AB55D-DCD8-41D7-971D-E0B00042DB0E', 'Swiss Medical', 985632),
('C53AEE4C-F6F2-443E-BC2B-C826190D46F7', 'IOMA', 748541);

-- Segundo: Usuarios
INSERT INTO user (idusuario, email, password) VALUES
('F45D8F93-B89E-4CAE-BB0B-A6340C68E48A', 'marcos.medina@clinica.com', '$2a$12$Aan.pCf8qYi.nul/nh2dtuG5lM/HGrFAzHvaRqir4kVAIHuUQzune'), -- marcos123
('23AEB4A4-A6F0-4B04-A5BE-EF29C18B612', 'laura.sanchez@clinica.com', '$2a$12$LXQOcgxN3FeO1BQi3BKL9e9MWt/HGdEmRm0iX8OTijWd5on1nRMHW'), -- laura123
('C68E6147-2F9D-4A3E-B930-AE540B8F75B1', 'carla.enfermera@clinica.com', '$2a$12$AdxDRokI4YNXT0hAkVQh2e4EnrzG2qmkzsOmgXFfxUv9ZRHO7uzAy'), -- carla123
('79988EEC-733C-47FA-8B8A-F3DBDDD6374C', 'roberto.enfermero@clinica.com', '$2a$12$UKCQgw5tpQPT2szCF6DMJOXEEITH4gQ1bC0cHeiNV0q37fhek5Ag.'), -- roberto123
('06451215-EE94-4E8F-ABAA-115A83E50FC3', 'melisa.enfermera@clinica.com', '$2a$12$gj/IBy0milhRQ46rfFDRYuqqHd4XAWnmuQysex5rT5VUS9xHrZNBO'); -- melisa123

-- Tercero: Enfermeras (dependen de usuarios)
INSERT INTO nurse (id_nurse, user_idusuario, nurse_dni, first_name, last_name, phone_number) VALUES
('42A34C14-B615-41D4-98E3-61D122ECB774', 'C68E6147-2F9D-4A3E-B930-AE540B8F75B1', '40236589', 'Carla', 'Fernández', '381450111'),
('DF95377A-900C-4309-973B-2E1186D3CC9E', '79988EEC-733C-47FA-8B8A-F3DBDDD6374C', '38965412', 'Roberto', 'Ponce', '381450222'),
('2D7E2179-254D-4634-9655-56345F27A339', '06451215-EE94-4E8F-ABAA-115A83E50FC3', '41523698', 'Melisa', 'Ríos', '381450333');

-- Cuarto: Doctores (dependen de usuarios)
INSERT INTO doctor (id_doctor, user_idusuario, doctor_dni, license_number, first_name, last_name, phone_number) VALUES
('C76B97BC-5459-48D1-A8B7-B95B0EBC3B28', 'F45D8F93-B89E-4CAE-BB0B-A6340C68E48A', 32654123, 15001, 'Marcos', 'Medina', '381460111'),
('B0D04C8B-74E0-4E51-8849-751604A396DF', '23AEB4A4-A6F0-4B04-A5BE-EF29C18B612', 35412897, 15002, 'Laura', 'Sánchez', '381460222');

-- Agregar columnas email a nurse y doctor
ALTER TABLE nurse ADD COLUMN IF NOT EXISTS email VARCHAR(255);
ALTER TABLE doctor ADD COLUMN IF NOT EXISTS email VARCHAR(255);

-- Actualizar emails desde user
UPDATE nurse n
INNER JOIN user u ON n.user_idusuario = u.idusuario
SET n.email = u.email;

UPDATE doctor d
INNER JOIN user u ON d.user_idusuario = u.idusuario
SET d.email = u.email;

-- Cambiar nombre de columnas DNI
ALTER TABLE doctor CHANGE COLUMN doctor_dni dni INT NOT NULL;
ALTER TABLE nurse CHANGE COLUMN nurse_dni dni INT NOT NULL;

-- Agregar columna CUIL
ALTER TABLE doctor ADD COLUMN IF NOT EXISTS cuil VARCHAR(13) NOT NULL DEFAULT '';
ALTER TABLE nurse ADD COLUMN IF NOT EXISTS cuil VARCHAR(13) NOT NULL DEFAULT '';

-- Actualizar CUIL
UPDATE doctor SET cuil = CONCAT('20-', dni, '-9') WHERE cuil = '';
UPDATE nurse SET cuil = CONCAT('20-', dni, '-8') WHERE cuil = '';

-- Agregar columna email a patient si no existe
ALTER TABLE patient ADD COLUMN IF NOT EXISTS email VARCHAR(100) NOT NULL DEFAULT 'placeholder@example.com';

-- Quinto: Pacientes (dependen de obras sociales)
INSERT INTO patient (
    id_patient,
    health_insurance_id,
    patient_cuil,
    first_name,
    last_name,
    email,
    street_address,
    number_address,
    town_address
) VALUES
('6F2E7853-B201-40F9-B0C9-147E0526B8B4', NULL, '20-12345678-3', 'María', 'García', 'maria.garcia@example.com', 'Av. Siempre Viva', '742', 'Springfield'),
('526A2163-2664-491F-B498-C1374366AC91', NULL, '23-87654321-9', 'Juan', 'Pérez', 'juan.perez@example.com', 'Calle Falsa', '123', 'Capital Federal'),
('26CF78DE-7511-4A94-A466-AF8C18927159', 'D0B94B17-0D8F-46AD-B5B8-3212E18D7098', '27-11223344-6', 'Lucía', 'Martínez', 'lucia.martinez@example.com', 'San Martín', '850', 'Rosario'),
('BD848063-1965-4AB7-8447-8835546734C6', 'FA3AB55D-DCD8-41D7-971D-E0B00042DB0E', '20-33445566-1', 'Carlos', 'Ramírez', 'carlos.ramirez@example.com', 'Belgrano', '220', 'Córdoba'),
('796DA8F5-B35B-482F-8077-43004F9B53D8', NULL, '23-99887766-0', 'Ana', 'López', 'ana.lopez@example.com', 'Mitre', '360', 'Mendoza');

SELECT 'Base de datos configurada correctamente!' AS resultado;


