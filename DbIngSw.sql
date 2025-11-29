use mydb;
INSERT INTO user (idusuario, email, password) VALUES
('F45D8F93-B89E-4CAE-BB0B-A6340C68E48A', 'marcos.medina@clinica.com', '$2a$12$Aan.pCf8qYi.nul/nh2dtuG5lM/HGrFAzHvaRqir4kVAIHuUQzune'), -- marcos123
('23AEB4A4-A6F0-4B04-A5BE-EF29C18B612', 'laura.sanchez@clinica.com', '$2a$12$LXQOcgxN3FeO1BQi3BKL9e9MWt/HGdEmRm0iX8OTijWd5on1nRMHW'), -- laura123
('C68E6147-2F9D-4A3E-B930-AE540B8F75B1', 'carla.enfermera@clinica.com', '$2a$12$AdxDRokI4YNXT0hAkVQh2e4EnrzG2qmkzsOmgXFfxUv9ZRHO7uzAy'), -- carla123
('79988EEC-733C-47FA-8B8A-F3DBDDD6374C', 'roberto.enfermero@clinica.com', '$2a$12$UKCQgw5tpQPT2szCF6DMJOXEEITH4gQ1bC0cHeiNV0q37fhek5Ag.'), -- roberto123
('06451215-EE94-4E8F-ABAA-115A83E50FC3', 'melisa.enfermera@clinica.com', '$2a$12$gj/IBy0milhRQ46rfFDRYuqqHd4XAWnmuQysex5rT5VUS9xHrZNBO'); -- melisa123

INSERT INTO nurse (id_nurse, user_idusuario, nurse_dni, first_name, last_name, phone_number) VALUES
('42A34C14-B615-41D4-98E3-61D122ECB774', 'C68E6147-2F9D-4A3E-B930-AE540B8F75B1', '40236589', 'Carla', 'Fernández', '381450111'),
('DF95377A-900C-4309-973B-2E1186D3CC9E', '79988EEC-733C-47FA-8B8A-F3DBDDD6374C', '38965412', 'Roberto', 'Ponce', '381450222'),
('2D7E2179-254D-4634-9655-56345F27A339', '06451215-EE94-4E8F-ABAA-115A83E50FC3', '41523698', 'Melisa', 'Ríos', '381450333');

INSERT INTO doctor (id_doctor, user_idusuario, doctor_dni, license_number, first_name, last_name, phone_number) VALUES
('C76B97BC-5459-48D1-A8B7-B95B0EBC3B28', 'F45D8F93-B89E-4CAE-BB0B-A6340C68E48A', 32654123, 15001, 'Marcos', 'Medina', '381460111'),
('B0D04C8B-74E0-4E51-8849-751604A396DF', '23AEB4A4-A6F0-4B04-A5BE-EF29C18B612', 35412897, 15002, 'Laura', 'Sánchez', '381460222');


ALTER TABLE nurse ADD COLUMN email VARCHAR(255);
ALTER TABLE doctor ADD COLUMN email VARCHAR(255);

UPDATE nurse n
INNER JOIN user u ON n.user_idusuario = u.idusuario
SET n.email = u.email;

UPDATE doctor d
INNER JOIN user u ON d.user_idusuario = u.idusuario
SET d.email = u.email;

ALTER TABLE doctor
CHANGE COLUMN doctor_dni dni INT NOT NULL;

ALTER TABLE nurse
CHANGE COLUMN nurse_dni dni INT NOT NULL;

ALTER TABLE doctor 
ADD COLUMN cuil VARCHAR(13) NOT NULL;

ALTER TABLE nurse 
ADD COLUMN cuil VARCHAR(13) NOT NULL;

ALTER TABLE doctor 
ADD COLUMN licence_number VARCHAR(20) NOT NULL;

UPDATE doctor 
SET cuil = CONCAT('20-', dni, '-9');

UPDATE nurse 
set cuil = CONCAT('20-', dni, '-8');

ALTER TABLE admission
MODIFY COLUMN nurse_id_nurse VARCHAR(45) NULL;

ALTER TABLE admission
MODIFY COLUMN patient_id_patient VARCHAR(45) NULL;

ALTER TABLE admission
CHANGE COLUMN blood_pressure diastolic_rate FLOAT not NULL;

ALTER TABLE admission 
ADD COLUMN systolic_rate float NOT NULL;

SHOW CREATE TABLE health_insurance;


INSERT INTO patient (
    id_patient,
    health_insurance_id,
    patient_cuil,
    first_name,
    last_name,
    street_address,
    number_address,
    town_address
) VALUES
('6F2E7853-B201-40F9-B0C9-147E0526B8B4', NULL, '20-12345678-3', 'María', 'García', 'Av. Siempre Viva', '742', 'Springfield'),
('526A2163-2664-491F-B498-C1374366AC91', NULL, '23-87654321-9', 'Juan', 'Pérez', 'Calle Falsa', '123', 'Capital Federal'),
('26CF78DE-7511-4A94-A466-AF8C18927159', 'D0B94B17-0D8F-46AD-B5B8-3212E18D7098', '27-11223344-6', 'Lucía', 'Martínez', 'San Martín', '850', 'Rosario'),
('BD848063-1965-4AB7-8447-8835546734C6', 'FA3AB55D-DCD8-41D7-971D-E0B00042DB0E', '20-33445566-1', 'Carlos', 'Ramírez', 'Belgrano', '220', 'Córdoba'),
('796DA8F5-B35B-482F-8077-43004F9B53D8', NULL, '23-99887766-0', 'Ana', 'López', 'Mitre', '360', 'Mendoza');


