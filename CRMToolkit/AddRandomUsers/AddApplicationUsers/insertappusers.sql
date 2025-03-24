use [EntityManagerorg9bdebcb5]
go

BULK INSERT vwappuserlist
FROM 'C:\CRMToolkitNew\AddRandomUsers\AddApplicationUsers\appuserlist.csv' 
WITH(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '\n'
);
