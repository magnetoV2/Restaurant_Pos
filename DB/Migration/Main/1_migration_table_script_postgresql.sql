-- ----------------------------
-- Table structure for ad_client
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_client";
CREATE TABLE "public"."ad_client" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for ad_org
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_org";
CREATE TABLE "public"."ad_org" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "arabicname" varchar(250),
  "logo" text,
  "phone" varchar(60),
  "email" varchar(60),
  "address" varchar(60),
  "city" varchar(60),
  "country" varchar(60),
  "postal" varchar(60),
  "weburl" varchar(60),
  "footermessage" varchar(250),
  "arabicfootermessage" varchar(250),
  "termsmessage" varchar(250),
  "arabictermsmessage" varchar(250),
  "displayimage" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for ad_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_role";
CREATE TABLE "public"."ad_role" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_role_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for ad_role_orgaccess
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_role_orgaccess";
CREATE TABLE "public"."ad_role_orgaccess" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "ad_role_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for ad_sequence
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_sequence";
CREATE TABLE "public"."ad_sequence" (
  "ad_sequence_id" numeric(10) NOT NULL,
  "isactive" char(1) DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(60) NOT NULL,
  "description" varchar(255),
  "vformat" varchar(40),
  "isautosequence" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "incrementno" numeric(10) NOT NULL,
  "startno" numeric(10) NOT NULL,
  "currentnext" numeric(10) NOT NULL,
  "currentnextsys" numeric(10) NOT NULL,
  "isaudited" char(1) DEFAULT 'N'::bpchar,
  "istableid" char(1) DEFAULT 'N'::bpchar,
  "prefix" varchar(255),
  "suffix" varchar(255),
  "startnewyear" char(1) DEFAULT 'N'::bpchar,
  "datecolumn" varchar(60),
  "decimalpattern" varchar(40)
)
;

-- ----------------------------
-- Table structure for ad_sys_config
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_sys_config";
CREATE TABLE "public"."ad_sys_config" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "ad_user_id" numeric(10) NOT NULL,
  "ad_role_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "pricelistid" numeric,
  "c_bpartner_id" numeric(10),
  "costelementid" numeric,
  "currencyid" numeric,
  "currencycode" varchar(3),
  "cashbookid" numeric,
  "periodid" numeric,
  "paymenttermid" numeric,
  "adtableid" numeric,
  "accountschemaid" numeric,
  "paymentrule" char(1) NOT NULL DEFAULT 'M'::bpchar,
  "printsalessummary" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "printprebill" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "showcomplement" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "isdiscount" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "discpercent" numeric,
  "name" varchar(250),
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for ad_user_pos
-- ----------------------------
DROP TABLE IF EXISTS "public"."ad_user_pos";
CREATE TABLE "public"."ad_user_pos" (
  "ad_user_pos_id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10),
  "ad_org_id" numeric(10),
  "ad_role_id" numeric(10),
  "ad_user_id" numeric(10),
  "c_bpartner_id" numeric(10),
  "m_warehouse_id" numeric(10),
  "name" varchar(60),
  "password" varchar(40),
  "islogged" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "isactive" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60),
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" varchar(40),
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" varchar(40),
  "sessionid" numeric NOT NULL DEFAULT 0
)
;

