DELETE FROM ad_client;
DELETE FROM ad_org;
DELETE FROM ad_role;
DELETE FROM ad_role_orgaccess;
DELETE FROM ad_sequence;
DELETE FROM ad_sys_config;
DELETE FROM ad_user_pos;
DELETE FROM c_bpartner;
DELETE FROM c_invoice;
DELETE FROM c_invoice_number_details;
DELETE FROM c_invoiceline;
DELETE FROM c_invoicepaymentdetails;
DELETE FROM ee_msrmanager;
DELETE FROM m_product;
DELETE FROM m_product_category;
DELETE FROM m_product_price;
DELETE FROM m_product_uom;
DELETE FROM m_warehouse;
DELETE FROM retail_app_setting;



INSERT INTO retail_app_setting(
            id, servername, serverport, api_url, server_local_name, server_local_port, 
            server_local_userid, server_local_password, server_local_dbname, 
            display_language, on_printer)
    VALUES (1, 'demo.empower-erp.com', '4646', '/POSZearoWebService/POSZearoResource/mobileApi', 'localhost','5432', 
            'adempiere', 'adempiere', 'pos', 
            'E', '');

-- ----------------------------
-- Table structure for ad_sequence
-- ----------------------------
DROP TABLE IF EXISTS public.ad_sequence;
CREATE TABLE public.ad_sequence (
  ad_sequence_id numeric(10) NOT NULL,
  isactive char(1) DEFAULT 'Y'::bpchar,
  created timestamp(6) NOT NULL DEFAULT now(),
  createdby numeric(10) NOT NULL,
  updated timestamp(6) NOT NULL DEFAULT now(),
  updatedby numeric(10) NOT NULL,
  name varchar(60) NOT NULL,
  description varchar(255),
  vformat varchar(40),
  isautosequence char(1) NOT NULL DEFAULT 'Y'::bpchar,
  incrementno numeric(10) NOT NULL,
  startno numeric(10) NOT NULL,
  currentnext numeric(10) NOT NULL,
  currentnextsys numeric(10) NOT NULL,
  isaudited char(1) DEFAULT 'N'::bpchar,
  istableid char(1) DEFAULT 'N'::bpchar,
  prefix varchar(255),
  suffix varchar(255),
  startnewyear char(1) DEFAULT 'N'::bpchar,
  datecolumn varchar(60),
  decimalpattern varchar(40)
)
;

-- ----------------------------
-- Records of ad_sequence
-- ----------------------------

INSERT INTO public.ad_sequence VALUES (1, 'Y', current_timestamp, 1, current_timestamp , 1, 'ad_client', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (2, 'Y', current_timestamp, 1, current_timestamp , 1, 'ad_org', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (3, 'Y', current_timestamp, 1, current_timestamp , 1, 'ad_role', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (4, 'Y', current_timestamp, 1, current_timestamp , 1, 'ad_role_orgaccess', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (5, 'Y', current_timestamp, 1, current_timestamp , 1, 'ad_sys_config', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (6, 'Y', current_timestamp, 1, current_timestamp , 1, 'ad_user_pos', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (7, 'Y', current_timestamp, 1, current_timestamp , 1, 'c_bpartner', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (8, 'Y', current_timestamp, 1, current_timestamp , 1, 'c_invoice', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (9, 'Y', current_timestamp, 1, current_timestamp , 1, 'c_invoice_number_details', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (10, 'Y',current_timestamp, 1, current_timestamp , 1, 'c_invoiceline', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (11, 'Y',current_timestamp, 1, current_timestamp , 1, 'c_invoicepaymentdetails', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (12, 'Y',current_timestamp, 1, current_timestamp , 1, 'ee_msrmanager', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (13, 'Y',current_timestamp, 1, current_timestamp , 1, 'm_product', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (14, 'Y',current_timestamp, 1, current_timestamp , 1, 'm_product_category', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (15, 'Y',current_timestamp, 1, current_timestamp , 1, 'm_product_price', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (16, 'Y',current_timestamp, 1, current_timestamp , 1, 'm_product_uom', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);
INSERT INTO public.ad_sequence VALUES (17, 'Y',current_timestamp, 1, current_timestamp , 1, 'm_warehouse', NULL, NULL, 'Y', 1, 1, 0, 5000, 'N', 'N', NULL, NULL, 'N', NULL, NULL);



-- ----------------------------
-- Indexes structure for table ad_sequence
-- ----------------------------
CREATE UNIQUE INDEX ad_sequence_name ON public.ad_sequence USING btree (
  name pg_catalog.text_ops ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table ad_sequence
-- ----------------------------
ALTER TABLE public.ad_sequence ADD CONSTRAINT ad_sequence_pkey PRIMARY KEY (ad_sequence_id);

