USE [Local]
GO

/****** Object:  Table [dbo].[taxi-fare-test]    Script Date: 5/9/2019 5:22:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[taxi-fare-test](
	[vendor_id] [nvarchar](50) NOT NULL,
	[rate_code] [int] NOT NULL,
	[passenger_count] [int] NOT NULL,
	[trip_time_in_secs] [int] NOT NULL,
	[trip_distance] [float] NOT NULL,
	[payment_type] [nvarchar](50) NOT NULL,
	[fare_amount] [float] NOT NULL
) ON [PRIMARY]
GO
/*** DUE TO THE DATASET IS VERY LARGE, PLEASE MANUALLY IMPORT IT INTO THIS TABLE ***/