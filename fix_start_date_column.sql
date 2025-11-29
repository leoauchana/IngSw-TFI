-- Modificar la columna start_date de DATE a DATETIME para incluir la hora del ingreso

ALTER TABLE admission 
MODIFY COLUMN start_date DATETIME NOT NULL;
