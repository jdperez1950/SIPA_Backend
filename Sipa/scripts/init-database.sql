-- Script inicial para crear la base de datos PostgreSQL
-- Ejecutar antes de las migraciones de Entity Framework

-- Crear base de datos si no existe
SELECT 'CREATE DATABASE PavisDb'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'PavisDb')\gexec

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'PavisDb') THEN
        CREATE DATABASE PavisDb;
        RAISE NOTICE 'Base de datos PavisDb creada exitosamente.';
    ELSE
        RAISE NOTICE 'La base de datos PavisDb ya existe.';
    END IF;
END
$$;

\c PavisDb

-- Verificar si existen tablas
DO $$
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Users') THEN
        RAISE NOTICE 'Las tablas ya existen. Si deseas reiniciar, ejecuta el script de limpieza.';
    ELSE
        RAISE NOTICE 'Base de datos lista para migraciones.';
    END IF;
END
$$;
