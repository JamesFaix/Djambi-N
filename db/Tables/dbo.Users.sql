CREATE TABLE [dbo].[Users](
	[UserId]                   [int] IDENTITY(1,1) NOT NULL,
	[Name]                     [nvarchar](50) NOT NULL,
	[Password]                 [nvarchar](50) NULL,
	[CreatedOn]                [datetime2](7) NOT NULL,
	[FailedLoginAttempts]      [tinyint] NOT NULL,
	[LastFailedLoginAttemptOn] [datetime2] NULL
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_Users_Name] UNIQUE NONCLUSTERED ([Name] ASC)
)