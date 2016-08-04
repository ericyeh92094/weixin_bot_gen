CREATE TABLE [dbo].[Messages] (
    [Id]               INT       NOT NULL,
    [MessageID]        TEXT      NULL,
    [FromUserName]     TEXT      NULL,
    [CreateTimeWeChat] TEXT NULL,
    [MsgType]          TEXT      NULL,
    [ContentWeChat]    XML       NULL,
    [Out]              BIT       DEFAULT ((0)) NULL,
    [URL] TEXT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

