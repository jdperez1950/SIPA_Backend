-- Script de limpieza de base de datos PostgreSQL
-- ⚠️ ADVERTENCIA: Esto eliminará todos los datos

\c PavisDb

-- Eliminar tablas en orden correcto (respetando claves foráneas)
DROP TABLE IF EXISTS "ProjectResponses" CASCADE;
DROP TABLE IF EXISTS "Questions" CASCADE;
DROP TABLE IF EXISTS "Projects" CASCADE;
DROP TABLE IF EXISTS "Organizations" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;

RAISE NOTICE '✅ Base de datos limpiada exitosamente.';
