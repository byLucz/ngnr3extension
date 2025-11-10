CREATE INDEX IF NOT EXISTS index_citymarker_true
ON public."checkSpecEmplCity" ("City", "Employee")
WHERE "Marker" = true;