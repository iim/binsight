sqlcmd -S %1 -d %3 -U sa -P %2 -i XXX_bio_insight_db_delete.sql
sqlcmd -S %1 -d %3 -U sa -P %2 -i 001_bio_insight_db_create_structure.sql
sqlcmd -S %1 -d %3 -U sa -P %2 -i 002_bio_insight_db_add_empty_rows.sql
sqlcmd -S %1 -d %3 -U sa -P %2 -i 003_bio_insight_db_add_indices.sql
sqlcmd -S %1 -d %3 -U sa -P %2 -i 004_bio_insight_db_add_constraints.sql

