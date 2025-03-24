DECLARE @copyTableUpdateBatchSize nvarchar(max)
DECLARE @mainTableInsertBatchSize nvarchar(max)
 
SET @copyTableUpdateBatchSize = '100000'
SET @mainTableInsertBatchSize = '100000'
 
--computed parameters SOURCE
DECLARE @sourceCopyName nvarchar(max)
DECLARE @sourceCopyClusteredIndexName nvarchar(max)
DECLARE @sourceCopyParentingIndexName nvarchar(max)
DECLARE @sourceTestDataTableName nvarchar(max)
DECLARE @sourceTestDataIndexName nvarchar(max)
DECLARE @sourceRecordsToCreate nvarchar(max)
DECLARE @sourceColumnNames nvarchar(max)
DECLARE @sourceCopyDefaultIdConstraintName nvarchar(max)

SET @sourceCopyName = @sourceTableName + 'Copy'
SET @sourceCopyClusteredIndexName = 'ndx_' + @sourceCopyName + '__identity'
SET @sourceCopyParentingIndexName = 'ndx_' + @sourceCopyName + '_' + @sourceParentAttributeName
SET @sourceTestDataTableName = @sourceTableName + 'TestCsv'
SET @sourceTestDataIndexName = 'ndx_' + @sourceTestDataTableName + '_identity'
SET @sourceCopyDefaultIdConstraintName = 'DF_' + @sourceCopyName + '__pkeycopy'
 
--computed parameters TARGET
DECLARE @targetCopyName nvarchar(max)
DECLARE @targetCopyClusteredIndexName nvarchar(max)
DECLARE @targetCopyParentingIndexName nvarchar(max)
DECLARE @targetTestDataTableName nvarchar(max)
DECLARE @targetTestDataIndexName nvarchar(max)
DECLARE @targetRecordsToCreate nvarchar(max)
DECLARE @targetColumnNames nvarchar(max)
DECLARE @targetCopyDefaultIdConstraintName nvarchar(max)
SET @targetCopyName = @targetTableName + 'Copy'
SET @targetCopyClusteredIndexName = 'ndx_' + @targetCopyName + '__identity'
SET @targetCopyParentingIndexName = 'ndx_' + @targetCopyName + '_' + @targetParentAttributeName
SET @targetTestDataTableName = @targetTableName + 'TestCsv'
SET @targetTestDataIndexName = 'ndx_' + @targetTestDataTableName + '_identity'
SET @targetCopyDefaultIdConstraintName = 'DF_' + @targetCopyName + '__pkeycopy'
 
DECLARE @recordsUpdated int
 
