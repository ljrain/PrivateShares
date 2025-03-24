use [EntityManagerorg9bdebcb5]
go

BULK INSERT vwuserlist
FROM 'C:\CRMToolkitNew\AddRandomUsers\userlist.csv' 
WITH(
FIELDTERMINATOR = ',',
ROWTERMINATOR = '\n'
);