-- ----------------------------
-- Table structure for c_bpartner
-- ----------------------------
DROP TABLE IF EXISTS "public"."c_bpartner";
CREATE TABLE "public"."c_bpartner" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "c_bpartner_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "searchkey" varchar(60),
  "pricelistid" numeric,
  "iscredit" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "creditamount" numeric,
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "iscashcustomer" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for c_invoice
-- ----------------------------
DROP TABLE IF EXISTS "public"."c_invoice";
CREATE TABLE "public"."c_invoice" (
  "c_invoice_id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10),
  "ad_org_id" numeric(10),
  "ad_role_id" numeric(10),
  "ad_user_id" numeric(10),
  "documentno" varchar(30),
  "m_warehouse_id" numeric(10),
  "c_bpartner_id" numeric(10) NOT NULL,
  "qid" varchar(60) DEFAULT 'N'::character varying,
  "mobilenumber" varchar(60) DEFAULT 0,
  "discounttype" numeric NOT NULL DEFAULT 0,
  "discountvalue" numeric NOT NULL DEFAULT 0,
  "grandtotal" numeric NOT NULL DEFAULT 0,
  "orderid" numeric(10),
  "reason" varchar(60) DEFAULT 'N'::character varying,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60),
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" varchar(40),
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" varchar(40),
  "is_posted" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "is_onhold" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "is_canceled" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "is_completed" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "grandtotal_round_off" numeric NOT NULL DEFAULT 0,
  "total_items_count" numeric DEFAULT 0,
  "balance" numeric NOT NULL DEFAULT 0,
  "change" numeric NOT NULL DEFAULT 0,
  "lossamount" numeric NOT NULL DEFAULT 0,
  "extraamount" numeric NOT NULL DEFAULT 0,
  "is_onsync" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "is_posterror" char(1) NOT NULL DEFAULT 'N'::bpchar
)
;

-- ----------------------------
-- Table structure for c_invoice_number_details
-- ----------------------------
DROP TABLE IF EXISTS "public"."c_invoice_number_details";
CREATE TABLE "public"."c_invoice_number_details" (
  "c_invoice_number_details_id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "ad_user_id" numeric(10) NOT NULL,
  "name" varchar(60),
  "start_no" numeric DEFAULT 0,
  "end_no" numeric DEFAULT 0,
  "doc_no" numeric(10),
  "increment_val" numeric DEFAULT 0,
  "currentnext" numeric DEFAULT 0,
  "m_warehouse_id" numeric(10),
  "macaddr" varchar(60),
  "description" varchar(255),
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for c_invoiceline
-- ----------------------------
DROP TABLE IF EXISTS "public"."c_invoiceline";
CREATE TABLE "public"."c_invoiceline" (
  "c_invoiceline_id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10),
  "ad_org_id" numeric(10),
  "c_invoice_id" numeric(10),
  "ad_user_id" numeric(10),
  "m_product_id" numeric(10),
  "productname" varchar(60),
  "paroductarabicname" varchar(60),
  "productbarcode" varchar(60),
  "c_uom_id" numeric(10),
  "uomname" varchar(60),
  "qtyinvoiced" numeric NOT NULL DEFAULT 0,
  "qtyentered" numeric NOT NULL DEFAULT 0,
  "saleprice" numeric NOT NULL DEFAULT 0,
  "costprice" numeric NOT NULL DEFAULT 0,
  "discounttype" numeric NOT NULL DEFAULT 0,
  "discountvalue" numeric NOT NULL DEFAULT 0,
  "linetotalamt" numeric NOT NULL DEFAULT 0,
  "pricelistid" numeric(10),
  "islinediscounted" varchar(60),
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60),
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" varchar(40),
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" varchar(40)
)
;

-- ----------------------------
-- Table structure for c_invoicepaymentdetails
-- ----------------------------
DROP TABLE IF EXISTS "public"."c_invoicepaymentdetails";
CREATE TABLE "public"."c_invoicepaymentdetails" (
  "c_invoicepaymentdetails_id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10),
  "ad_org_id" numeric(10),
  "c_invoice_id" numeric(10) NOT NULL,
  "cash" numeric NOT NULL DEFAULT 0,
  "card" numeric NOT NULL DEFAULT 0,
  "exchange" numeric NOT NULL DEFAULT 0,
  "redemption" numeric NOT NULL DEFAULT 0,
  "iscomplementary" varchar(1),
  "iscredit" varchar(1),
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60),
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" varchar(40),
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" varchar(40),
  "name_id" varchar(60) DEFAULT 'N'::character varying,
  "mobile_numbler" varchar(60) DEFAULT 0,
  "reason" varchar(100) DEFAULT 'N'::character varying
)
;

