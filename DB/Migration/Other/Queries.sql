SELECT c_invoice_id, ad_client_id, ad_org_id, ad_role_id, ad_user_id, 
       documentno, m_warehouse_id, c_bpartner_id, qid, mobilenumber, 
       discounttype, discountvalue, grandtotal, orderid, reason, created, createdby, updated, 
       updatedby, is_posted, is_onhold, is_canceled, is_completed, grandtotal_round_off, 
       total_items_count, balance, change, lossamount, extraamount
  FROM c_invoice where is_completed = 'Y' Order by created Desc;

  SELECT t2.name, t2.searchkey, t2.pricelistid, t2.iscredit, t2.creditamount, t2.isdefault, t2.iscashcustomer
  FROM c_bpartner;

  SELECT t1.c_invoice_id, t1.ad_client_id, t1.ad_org_id, t1.ad_role_id, t1.ad_user_id, t1.documentno, t1.m_warehouse_id, t1.c_bpartner_id, t1.qid, t1.mobilenumber, t1.discounttype, t1.discountvalue, t1.grandtotal, t1.orderid, t1.reason, t1.created, t1.createdby, t1.updated, t1.updatedby, t1.is_posted, t1.is_onhold, t1.is_canceled, t1.is_completed, t1.grandtotal_round_off, t1.total_items_count, t1.balance, t1.change, t1.lossamount, t1.extraamount, t2.name, t2.searchkey, t2.pricelistid, t2.iscredit, t2.creditamount, t2.isdefault, t2.iscashcustomer FROM c_invoice t1 , c_bpartner t2 Where is_completed = 'Y' And t1.c_bpartner_id = t2.c_bpartner_id Order by created Desc;

SELECT COUNT(*) FROM m_product where ad_client_id= and ad_org_id = and ;

CREATE LANGUAGE plpgsql;