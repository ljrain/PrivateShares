use [EntityManager{orgname}]
go

BULK INSERT vwappuserlist
FROM '{ToolkitPath}\AddRandomUsers\AddApplicationUsers\appuserlist.csv' 
WITH(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '\n'
);