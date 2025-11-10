CREATE TABLE public."checkSpecEmplCity" (
    "RowId"   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Employee" uuid NOT NULL,
    "City"     uuid NOT NULL,
    "Marker"   boolean NOT NULL,
    CONSTRAINT fk_ext_empl FOREIGN KEY ("Employee")
        REFERENCES public."dvtable_{dbc8ae9d-c1d2-4d5e-978b-339d22b32482}" ("RowID"),
    CONSTRAINT fk_ext_city FOREIGN KEY ("City")
        REFERENCES public."dvtable_{1b1a44fb-1fb1-4876-83aa-95ad38907e24}" ("RowID"),
    UNIQUE ("Employee", "City")
);