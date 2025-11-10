CREATE OR REPLACE VIEW authorCoreOrg AS
SELECT
    usr."RowID"           AS "emplId",
    core."Name"       AS "coreOrg"
FROM public."dvtable_{dbc8ae9d-c1d2-4d5e-978b-339d22b32482}" AS usr
JOIN public."dvtable_{7473f07f-11ed-4762-9f1e-7ff10808ddd1}" AS dep
    ON usr."ParentRowID" = dep."RowID"
JOIN public."dvtable_{7473f07f-11ed-4762-9f1e-7ff10808ddd1}" AS core
    ON core."SDID" = dep."SDID"
	AND core."SectionTreeKey" = subpath(dep."SectionTreeKey", 0, 2)
