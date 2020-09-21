--CREATE LANGUAGE plpgsql;

-- Function: update_ad_user_pos()

-- DROP FUNCTION update_ad_user_pos();

CREATE OR REPLACE FUNCTION update_ad_user_pos()
  RETURNS integer AS
$BODY$
declare
	r RECORD;
BEGIN
   FOR r IN
        SELECT ad_user_id FROM ad_user_pos
    LOOP
        UPDATE ad_user_pos set islogged = 'N' , isactive = 'N' where ad_user_id = r.ad_user_id ;
    END LOOP;
   RETURN 1;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION update_ad_user_pos() OWNER TO adempiere;