INSERT INTO health_insurance (
    id_health_insurance,
    name,
    member_number
) VALUES
('D0B94B17-0D8F-46AD-B5B8-3212E18D7098', 'OSDE', 451235),
('FA3AB55D-DCD8-41D7-971D-E0B00042DB0E', 'Swiss Medical', 985632),
('C53AEE4C-F6F2-443E-BC2B-C826190D46F7', 'IOMA', 748541);


ALTER TABLE patient
MODIFY id_patient CHAR(36) NOT NULL;

SET FOREIGN_KEY_CHECKS = 0;

ALTER TABLE admission DROP FOREIGN KEY fk_admission_patient;

ALTER TABLE patient MODIFY id_patient CHAR(36) NOT NULL;

ALTER TABLE admission
ADD CONSTRAINT fk_admission_patient
FOREIGN KEY (patient_id_patient) REFERENCES patient(id_patient)
ON DELETE RESTRICT
ON UPDATE CASCADE;

SET FOREIGN_KEY_CHECKS = 1;

ALTER TABLE patient
ADD COLUMN email VARCHAR(100) NOT NULL AFTER last_name;

UPDATE patient
SET email = 'maria.garcia@example.com'
WHERE id_patient = '6F2E7853-B201-40F9-B0C9-147E0526B8B4';

UPDATE patient
SET email = 'juan.perez@example.com'
WHERE id_patient = '526A2163-2664-491F-B498-C1374366AC91';

UPDATE patient
SET email = 'lucia.martinez@example.com'
WHERE id_patient = '26CF78DE-7511-4A94-A466-AF8C18927159';

UPDATE patient
SET email = 'carlos.ramirez@example.com'
WHERE id_patient = 'BD848063-1965-4AB7-8447-8835546734C6';

UPDATE patient
SET email = 'ana.lopez@example.com'
WHERE id_patient = '796DA8F5-B35B-482F-8077-43004F9B53D8';

use mydb;
show create table consultation;

SET FOREIGN_KEY_CHECKS = 0;
ALTER TABLE admission DROP FOREIGN KEY fk_admission_patient;
ALTER TABLE patient DROP FOREIGN KEY fk_patient_insurance;
ALTER TABLE consultation DROP FOREIGN KEY fk_consultation_admission;


ALTER TABLE patient MODIFY id_patient CHAR(36) NOT NULL;
alter table mydb.health_insurance modify id_health_insurance CHAR(36) not null;
alter table mydb.consultation modify id_consultation CHAR(36) not null;

ALTER TABLE admission
ADD CONSTRAINT fk_admission_patient
FOREIGN KEY (patient_id_patient) REFERENCES patient(id_patient)
ON DELETE RESTRICT
ON UPDATE CASCADE;

ALTER TABLE consultation
ADD CONSTRAINT fk_consultation_admission
FOREIGN KEY (admission_id_admission) REFERENCES admission(id_admission)
ON DELETE RESTRICT
ON UPDATE CASCADE;


ALTER TABLE patient
ADD CONSTRAINT fk_patient_insurance
FOREIGN KEY (health_insurance_id) REFERENCES health_insurance(id_health_insurance)
ON DELETE RESTRICT
ON UPDATE CASCADE;


SET FOREIGN_KEY_CHECKS = 1;

-- CREATE TABLE `patient` (
--   `id_patient` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `health_insurance_id` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
--   `patient_cuil` varchar(13) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `first_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `last_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `email` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `street_address` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `number_address` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
--   `town_address` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
--   PRIMARY KEY (`id_patient`),
--   UNIQUE KEY `uq_patient_id` (`id_patient`),
--   KEY `idx_patient_insurance` (`health_insurance_id`),
--   CONSTRAINT `fk_patient_insurance` FOREIGN KEY (`health_insurance_id`) REFERENCES `health_insurance` (`id_health_insurance`) ON DELETE SET NULL ON UPDATE CASCADE
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci

CREATE TABLE `consultation` (
  `id_consultation` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `doctor_id_doctor` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `admission_id_admission` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `start_date_time` datetime(4) NOT NULL,
  `diagnosis` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `procedure_done` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `end_date_time` datetime(4) NOT NULL,
  `report` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_consultation`),
  UNIQUE KEY `uq_consultation_id` (`id_consultation`),
  KEY `idx_consultation_doctor` (`doctor_id_doctor`),
  KEY `idx_consultation_admission` (`admission_id_admission`),
  CONSTRAINT `fk_consultation_admission` FOREIGN KEY (`admission_id_admission`) REFERENCES `admission` (`id_admission`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_consultation_doctor` FOREIGN KEY (`doctor_id_doctor`) REFERENCES `doctor` (`id_doctor`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci