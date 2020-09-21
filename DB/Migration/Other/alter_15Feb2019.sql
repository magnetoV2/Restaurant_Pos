ALTER TABLE c_invoice
ADD COLUMN is_onsync character(1) NOT NULL DEFAULT 'N'::bpchar,
ADD COLUMN is_posterror character(1) NOT NULL DEFAULT 'N'::bpchar;