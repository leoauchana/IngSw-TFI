-- Agregar columnas phone y birth_date a la tabla patient

ALTER TABLE patient 
ADD COLUMN IF NOT EXISTS phone VARCHAR(20) NULL;

ALTER TABLE patient 
ADD COLUMN IF NOT EXISTS birth_date DATE NULL;
