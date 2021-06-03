﻿CREATE TABLE [dbo].[Caller] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [CiviContactId]          INT            NOT NULL,
    [CallerIdentifier]       NVARCHAR (45)  NOT NULL,
    [FirstName]              NVARCHAR (50)  NOT NULL,
    [LastName]               NVARCHAR (50)  NULL,
    [DateOfBirth]            DATE NULL,
	[ResidencePostalCode]    NVARCHAR (10)  NULL,
    [Phone]                  NVARCHAR (20)  NOT NULL,
    [IsMinor]                BIT            NOT NULL,
    [PreferredLanguage]      NVARCHAR (25)  NOT NULL,
    [PreferredContactMethod] SMALLINT       NOT NULL,
    [Note]                   NVARCHAR (500) NULL,
    [HouseholdSize]          INT NULL,
    [HouseholdIncome]        INT NULL,
    [FirstContactDate]       DATE NULL,
    [ResidenceState]         NVARCHAR(2) NULL,
    [ReferralSourceId]       INT NULL,
    [HousingUnstable]        BIT NOT NULL   CONSTRAINT [DF_Caller_HousingUnstable] DEFAULT (0),
    [IsActive]               BIT            CONSTRAINT [DF_Caller_IsActive] DEFAULT (0x01) NOT NULL,
    [Created]                DATETIME       CONSTRAINT [DF_Caller_Created] DEFAULT (getutcdate()) NOT NULL,
    [Updated]                DATETIME       NULL,
    CONSTRAINT [FK_Caller_ReferralSource] FOREIGN KEY ([ReferralSourceId]) REFERENCES [dbo].[ReferralSource]([Id]),
    CONSTRAINT [PK_Caller] PRIMARY KEY CLUSTERED ([Id] ASC)
);