PRINT 'Checking if masterRecordIds are valid'
EXEC('
IF NOT EXISTS(SELECT * FROM ' + @sourceTableName + ' WHERE ' + @sourcePKName + '=''' + @sourceMasterRecordId + ''')
BEGIN
RAISERROR(N''@sourceMasterRecordId is invalid'', 11, 1);
RETURN
END
 
IF NOT EXISTS(SELECT * FROM ' + @targetTableName + ' WHERE ' + @targetPKName + '=''' + @targetMasterRecordId + ''')
BEGIN
RAISERROR(N''@targetMasterRecordId is invalid'', 11, 1);
RETURN
END')
 
EXEC('UPDATE ' + @sourceTableName + ' SET ' + @sourceParentAttributeName + ' = NULL WHERE ' + @sourcePKName + ' = ''' + @sourceMasterRecordId + ''' ')
EXEC('UPDATE ' + @targetTableName + ' SET ' + @targetParentAttributeName + ' = NULL WHERE ' + @targetPKName + ' = ''' + @targetMasterRecordId + ''' ')
 
PRINT 'Drop test data table'
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND [Name] = @sourceTestDataTableName) BEGIN EXEC('DROP TABLE ' + @sourceTestDataTableName) END
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND [Name] = @targetTestDataTableName) BEGIN EXEC('DROP TABLE ' + @targetTestDataTableName) END
 
PRINT 'Create test data table for importing from CSV'
EXEC('CREATE TABLE ' + @sourceTestDataTableName + ' (Id bigint, ParentId bigint, Depth int)')
EXEC('CREATE TABLE ' + @targetTestDataTableName + ' (Id bigint, ParentId bigint)')
 
PRINT 'Bulk import from CSV'
EXEC('BULK INSERT ' + @sourceTestDataTableName + ' FROM ''' + @sourceDataFileName + ''' WITH (FIELDTERMINATOR='','',ROWTERMINATOR=''\n'')')
EXEC('BULK INSERT ' + @targetTestDataTableName + ' FROM ''' + @targetDataFileName + ''' WITH (FIELDTERMINATOR='','',ROWTERMINATOR=''\n'')')
 
PRINT 'Create index on Id/ParentId to assist future queries'
EXEC('CREATE NONCLUSTERED INDEX '+ @sourceTestDataIndexName +' ON ' + @sourceTestDataTableName + ' ([Id]) INCLUDE ([ParentId],[Depth])')
EXEC('CREATE NONCLUSTERED INDEX '+ @targetTestDataIndexName +' ON ' + @targetTestDataTableName + ' ([Id]) INCLUDE ([ParentId])')
 
PRINT 'Get the number of records imported from CSV to create that many copies of master record from main table'
DECLARE @sqlToGetRecordCountSource nvarchar(max)
SET @sqlToGetRecordCountSource = 'SET @recCount = CONVERT(nvarchar, (SELECT COUNT(Id) FROM ' + @sourceTestDataTableName + ' WITH(NOLOCK)))'
EXECUTE sp_EXECUTESQL @sqlToGetRecordCountSource, N'@recCount int OUTPUT', @recCount = @sourceRecordsToCreate OUTPUT
 
DECLARE @sqlToGetRecordCountTarget nvarchar(max)
SET @sqlToGetRecordCountTarget = 'SET @recCount = CONVERT(nvarchar, (SELECT COUNT(Id) FROM ' + @targetTestDataTableName + ' WITH(NOLOCK)))'
EXECUTE sp_EXECUTESQL @sqlToGetRecordCountTarget, N'@recCount int OUTPUT', @recCount = @targetRecordsToCreate OUTPUT
 
IF @sourceRecordsToCreate > 3000000  OR @targetRecordsToCreate > 3000000
BEGIN
RAISERROR (N'If there are more than 2 million records in either source or target then adding the sequential ID column will take a really long time.
Reduce the size of input csv files and increase the multiplication factor at the top of this script to achieve the numbers you need.',
               11,
               1);
     RETURN
END
 
PRINT 'Drop copy table'
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND [Name] = @sourceCopyName) BEGIN EXEC('DROP TABLE ' + @sourceCopyName) END
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'U' AND [Name] = @targetCopyName) BEGIN EXEC('DROP TABLE ' + @targetCopyName) END
 
PRINT 'Create copy records from main table into copy table - SOURCE'
--Identity helps to map between copy data and incoming test (hierarchy) data
EXEC('SELECT TOP ' + @sourceRecordsToCreate + ' 
IDENTITY(bigint,1,1) AS _identity, 
o.* 
INTO 
' + @sourceCopyName + ' 
FROM sys.objects s1 
CROSS JOIN sys.objects s2 
CROSS JOIN sys.objects s3 
CROSS JOIN ' + @sourceTableName + ' o
WHERE o.' + @sourcePKName + ' = ''' + @sourceMasterRecordId + '''') 
 
PRINT 'Create copy records from main table into copy table - TARGET'
--Identity helps to map between copy data and incoming test (hierarchy) data
EXEC('SELECT TOP ' + @targetRecordsToCreate + ' 
IDENTITY(bigint,1,1) AS _identity, 
o.* 
INTO 
' + @targetCopyName + ' 
FROM sys.objects s1 
CROSS JOIN sys.objects s2 
CROSS JOIN sys.objects s3 
CROSS JOIN ' + @targetTableName + ' o
WHERE o.' + @targetPKName + ' = ''' + @targetMasterRecordId + '''') 
 
PRINT 'Create index on copy table to assist the application-of-hierarchy section coming up - SOURCE'
EXEC('CREATE CLUSTERED INDEX ' + @sourceCopyClusteredIndexName + ' ON ' + @sourceCopyName + ' (_identity ASC)')
 
PRINT 'Create index on copy table to assist the application-of-hierarchy section coming up - TARGET'
EXEC('CREATE CLUSTERED INDEX ' + @targetCopyClusteredIndexName + ' ON ' + @targetCopyName + ' (_identity ASC)')
 
PRINT 'Get comma separated list of columns from source table whose values need to be copied over '
DECLARE @sqlToGetSourceColumns nvarchar(max)
SET @sqlToGetSourceColumns = 'SELECT @sourceColumnNames = COALESCE(@sourceColumnNames+'','' ,'''') + columnNames.name
FROM (SELECT sc.name
FROM syscolumns sc
WHERE sc.name NOT IN (''_identity'', ''VersionNumber'', ''_pkeycopy'',''_parentidcopy'')
AND sc.id = (SELECT so.id 
FROM sysobjects so 
WHERE so.type = ''U'' 
AND so.Name = ''' + @sourceCopyName + ''')) 
AS columnNames'
EXECUTE sp_EXECUTESQL @sqlToGetSourceColumns, N'@sourceColumnNames nvarchar(max) OUTPUT', @sourceColumnNames = @sourceColumnNames OUTPUT
 
PRINT 'Get comma separated list of columns from target table whose values need to be copied over '
DECLARE @sqlToGetTargetColumns nvarchar(max)
SET @sqlToGetTargetColumns = 'SELECT @targetColumnNames = COALESCE(@targetColumnNames+'','' ,'''') + columnNames.name
FROM (SELECT sc.name
FROM syscolumns sc
WHERE sc.name NOT IN (''_identity'', ''VersionNumber'', ''_pkeycopy'',''_parentidcopy'')
AND sc.id = (SELECT so.id 
FROM sysobjects so 
WHERE so.type = ''U'' 
AND so.Name = ''' + @targetCopyName + ''')) 
AS columnNames'
EXECUTE sp_EXECUTESQL @sqlToGetTargetColumns, N'@targetColumnNames nvarchar(max) OUTPUT', @targetColumnNames = @targetColumnNames OUTPUT
 
PRINT 'Sql to update source hierarchy'
DECLARE @sqlToUpdateSourceHierarchy nvarchar(max)
SET @sqlToUpdateSourceHierarchy = '
UPDATE TOP(' + @copyTableUpdateBatchSize + ') ac 
SET ac.' + @sourceParentAttributeName + ' = ac2.' + @sourcePKName + ',
ac.' + @sourceDepthAttributeName + ' = selfcsv.Depth
FROM ' + @sourceCopyName + ' ac WITH(TABLOCK)
JOIN ' + @sourceTestDataTableName + ' selfcsv on ac._identity = selfcsv.Id
JOIN ' + @sourceCopyName + ' ac2 on ac2._identity = selfcsv.ParentId
WHERE ac.' + @sourceParentAttributeName + ' IS NULL
SET @recordsUpdated = @@ROWCOUNT'
 
PRINT 'sqlToUpdateTargetAssociation'
DECLARE @sqlToUpdateTargetAssociation nvarchar(max)
SET @sqlToUpdateTargetAssociation = '
UPDATE TOP(' + @copyTableUpdateBatchSize + ') t 
SET t.' + @targetParentAttributeName + ' = s.' + @sourcePKName + '
FROM ' + @targetCopyName + ' t WITH(TABLOCK)
JOIN ' + @targetTestDataTableName + ' targetcsv on t._identity = targetcsv.Id
JOIN ' + @sourceCopyName + ' s on s._identity = targetcsv.ParentId
WHERE t.' + @targetParentAttributeName + ' IS NULL
SET @recordsUpdated = @@ROWCOUNT'
 
PRINT 'sqlToCreateRecordsInSourceTable'
DECLARE @sqlToCreateRecordsInSourceTable nvarchar(max)
SET @sqlToCreateRecordsInSourceTable = '
INSERT INTO ' + @sourceTableName + ' WITH(TABLOCK) (' + @sourceColumnNames + ') 
SELECT TOP ' + @mainTableInsertBatchSize + ' ' + @sourceColumnNames + ' 
FROM ' + @sourceCopyName + '
WHERE _identity > @recordsProcessed
SET @recordsUpdated = @@ROWCOUNT'
 
PRINT 'sqlToCreateRecordsInTargetTable'
DECLARE @sqlToCreateRecordsInTargetTable nvarchar(max)
SET @sqlToCreateRecordsInTargetTable = '
INSERT INTO ' + @targetTableName + ' WITH(TABLOCK) (' + @targetColumnNames + ') 
SELECT TOP ' + @mainTableInsertBatchSize + ' ' + @targetColumnNames + ' 
FROM ' + @targetCopyName + '
WHERE _identity > @recordsProcessed
SET @recordsUpdated = @@ROWCOUNT'
 
DECLARE @recordsProcessed bigint
 
WHILE (@multiplicationFactor > 0)
BEGIN
PRINT 'LOOP START. REMAINING LOOP COUNT = ' + CAST(@multiplicationFactor as nvarchar(max))
SET @multiplicationFactor = @multiplicationFactor - 1
 
-- generate sequential primary keys for all the records
PRINT 'PK: generate sequential primary key for all records into a new column - SOURCE'
EXEC('ALTER TABLE ' + @sourceCopyName + ' ADD _pkeycopy uniqueidentifier NOT NULL CONSTRAINT ' + @sourceCopyDefaultIdConstraintName + ' DEFAULT NEWSEQUENTIALID()')
 
PRINT 'PK: generate sequential primary key for all records into a new column - TARGET'
EXEC('ALTER TABLE ' + @targetCopyName + ' ADD _pkeycopy uniqueidentifier NOT NULL CONSTRAINT ' + @targetCopyDefaultIdConstraintName + ' DEFAULT NEWSEQUENTIALID()')
 
PRINT 'PK: drop constraint, its no longer needed'
EXEC('ALTER TABLE ' + @sourceCopyName + ' DROP CONSTRAINT ' + @sourceCopyDefaultIdConstraintName)
EXEC('ALTER TABLE ' + @targetCopyName + ' DROP CONSTRAINT ' + @targetCopyDefaultIdConstraintName)
 
PRINT 'PK: drop the primary key column'
EXEC('ALTER TABLE ' + @sourceCopyName + ' DROP COLUMN ' + @sourcePKName)
EXEC('ALTER TABLE ' + @targetCopyName + ' DROP COLUMN ' + @targetPKName)
 
PRINT 'PK: rename the sequential primary key column to actual primary key column name so that subsequent insertion into main table can proceed'
EXEC('EXECUTE sp_RENAME ''' + @sourceCopyName + '.[_pkeycopy]'', ''' + @sourcePKName + ''', ''COLUMN''')
EXEC('EXECUTE sp_RENAME ''' + @targetCopyName + '.[_pkeycopy]'', ''' + @targetPKName + ''', ''COLUMN''')
 
-- reset parenting for all records on both source and target entities
PRINT 'ParentId: Drop the parenting index'
IF EXISTS (SELECT * FROM sysindexes WHERE [Name] = @sourceCopyParentingIndexName) BEGIN EXEC('DROP INDEX ' + @sourceCopyName + '.' + @sourceCopyParentingIndexName) END
IF EXISTS (SELECT * FROM sysindexes WHERE [Name] = @targetCopyParentingIndexName) BEGIN EXEC('DROP INDEX ' + @targetCopyName + '.' + @targetCopyParentingIndexName) END
 
PRINT 'ParentId: Create empty parent column'
EXEC('ALTER TABLE ' + @sourceCopyName + ' ADD _parentidcopy uniqueidentifier')
EXEC('ALTER TABLE ' + @targetCopyName + ' ADD _parentidcopy uniqueidentifier')
 
PRINT 'ParentId: Drop the old parent column'
EXEC('ALTER TABLE ' + @sourceCopyName + ' DROP COLUMN ' + @sourceParentAttributeName)
EXEC('ALTER TABLE ' + @targetCopyName + ' DROP COLUMN ' + @targetParentAttributeName)
 
PRINT 'ParentId: Rename empty parent column'
EXEC('EXECUTE sp_RENAME ''' + @sourceCopyName + '.[_parentidcopy]'', ''' + @sourceParentAttributeName + ''', ''COLUMN''')
EXEC('EXECUTE sp_RENAME ''' + @targetCopyName + '.[_parentidcopy]'', ''' + @targetParentAttributeName + ''', ''COLUMN''')
 
PRINT 'ParentId: Create index on parenting column'
EXEC('CREATE NONCLUSTERED INDEX [' + @sourceCopyParentingIndexName + '] ON ' + @sourceCopyName + ' (' + @sourceParentAttributeName + ' ASC) INCLUDE (_identity)')
EXEC('CREATE NONCLUSTERED INDEX [' + @targetCopyParentingIndexName + '] ON ' + @targetCopyName + ' (' + @targetParentAttributeName + ' ASC) INCLUDE (_identity)')
 
SET @recordsProcessed = 0
PRINT 'Apply hierarchies on source'
WHILE (1 > 0) 
BEGIN 
BEGIN TRAN
EXECUTE sp_EXECUTESQL @sqlToUpdateSourceHierarchy, N'@recordsUpdated int OUTPUT', @recordsUpdated = @recordsUpdated OUTPUT
COMMIT TRAN
IF @recordsUpdated = 0 
BEGIN 
BREAK 
END 
SET @recordsProcessed = @recordsProcessed + @recordsUpdated
END
 
SET @recordsProcessed = 0
PRINT 'Associate target records with source'
WHILE (1 > 0) 
BEGIN 
BEGIN TRAN
EXECUTE sp_EXECUTESQL @sqlToUpdateTargetAssociation, N'@recordsUpdated int OUTPUT', @recordsUpdated = @recordsUpdated OUTPUT
COMMIT TRAN
IF @recordsUpdated = 0 
BEGIN 
BREAK 
END 
SET @recordsProcessed = @recordsProcessed + @recordsUpdated
END
 
SET @recordsProcessed = 0
PRINT 'Copy all the columns data from copy table to main table - SOURCE'
WHILE (1 > 0) 
BEGIN 
BEGIN TRAN
EXECUTE sp_EXECUTESQL @sqlToCreateRecordsInSourceTable, N'@recordsProcessed bigint, @recordsUpdated int OUTPUT', @recordsProcessed = @recordsProcessed, @recordsUpdated = @recordsUpdated OUTPUT
COMMIT TRAN
IF @recordsUpdated = 0 
BEGIN 
BREAK 
END 
SET @recordsProcessed = @recordsProcessed + @recordsUpdated
END
 
SET @recordsProcessed = 0
PRINT 'Copy all the columns data from copy table to main table - TARGET'
WHILE (1 > 0) 
BEGIN 
BEGIN TRAN
EXECUTE sp_EXECUTESQL @sqlToCreateRecordsInTargetTable, N'@recordsProcessed bigint, @recordsUpdated int OUTPUT', @recordsProcessed = @recordsProcessed, @recordsUpdated = @recordsUpdated OUTPUT
COMMIT TRAN
IF @recordsUpdated = 0 
BEGIN 
BREAK 
END
SET @recordsProcessed = @recordsProcessed + @recordsUpdated
END
END
