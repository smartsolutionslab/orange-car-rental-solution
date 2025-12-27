-- Eventuous SQL Server Event Store Schema
-- This schema is created automatically when InitializeDatabase=true
-- This file is provided for reference and manual migrations

-- Create schema if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'events')
BEGIN
    EXEC('CREATE SCHEMA [events]')
END
GO

-- Streams table - tracks stream versions for optimistic concurrency
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Streams' AND schema_id = SCHEMA_ID('events'))
BEGIN
    CREATE TABLE [events].[Streams] (
        [StreamId] NVARCHAR(200) NOT NULL,
        [StreamName] NVARCHAR(200) NOT NULL,
        [StreamVersion] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Streams] PRIMARY KEY CLUSTERED ([StreamId])
    )

    CREATE UNIQUE INDEX [IX_Streams_StreamName] ON [events].[Streams] ([StreamName])
END
GO

-- Messages table - stores all events
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages' AND schema_id = SCHEMA_ID('events'))
BEGIN
    CREATE TABLE [events].[Messages] (
        [MessageId] UNIQUEIDENTIFIER NOT NULL,
        [MessageType] NVARCHAR(256) NOT NULL,
        [StreamId] NVARCHAR(200) NOT NULL,
        [StreamPosition] INT NOT NULL,
        [GlobalPosition] BIGINT IDENTITY(1,1) NOT NULL,
        [JsonData] NVARCHAR(MAX) NOT NULL,
        [JsonMetadata] NVARCHAR(MAX) NULL,
        [Created] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([GlobalPosition]),
        CONSTRAINT [FK_Messages_Streams] FOREIGN KEY ([StreamId]) REFERENCES [events].[Streams]([StreamId])
    )

    CREATE UNIQUE INDEX [IX_Messages_StreamId_StreamPosition] ON [events].[Messages] ([StreamId], [StreamPosition])
    CREATE INDEX [IX_Messages_MessageType] ON [events].[Messages] ([MessageType])
    CREATE INDEX [IX_Messages_Created] ON [events].[Messages] ([Created])
END
GO

-- Checkpoints table - tracks subscription checkpoint positions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Checkpoints' AND schema_id = SCHEMA_ID('events'))
BEGIN
    CREATE TABLE [events].[Checkpoints] (
        [SubscriptionId] NVARCHAR(200) NOT NULL,
        [Position] BIGINT NOT NULL DEFAULT 0,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Checkpoints] PRIMARY KEY CLUSTERED ([SubscriptionId])
    )
END
GO

-- Grant permissions (adjust as needed for your environment)
-- GRANT SELECT, INSERT, UPDATE ON SCHEMA::[events] TO [YourAppUser]
-- GO
