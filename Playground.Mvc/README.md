## MS SQL Server

### Snapshot isolation level

Execute the following script to enable `snapshot` isolation level.

```sql
ALTER DATABASE Playground
SET ALLOW_SNAPSHOT_ISOLATION ON 
```

[Snapshot Isolation in SQL Server](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/snapshot-isolation-in-sql-server).

### Before each benchmark run

Execute the following script to restore `Items` table to it's original state.

```sql
TRUNCATE TABLE Items
```

### Switch between SQL Server isolation level

- Update `WebApiConfig.Register` method to enable `AsyncItemsController` or `ScopeItemsController` implementation.
- Update `ScopeItemsController.DefaultIsolationLevel` property to set isolation level when `ScopeItemsController` enabled.