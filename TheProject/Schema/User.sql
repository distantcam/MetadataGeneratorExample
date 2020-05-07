CREATE TABLE [dbo].[User](
    [Id] [int] IDENTITY(1,1) NOT NULL,

    [FirstName] [varchar](MAX) NOT NULL,
    [LastName] [varchar](MAX) NULL,
    [Email] [varchar](250) NULL,
    [CreditCard] [char](8) NULL,

 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)
)
GO