-- ----------------------------
-- Table structure for ee_msrmanager
-- ----------------------------
DROP TABLE IF EXISTS "public"."ee_msrmanager";
CREATE TABLE "public"."ee_msrmanager" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "ad_user_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "msr_code" varchar(60),
  "salesrep_id" numeric,
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for m_product
-- ----------------------------
DROP TABLE IF EXISTS "public"."m_product";
CREATE TABLE "public"."m_product" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "m_product_id" numeric(10) NOT NULL,
  "m_product_category_id" numeric,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "searchkey" varchar(60),
  "arabicname" varchar(250),
  "image" varchar(250),
  "scanbyweight" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "scanbyprice" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "uomid" numeric,
  "uomname" varchar(60),
  "sopricestd" numeric,
  "currentcostprice" numeric,
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for m_product_category
-- ----------------------------
DROP TABLE IF EXISTS "public"."m_product_category";
CREATE TABLE "public"."m_product_category" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "m_product_category_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "searchkey" varchar(60),
  "arabicname" varchar(250),
  "image" varchar(250),
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for m_product_price
-- ----------------------------
DROP TABLE IF EXISTS "public"."m_product_price";
CREATE TABLE "public"."m_product_price" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "m_product_id" numeric(10) NOT NULL,
  "pricelistid" numeric,
  "pricestd" numeric,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for m_product_uom
