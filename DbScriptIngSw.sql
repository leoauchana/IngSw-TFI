-- SCRIPT AJUSTADO para MySQL / DBeaver
-- Asegura que PKs sean NOT NULL y únicas, y que FKs coincidan exactamente en tipo/nullable.

SET @OLD_UNIQUE_CHECKS = @@UNIQUE_CHECKS;
SET UNIQUE_CHECKS = 0;

SET @OLD_FOREIGN_KEY_CHECKS = @@FOREIGN_KEY_CHECKS;
SET FOREIGN_KEY_CHECKS = 0;

SET @OLD_SQL_MODE = @@SQL_MODE;
SET SQL_MODE = 'STRICT_TRANS_TABLES,NO_ZERO_DATE,NO_ENGINE_SUBSTITUTION';

DROP SCHEMA IF EXISTS `mydb`;
CREATE SCHEMA `mydb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `mydb`;

-- -----------------------------------------------------
-- Table user
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `user` (
  `idusuario` VARCHAR(45) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `password` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`idusuario`),
  UNIQUE KEY `uq_user_idusuario` (`idusuario`)
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table health_insurance
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `health_insurance` (
  `id_health_insurance` VARCHAR(45) NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `member_number` INT NOT NULL,
  PRIMARY KEY (`id_health_insurance`),
  UNIQUE KEY `uq_health_insurance_id` (`id_health_insurance`)
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table patient
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `patient` (
  `id_patient` VARCHAR(45) NOT NULL,
  `health_insurance_id` VARCHAR(45) NULL,
  `patient_cuil` VARCHAR(13) NOT NULL,
  `first_name` VARCHAR(100) NOT NULL,
  `last_name` VARCHAR(100) NOT NULL,
  `street_address` VARCHAR(100) NOT NULL,
  `number_address` VARCHAR(45) NOT NULL,
  `town_address` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id_patient`),
  UNIQUE KEY `uq_patient_id` (`id_patient`),
  INDEX `idx_patient_insurance` (`health_insurance_id`),
  CONSTRAINT `fk_patient_insurance`
      FOREIGN KEY (`health_insurance_id`)
      REFERENCES `health_insurance` (`id_health_insurance`)
      ON DELETE SET NULL
      ON UPDATE CASCADE
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table doctor
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `doctor` (
  `id_doctor` VARCHAR(45) NOT NULL,
  `user_idusuario` VARCHAR(45) NOT NULL,
  `doctor_dni` INT NOT NULL,
  `license_number` INT NOT NULL,
  `first_name` VARCHAR(100) NOT NULL,
  `last_name` VARCHAR(100) NOT NULL,
  `phone_number` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`id_doctor`),
  UNIQUE KEY `uq_doctor_id` (`id_doctor`),
  INDEX `idx_doctor_user` (`user_idusuario`),
  CONSTRAINT `fk_doctor_user`
      FOREIGN KEY (`user_idusuario`)
      REFERENCES `user` (`idusuario`)
      ON DELETE RESTRICT
      ON UPDATE CASCADE
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table nurse
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `nurse` (
  `id_nurse` VARCHAR(45) NOT NULL,
  `user_idusuario` VARCHAR(45) NOT NULL,
  `nurse_dni` VARCHAR(45) NOT NULL,
  `first_name` VARCHAR(100) NOT NULL,
  `last_name` VARCHAR(100) NOT NULL,
  `phone_number` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`id_nurse`),
  UNIQUE KEY `uq_nurse_id` (`id_nurse`),
  INDEX `idx_nurse_user` (`user_idusuario`),
  CONSTRAINT `fk_nurse_user`
      FOREIGN KEY (`user_idusuario`)
      REFERENCES `user` (`idusuario`)
      ON DELETE RESTRICT
      ON UPDATE CASCADE
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table admission
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `admission` (
  `id_admission` VARCHAR(45) NOT NULL,
  `nurse_id_nurse` VARCHAR(45) NOT NULL,
  `patient_id_patient` VARCHAR(45) NOT NULL,
  `status` INT NOT NULL,
  `level` INT NOT NULL,
  `start_date` DATE NOT NULL,
  `end_date_time` DATETIME(4) NOT NULL,
  `temperature` FLOAT NOT NULL,
  `heart_rate` FLOAT NOT NULL,
  `respiratory_rate` FLOAT NOT NULL,
  `report` VARCHAR(255) NOT NULL,
  `blood_pressure` FLOAT NOT NULL,
  PRIMARY KEY (`id_admission`),
  UNIQUE KEY `uq_admission_id` (`id_admission`),
  INDEX `idx_admission_nurse` (`nurse_id_nurse`),
  INDEX `idx_admission_patient` (`patient_id_patient`),
  CONSTRAINT `fk_admission_nurse`
      FOREIGN KEY (`nurse_id_nurse`)
      REFERENCES `nurse` (`id_nurse`)
      ON DELETE RESTRICT
      ON UPDATE CASCADE,
  CONSTRAINT `fk_admission_patient`
      FOREIGN KEY (`patient_id_patient`)
      REFERENCES `patient` (`id_patient`)
      ON DELETE RESTRICT
      ON UPDATE CASCADE
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table consultation
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `consultation` (
  `id_consultation` VARCHAR(45) NOT NULL,
  `doctor_id_doctor` VARCHAR(45) NOT NULL,
  `admission_id_admission` VARCHAR(45) NOT NULL,
  `start_date_time` DATETIME(4) NOT NULL,
  `diagnosis` VARCHAR(255) NOT NULL,
  `procedure_done` VARCHAR(255) NOT NULL,
  `end_date_time` DATETIME(4) NOT NULL,
  `report` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_consultation`),
  UNIQUE KEY `uq_consultation_id` (`id_consultation`),
  INDEX `idx_consultation_doctor` (`doctor_id_doctor`),
  INDEX `idx_consultation_admission` (`admission_id_admission`),
  CONSTRAINT `fk_consultation_doctor`
      FOREIGN KEY (`doctor_id_doctor`)
      REFERENCES `doctor` (`id_doctor`)
      ON DELETE RESTRICT
      ON UPDATE CASCADE,
  CONSTRAINT `fk_consultation_admission`
      FOREIGN KEY (`admission_id_admission`)
      REFERENCES `admission` (`id_admission`)
      ON DELETE RESTRICT
      ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Restore settings
SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
