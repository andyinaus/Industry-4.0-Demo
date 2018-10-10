IF NOT EXISTS (SELECT * FROM sys.tables WHERE NAME='Devices')	
	CREATE TABLE [dbo].[Devices]
	(
		[ID] varchar(36) NOT NULL PRIMARY KEY,
		[Type] varchar(100) NOT NULL
	)
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE NAME='DeviceReadings')	
CREATE TABLE [dbo].[DeviceReadings]
	(
		[DeviceID] varchar(36) NOT NULL,
		[DateTime] DateTime NOT NULL,
		[Speed] int NOT NULL,
		[PackageTrackingAlarmState] varchar(10) NOT NULL,
		[CurrentTotalBoards] INT NOT NULL,
		[CurrentRecipeCount] INT NOT NULL,
		PRIMARY KEY ([DeviceID], [DateTime]),
		FOREIGN KEY ([DeviceID]) REFERENCES dbo.Devices(ID)
	)
GO


