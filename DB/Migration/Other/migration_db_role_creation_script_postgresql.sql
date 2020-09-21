-- Role: adempiere

-- DROP ROLE adempiere;

CREATE ROLE adempiere LOGIN
  ENCRYPTED PASSWORD 'md5e39d02ba00f8344d92323e6b8ffe612f'
  SUPERUSER INHERIT CREATEDB CREATEROLE;

-- Database: pos

-- DROP DATABASE pos;

CREATE DATABASE pos
  WITH OWNER = adempiere
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'English_India.1252'
       LC_CTYPE = 'English_India.1252'
       CONNECTION LIMIT = -1;

-- DROP DATABASE pos;

CREATE DATABASE pos
  WITH OWNER = adempiere
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'English_United States.1252'
       LC_CTYPE = 'English_United States.1252'
       CONNECTION LIMIT = -1;