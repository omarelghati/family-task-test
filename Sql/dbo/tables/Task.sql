CREATE TABLE [dbo].[Task] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Subject] VARCHAR(255) NOT NULL,
    [IsComplete] bit NOT NULL,
    [AssignedMemberId] UNIQUEIDENTIFIER NULL,
    FOREIGN KEY ([AssignedMemberId]) REFERENCES [dbo].[Member] ([Id])
)