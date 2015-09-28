CREATE TABLE [dbo].[clients] (
    [Id]          INT       IDENTITY (1, 1) NOT NULL,
	[client_name] VARCHAR(200) NOT NULL, 
    [client_id]     VARCHAR (200) NOT NULL,
    [client_secret] VARCHAR (500) NOT NULL,
    [client_description] NCHAR(10) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[users] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [username] VARCHAR (100) NOT NULL,
    [pwd]      VARCHAR (500) NULL,
    [mobile]   VARCHAR (15)  NULL,
    [birthday] DATETIME      NULL,
    [sex]      INT           NULL,
    [age]      INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tokens] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [clientId]     VARCHAR (200) NULL,
    [userName]     VARCHAR (100) NULL,
    [accessToken]  VARCHAR (300) NULL,
    [refreshToken] VARCHAR (300) NULL,
    [issuedUtc]    DATETIME      NULL,
    [expiresUtc]   DATETIME      NULL,
    [ipAddress]    VARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);