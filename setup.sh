#!/bin/bash

echo "Starting SQL Server..."
docker compose up -d

echo "Waiting 30 seconds for SQL Server to start..."
sleep 30

echo "Creating database and tables..."
sqlcmd -S localhost -U sa -P 'Salah@123!' -N disable -i Docker/init-db.sql

echo "Done! You can now run: dotnet run"
```

But for daily work, you just need:
```
docker compose up -d
dotnet run