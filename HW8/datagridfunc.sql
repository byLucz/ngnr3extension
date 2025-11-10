CREATE OR REPLACE FUNCTION "getInfoFromEmpl"(val_emplid uuid)
RETURNS TABLE (
    "RowNum"  integer,
    "DateOut" date,
    "City"    text,
    "Reason"  text,
    "State"   text
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        ROW_NUMBER() OVER (ORDER BY mydoc."dateOut") AS "RowNum",
        mydoc."dateOut"::date                        AS "DateOut",
        city."Name"                                  AS "City",
        mydoc."formOut"                              AS "Reason",
        statelocale."Name"                           AS "State"
    FROM public."dvtable_{30eb9b87-822b-4753-9a50-a1825dca1b74}" AS mydoc
    JOIN public."dvtable_{1b1a44fb-1fb1-4876-83aa-95ad38907e24}" AS city
        ON city."RowID"  = mydoc."City"
    JOIN public."dvtable_{521b4477-dd10-4f57-a453-09c70adb7799}" AS state
        ON state."RowID" = mydoc."State"
    JOIN public."dvtable_{da37ca71-a977-48e9-a4fd-a2b30479e824}" AS statelocale
        ON statelocale."ParentRowID" = mydoc."State"
    WHERE mydoc."Kind"     = 'a69ef565-c026-4942-9efd-1bf9a137cab0'
      AND mydoc."emplOut"  = val_emplid
    ORDER BY mydoc."dateOut" ASC;
END;
$$;
