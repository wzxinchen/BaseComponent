USE [ppdai]
GO

/****** Object:  Table [dbo].[user]    Script Date: 2015/6/24 10:29:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[user](
	[UserID] [int] NOT NULL,
	[UserName] [nvarchar](30) NOT NULL,
	[Password] [nvarchar](50) NULL,
	[Password2] [nvarchar](50) NULL,
	[Password3] [nvarchar](50) NULL,
	[Password4] [nvarchar](50) NULL,
	[Password5] [nvarchar](50) NULL,
	[Password6] [nvarchar](50) NULL,
	[Password7] [nvarchar](50) NULL,
	[Password8] [nvarchar](50) NULL,
	[Password9] [nvarchar](50) NULL,
	[Password0] [nvarchar](50) NULL,
	[Password11] [nvarchar](50) NULL,
	[Password12] [nvarchar](50) NULL,
	[Password13] [nvarchar](50) NULL,
	[Password14] [nvarchar](50) NULL,
	[Password15] [nvarchar](50) NULL,
	[Password16] [nvarchar](50) NULL,
	[Password17] [nvarchar](50) NULL,
	[Password18] [nvarchar](50) NULL,
	[Password19] [nvarchar](50) NULL,
	[Password21] [nvarchar](50) NULL,
	[Password32] [nvarchar](50) NULL,
	[Password43] [nvarchar](50) NULL,
	[Password54] [nvarchar](50) NULL,
	[Password65] [nvarchar](50) NULL,
	[Password76] [nvarchar](50) NULL,
	[Password87] [nvarchar](50) NULL,
	[Password98] [nvarchar](50) NULL,
	[Password09] [nvarchar](50) NULL,
	[Password111] [nvarchar](50) NULL,
	[Password122] [nvarchar](50) NULL,
	[Password133] [nvarchar](50) NULL,
	[Password144] [nvarchar](50) NULL,
	[Password155] [nvarchar](50) NULL,
	[Password166] [nvarchar](50) NULL,
	[Password177] [nvarchar](50) NULL,
	[Password188] [nvarchar](50) NULL,
	[Password199] [nvarchar](50) NULL,
	[Email] [nvarchar](256) NULL,
	[DisplayName] [nvarchar](20) NOT NULL,
	[Picture] [nvarchar](300) NULL,
	[LastLoginDate] [datetime] NULL,
	[Role] [int] NULL,
	[CreationDate] [datetime] NULL,
	[Dispark] [int] NULL,
	[BankAccount] [varchar](20) NULL,
	[BankType] [int] NULL,
	[BankBranchName] [nvarchar](50) NULL,
	[IsTemp] [bit] NULL,
	[LastUpdated] [datetime] NULL,
	[BankOtherType] [nvarchar](50) NULL,
	[Verifystatus] [int] NULL,
	[BorrowCredit] [float] NULL,
	[LendCredit] [float] NULL,
	[SpaceID] [int] NULL,
	[DefaultRisk] [tinyint] NULL,
	[LendRisk] [bit] NULL,
	[IsDelete] [int] NOT NULL,
	[ExecuteAdminID] [int] NOT NULL,
	[BankCity] [int] NULL,
	[Level] [tinyint] NULL,
	[CanCreateListDate] [datetime] NULL,
	[IsInpour] [bit] NULL,
	[flag_UsertoCEmail] [int] NULL,
	[materialScore] [int] NULL,
	[inserttime] [datetime] NULL,
	[updatetime] [datetime] NULL,
	[ISactive] [bit] NULL,
 CONSTRAINT [PK_User_back] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_User_back_LastLoginDate]  DEFAULT (getdate()) FOR [LastLoginDate]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_User_back_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_User_back_Dispark]  DEFAULT ((0)) FOR [Dispark]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__Verifystat__15FA39EE]  DEFAULT ((0)) FOR [Verifystatus]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__LendRisk__2E1BDC42]  DEFAULT ((0)) FOR [LendRisk]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__IsDelete__2F10007B]  DEFAULT ((0)) FOR [IsDelete]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__ExecuteAdm__300424B4]  DEFAULT ((0)) FOR [ExecuteAdminID]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__BankCity__30F848ED]  DEFAULT ((0)) FOR [BankCity]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__Level__31EC6D26]  DEFAULT ((0)) FOR [Level]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back__IsInpour__32E0915F]  DEFAULT ((0)) FOR [IsInpour]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_User_backtoCEmail]  DEFAULT ((0)) FOR [flag_UsertoCEmail]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF__User_back_materialSc__17A35695]  DEFAULT ((0)) FOR [materialScore]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_inserttime]  DEFAULT (getdate()) FOR [inserttime]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_updatetime]  DEFAULT (getdate()) FOR [updatetime]
GO

ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_ISactive]  DEFAULT ((1)) FOR [ISactive]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ＵＲＬ地址，如果修改图片的话，可以直接修改相应的ＵＲＬ地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'Picture'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上次登陆时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'LastLoginDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色
２通过ＥＭＡＩＬ
10,26纯借入
6纯借出
14，30借入，借出' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'Role'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'银行卡号(是否是取现的卡号?)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'BankAccount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'（不用）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'IsTemp'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'（不用）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'SpaceID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'VIP Level' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'COLUMN',@level2name=N'Level'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户基本信息表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user'
GO