-- ----------------------------
DROP TABLE IF EXISTS "public"."m_product_uom";
CREATE TABLE "public"."m_product_uom" (
  "m_product_uom_id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "m_product_id" numeric(10) NOT NULL,
  "uomid" numeric(10),
  "uomvalue" varchar(60),
  "uomconvrate" numeric(10),
  "m_warehouse_id" numeric(10),
  "name" varchar(60),
  "description" varchar(255),
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for m_warehouse
-- ----------------------------
DROP TABLE IF EXISTS "public"."m_warehouse";
CREATE TABLE "public"."m_warehouse" (
  "id" numeric(10) NOT NULL,
  "ad_client_id" numeric(10) NOT NULL,
  "ad_org_id" numeric(10) NOT NULL,
  "m_warehouse_id" numeric(10) NOT NULL,
  "isactive" char(1) NOT NULL DEFAULT 'Y'::bpchar,
  "created" timestamp(6) NOT NULL DEFAULT now(),
  "createdby" numeric(10) NOT NULL,
  "updated" timestamp(6) NOT NULL DEFAULT now(),
  "updatedby" numeric(10) NOT NULL,
  "name" varchar(250),
  "phone" varchar(60),
  "city" varchar(60),
  "warehouepricelistid" numeric,
  "isdefault" char(1) NOT NULL DEFAULT 'N'::bpchar,
  "attribute1" varchar(60),
  "attribute2" varchar(60),
  "attribute3" varchar(60),
  "attribute4" varchar(60),
  "attribute5" varchar(60),
  "attribute6" varchar(60),
  "attribute7" varchar(60),
  "attribute8" varchar(60),
  "attribute9" varchar(60),
  "attribute10" varchar(60)
)
;

-- ----------------------------
-- Table structure for retail_app_setting
-- ----------------------------
DROP TABLE IF EXISTS "public"."retail_app_setting";
CREATE TABLE "public"."retail_app_setting" (
  "id" numeric(10) NOT NULL,
  "servername" varchar(60),
  "serverport" varchar(60),
  "api_url" varchar(60),
  "server_local_name" varchar(60),
  "server_local_port" varchar(60),
  "server_local_userid" varchar(60),
  "server_local_password" varchar(60),
  "server_local_dbname" varchar(60),
  "display_language" varchar(60) NOT NULL DEFAULT 'E'::character varying,
  "on_printer" varchar(60)
)
;

-- ----------------------------
-- Primary Key structure for table ad_client
-- ----------------------------
ALTER TABLE "public"."ad_client" ADD CONSTRAINT "ad_client_pkey" PRIMARY KEY ("ad_client_id");

-- ----------------------------
-- Primary Key structure for table ad_org
-- ----------------------------
ALTER TABLE "public"."ad_org" ADD CONSTRAINT "ad_org_pkey" PRIMARY KEY ("ad_org_id");

-- ----------------------------
-- Primary Key structure for table ad_role
-- ----------------------------
ALTER TABLE "public"."ad_role" ADD CONSTRAINT "ad_role_pkey" PRIMARY KEY ("ad_role_id");

-- ----------------------------
-- Primary Key structure for table ad_role_orgaccess
-- ----------------------------
ALTER TABLE "public"."ad_role_orgaccess" ADD CONSTRAINT "ad_role_orgaccess_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table ad_sequence
-- ----------------------------
CREATE UNIQUE INDEX "ad_sequence_name" ON "public"."ad_sequence" USING btree (
  "name" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table ad_sequence
-- ----------------------------
ALTER TABLE "public"."ad_sequence" ADD CONSTRAINT "ad_sequence_pkey" PRIMARY KEY ("ad_sequence_id");

-- ----------------------------
-- Primary Key structure for table ad_sys_config
-- ----------------------------
ALTER TABLE "public"."ad_sys_config" ADD CONSTRAINT "ad_sys_config_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table ad_user_pos
-- ----------------------------
ALTER TABLE "public"."ad_user_pos" ADD CONSTRAINT "ad_user_pos_pkey" PRIMARY KEY ("ad_user_pos_id");

-- ----------------------------
-- Primary Key structure for table c_bpartner
-- ----------------------------
ALTER TABLE "public"."c_bpartner" ADD CONSTRAINT "c_bpartner_pkey" PRIMARY KEY ("c_bpartner_id");

-- ----------------------------
-- Primary Key structure for table c_invoice
-- ----------------------------
ALTER TABLE "public"."c_invoice" ADD CONSTRAINT "c_invoice_pkey" PRIMARY KEY ("c_invoice_id");

-- ----------------------------
-- Primary Key structure for table c_invoice_number_details
-- ----------------------------
ALTER TABLE "public"."c_invoice_number_details" ADD CONSTRAINT "c_invoice_number_details_pkey" PRIMARY KEY ("c_invoice_number_details_id");

-- ----------------------------
-- Primary Key structure for table c_invoiceline
-- ----------------------------
ALTER TABLE "public"."c_invoiceline" ADD CONSTRAINT "c_invoiceline_pkey" PRIMARY KEY ("c_invoiceline_id");

-- ----------------------------
-- Primary Key structure for table c_invoicepaymentdetails
-- ----------------------------
ALTER TABLE "public"."c_invoicepaymentdetails" ADD CONSTRAINT "c_invoicepaymentdetails_pkey" PRIMARY KEY ("c_invoicepaymentdetails_id");

-- ----------------------------
-- Primary Key structure for table ee_msrmanager
-- ----------------------------
ALTER TABLE "public"."ee_msrmanager" ADD CONSTRAINT "ee_msrmanager_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table m_product
-- ----------------------------
ALTER TABLE "public"."m_product" ADD CONSTRAINT "m_product_pkey" PRIMARY KEY ("m_product_id");

-- ----------------------------
-- Primary Key structure for table m_product_category
-- ----------------------------
ALTER TABLE "public"."m_product_category" ADD CONSTRAINT "m_product_category_pkey" PRIMARY KEY ("m_product_category_id");

-- ----------------------------
-- Primary Key structure for table m_product_price
-- ----------------------------
ALTER TABLE "public"."m_product_price" ADD CONSTRAINT "m_product_price_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table m_product_uom
-- ----------------------------
ALTER TABLE "public"."m_product_uom" ADD CONSTRAINT "m_product_uom_pkey" PRIMARY KEY ("m_product_uom_id");

-- ----------------------------
-- Primary Key structure for table m_warehouse
-- ----------------------------
ALTER TABLE "public"."m_warehouse" ADD CONSTRAINT "m_warehouse_pkey" PRIMARY KEY ("m_warehouse_id");

-- ----------------------------
-- Primary Key structure for table retail_app_setting
-- ----------------------------
ALTER TABLE "public"."retail_app_setting" ADD CONSTRAINT "retail_app_setting_pkey" PRIMARY KEY ("id");
