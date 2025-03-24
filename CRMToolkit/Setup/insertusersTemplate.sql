use [EntityManager{orgname}]
go

BULK INSERT vwuserlist
FROM '{ToolkitPath}\AddRandomUsers\userlist.csv' 
WITH(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '\n'
);