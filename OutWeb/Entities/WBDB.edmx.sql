
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/02/2018 17:31:45
-- Generated from EDMX file: D:\Code\My\C741714_TCG\OutWeb\Entities\WBDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WBDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[WBAGENT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBAGENT];
GO
IF OBJECT_ID(N'[dbo].[WBLOGERR]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBLOGERR];
GO
IF OBJECT_ID(N'[dbo].[WBNEWS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBNEWS];
GO
IF OBJECT_ID(N'[dbo].[WBPIC]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBPIC];
GO
IF OBJECT_ID(N'[dbo].[WBPRODUCT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBPRODUCT];
GO
IF OBJECT_ID(N'[dbo].[WBPRODUCTTYPE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBPRODUCTTYPE];
GO
IF OBJECT_ID(N'[dbo].[WBUSR]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBUSR];
GO
IF OBJECT_ID(N'[dbo].[WBWORKS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WBWORKS];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'WBAGENT'
CREATE TABLE [dbo].[WBAGENT] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [AGENT_CONTENT] nvarchar(max)  NULL
);
GO

-- Creating table 'WBLOGERR'
CREATE TABLE [dbo].[WBLOGERR] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ERR_GID] int  NOT NULL,
    [ERR_SMRY] nvarchar(max)  NOT NULL,
    [ERR_DESC] nvarchar(max)  NOT NULL,
    [ERR_SRC] varchar(max)  NOT NULL,
    [LOG_DTM] datetime  NOT NULL
);
GO

-- Creating table 'WBNEWS'
CREATE TABLE [dbo].[WBNEWS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [BUD_ID] int  NULL,
    [BUD_DT] datetime  NULL,
    [UPD_ID] int  NULL,
    [UPD_DT] datetime  NULL,
    [NEWS_TITLE] nvarchar(200)  NULL,
    [PUB_DT] nvarchar(10)  NULL,
    [NEWS_CONTENT] nvarchar(max)  NULL,
    [LANG_CD] varchar(5)  NULL,
    [SR_SQ] int  NULL,
    [DIS_FRONT_ST] bit  NULL,
    [DIS_HOME_ST] bit  NULL
);
GO

-- Creating table 'WBPIC'
CREATE TABLE [dbo].[WBPIC] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [MAP_AC_NM] varchar(20)  NOT NULL,
    [IMG_NM] nvarchar(50)  NOT NULL,
    [IMG_URL] nvarchar(max)  NOT NULL,
    [IMG_LINK] nvarchar(max)  NOT NULL,
    [FILE_PATH] nvarchar(max)  NOT NULL,
    [UP_DT] datetime  NOT NULL,
    [UP_USR_ID] int  NOT NULL,
    [UP_MODE] nchar(10)  NOT NULL,
    [SR_SQ] int  NOT NULL,
    [MAP_NEWS_ID] int  NOT NULL,
    [MAP_PRODUCT_ID] int  NOT NULL,
    [MAP_WORKS_ID] int  NOT NULL
);
GO

-- Creating table 'WBPRODUCT'
CREATE TABLE [dbo].[WBPRODUCT] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [MAP_PRODUCT_TP_ID] int  NOT NULL,
    [BUD_USRID] int  NOT NULL,
    [BUD_DT] datetime  NOT NULL,
    [UPD_USRID] int  NOT NULL,
    [UPD_DT] datetime  NOT NULL,
    [PRD_NM] nvarchar(500)  NOT NULL,
    [PRD_CONTENT] nvarchar(max)  NOT NULL,
    [LANG_CD] varchar(5)  NOT NULL,
    [DIS_FRONT_ST] bit  NOT NULL,
    [SR_SQ] int  NOT NULL,
    [PRD_TP] nvarchar(500)  NULL,
    [PRD_SPE] nvarchar(500)  NULL,
    [PRD_MT] nvarchar(500)  NULL,
    [PRD_FEAT] nvarchar(500)  NULL
);
GO

-- Creating table 'WBUSR'
CREATE TABLE [dbo].[WBUSR] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SIGNIN_ID] nvarchar(50)  NOT NULL,
    [SIGNIN_PWD] nvarchar(50)  NOT NULL,
    [USR_NM] nvarchar(50)  NOT NULL,
    [USR_ENM] varchar(50)  NULL,
    [USR_EML] nvarchar(50)  NULL,
    [USR_GUID] char(36)  NOT NULL,
    [SIGNIN_DTM] datetime  NOT NULL,
    [BUD_USRID] int  NOT NULL,
    [BUD_DTM] datetime  NOT NULL,
    [UPD_USRID] int  NOT NULL,
    [UPD_DTM] datetime  NOT NULL
);
GO

-- Creating table 'WBWORKS'
CREATE TABLE [dbo].[WBWORKS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [BUD_USRID] int  NOT NULL,
    [BUD_DT] datetime  NOT NULL,
    [UPD_USRID] int  NOT NULL,
    [UPD_DT] datetime  NOT NULL,
    [WKS_TITLE] nvarchar(500)  NOT NULL,
    [WKS_CONTENT] nvarchar(max)  NOT NULL,
    [LANG_CD] varchar(5)  NOT NULL,
    [DIS_FRONT_ST] bit  NOT NULL,
    [SR_SQ] int  NOT NULL,
    [PUB_DT] nvarchar(10)  NULL
);
GO

-- Creating table 'WBPRODUCTTYPE'
CREATE TABLE [dbo].[WBPRODUCTTYPE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [LANG_CD] varchar(5)  NOT NULL,
    [PRD_TP_NM] nvarchar(50)  NOT NULL,
    [SR_SQ] int  NOT NULL,
    [PRD_TP_ST] char(1)  NOT NULL,
    [BUD_ID] int  NOT NULL,
    [BUD_DT] datetime  NOT NULL,
    [UPD_USR_ID] int  NULL,
    [UPD_DT] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'WBAGENT'
ALTER TABLE [dbo].[WBAGENT]
ADD CONSTRAINT [PK_WBAGENT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBLOGERR'
ALTER TABLE [dbo].[WBLOGERR]
ADD CONSTRAINT [PK_WBLOGERR]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBNEWS'
ALTER TABLE [dbo].[WBNEWS]
ADD CONSTRAINT [PK_WBNEWS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBPIC'
ALTER TABLE [dbo].[WBPIC]
ADD CONSTRAINT [PK_WBPIC]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBPRODUCT'
ALTER TABLE [dbo].[WBPRODUCT]
ADD CONSTRAINT [PK_WBPRODUCT]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBUSR'
ALTER TABLE [dbo].[WBUSR]
ADD CONSTRAINT [PK_WBUSR]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBWORKS'
ALTER TABLE [dbo].[WBWORKS]
ADD CONSTRAINT [PK_WBWORKS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WBPRODUCTTYPE'
ALTER TABLE [dbo].[WBPRODUCTTYPE]
ADD CONSTRAINT [PK_WBPRODUCTTYPE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------