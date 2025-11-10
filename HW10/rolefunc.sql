CREATE OR REPLACE FUNCTION public."getCitySpectator"(val_cardids uuid[])
RETURNS TABLE("CardID" uuid, "Value" uuid, "Type" integer)
AS
$FUNCTION$
BEGIN
    RETURN query
    SELECT
        bt."InstanceID"      AS "CardID",
        map."Employee"       AS "Value", 
        13                   AS "Type" 
    FROM public."dvtable_{30eb9b87-822b-4753-9a50-a1825dca1b74}" bt   
    JOIN public."checkSpecEmplCity" map
        ON  map."City"   = bt."City" 
        AND map."Marker" = true  
    WHERE bt."InstanceID" = any(val_cardids); 
END;
$FUNCTION$
LANGUAGE plpgsql;
