# NGNR3EXTENSION

## Оптимизация ролевой функции
Исходные данные:
~~~
Planning Time: 0.114 ms
Execution Time: 0.537 ms
~~~
После анализа методом explain analyze и дальнейшим изучением в сервисе explain.tensor.ru было найдено узкое горлышко в том, что из таблицы каждый раз дергаются все значения (а их там очень много), поэтому добавлен индекс обеспечивающий эффективную фильтрацию по условиям
```sql
CREATE INDEX IF NOT EXISTS index_citymarker_true
ON public."checkSpecEmplCity" ("City", "Employee")
WHERE "Marker" = true;
```
Результат:
~~~
Planning Time: 0.134 ms
Execution Time: 0.254 ms
~~~
