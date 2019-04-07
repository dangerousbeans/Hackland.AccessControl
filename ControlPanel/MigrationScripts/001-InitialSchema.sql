-- MySQL dump 10.13  Distrib 5.6.23, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: accesscontrol
-- ------------------------------------------------------
-- Server version	5.6.25-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

/*LOCK TABLES `UserToken` WRITE , `DoorRead` WRITE, `PersonDoor` WRITE, `Door` WRITE, `Person` WRITE, `RoleClaim` WRITE, `UserLogin` WRITE, `UserRole` WRITE, `Role` WRITE, `User` WRITE;*/

DROP TABLE IF EXISTS `UserToken`;
DROP TABLE IF EXISTS `DoorRead`;
DROP TABLE IF EXISTS `PersonDoor`;
DROP TABLE IF EXISTS `Door`;
DROP TABLE IF EXISTS `Person`;

DROP TABLE IF EXISTS `RoleClaim`;
DROP TABLE IF EXISTS `UserClaim`;
DROP TABLE IF EXISTS `UserLogin`;
DROP TABLE IF EXISTS `UserRole`;
DROP TABLE IF EXISTS `Role`;
DROP TABLE IF EXISTS `User`;

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Role` (
  `Id` int(11) NOT NULL,
  `Name` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NormalizedName` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `RoleClaim`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `RoleClaim` (
  `Id` int(11) NOT NULL,
  `RoleId` int(11) NOT NULL,
  `ClaimType` longtext COLLATE utf8mb4_unicode_ci,
  `ClaimValue` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_RoleClaim_RoleId` (`RoleId`),
  CONSTRAINT `FK_RoleClaim_Role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `User`
--


/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `User` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NormalizedUserName` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NormalizedEmail` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `EmailConfirmed` bit(1) NOT NULL,
  `PasswordHash` longtext COLLATE utf8mb4_unicode_ci,
  `SecurityStamp` longtext COLLATE utf8mb4_unicode_ci,
  `ConcurrencyStamp` longtext COLLATE utf8mb4_unicode_ci,
  `PhoneNumber` longtext COLLATE utf8mb4_unicode_ci,
  `PhoneNumberConfirmed` bit(1) NOT NULL,
  `TwoFactorEnabled` bit(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` bit(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `FirstName` longtext COLLATE utf8mb4_unicode_ci,
  `LastName` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `User`
--

LOCK TABLES `User` WRITE;
/*!40000 ALTER TABLE `User` DISABLE KEYS */;
INSERT INTO `User` VALUES (1, 'agrath@hackland.nz','AGRATH@HACKLAND.NZ',NULL,NULL,'\0','AQAAAAEAACcQAAAAEHqiD8WK3yxG41chU9Im3m/atGwUor471sD2+5s3y0Zw1fXQ+HlJwrvmIzmf325kow==','GTBHYMELIAJYGBFGMDAZ2QS3XCC46A2Y','9fbe66d1-b601-4c8a-ae27-9c7a71d3330d',NULL,'\0','\0',NULL,'',0,NULL,NULL);
/*!40000 ALTER TABLE `User` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `UserClaim`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserClaim` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `ClaimType` longtext COLLATE utf8mb4_unicode_ci,
  `ClaimValue` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_UserClaim_UserId` (`UserId`),
  CONSTRAINT `FK_UserClaim_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `UserLogin`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserLogin` (
  `LoginProvider` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProviderKey` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProviderDisplayName` longtext COLLATE utf8mb4_unicode_ci,
  `UserId` int(11) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_UserLogin_UserId` (`UserId`),
  CONSTRAINT `FK_UserLogin_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `UserRole`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserRole` (
  `UserId` int(11) NOT NULL,
  `RoleId` int(11) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_UserRole_RoleId` (`RoleId`),
  CONSTRAINT `FK_UserRole_Role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserRole_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `UserToken`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `UserToken` (
  `UserId` int(11) NOT NULL,
  `LoginProvider` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Name` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`UserId`),
  CONSTRAINT `FK_UserToken_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Door`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Door` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `MacAddress` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `LastHeartbeatTimestamp` datetime(6) NOT NULL,
  `LastReadTimestamp` datetime(6) DEFAULT NULL,
  `Status` int(11) NOT NULL,
  `IsDeleted` smallint(6) NOT NULL DEFAULT '0',
  `CreatedTimestamp` datetime(6) NOT NULL,
  `CreatedByUserId` int(11) NULL,
  `UpdatedTimestamp` datetime(6) DEFAULT NULL,
  `UpdatedByUserId` int(11) NULL,
  `RemoteUnlockRequestSeconds` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Door_CreatedUser` (`CreatedByUserId`),
  KEY `FK_Door_UpdatedUser` (`UpdatedByUserId`),
  CONSTRAINT `FK_Door_CreatedUser` FOREIGN KEY (`CreatedByUserId`) REFERENCES `User` (`Id`),
  CONSTRAINT `FK_Door_UpdatedUser` FOREIGN KEY (`UpdatedByUserId`) REFERENCES `User` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `DoorRead`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `DoorRead` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TokenValue` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `Timestamp` datetime(6) DEFAULT NULL,
  `DoorId` int(11) NOT NULL,
  `PersonId` int(11) DEFAULT NULL,
  `IsSuccess` smallint(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_DoorRead_Door` (`DoorId`),
  KEY `FK_DoorRead_Person` (`PersonId`),
  CONSTRAINT `FK_DoorRead_Door` FOREIGN KEY (`DoorId`) REFERENCES `Door` (`Id`),
  CONSTRAINT `FK_DoorRead_Person` FOREIGN KEY (`PersonId`) REFERENCES `Person` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Person`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Person` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `EmailAddress` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhoneNumber` longtext COLLATE utf8mb4_unicode_ci,
  `CreatedTimestamp` datetime(6) NOT NULL,
  `CreatedByUserId` int(11) NOT NULL,
  `UpdatedTimestamp` datetime(6) DEFAULT NULL,
  `UpdatedByUserId` int(11) NULL,
  `LastSeenTimestamp` datetime(6) DEFAULT NULL,
  `IsDeleted` smallint(6) NOT NULL,
  `TokenValue` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_Person_CreatedUser` (`CreatedByUserId`),
  KEY `FK_Person_UpdatedUser` (`UpdatedByUserId`),
  CONSTRAINT `FK_Person_CreatedUser` FOREIGN KEY (`CreatedByUserId`) REFERENCES `User` (`Id`),
  CONSTRAINT `FK_Person_UpdatedUser` FOREIGN KEY (`UpdatedByUserId`) REFERENCES `User` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `PersonDoor`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `PersonDoor` (
  `PersonId` int(11) NOT NULL,
  `DoorId` int(11) NOT NULL,
  `CreatedTimestamp` datetime(6) NOT NULL,
  `CreatedByUserId` int(11) NOT NULL,
  `UpdatedTimestamp` datetime(6) DEFAULT NULL,
  `UpdatedByUserId` int(11) NULL,
  `IsDeleted` smallint(6) NOT NULL,
  PRIMARY KEY (`PersonId`,`DoorId`),
  KEY `FK_PersonDoor_CreatedUser` (`CreatedByUserId`),
  KEY `FK_PersonDoor_UpdatedUser` (`UpdatedByUserId`),
  KEY `FK_PersonDoor_Door` (`DoorId`),
  CONSTRAINT `FK_PersonDoor_CreatedUser` FOREIGN KEY (`CreatedByUserId`) REFERENCES `User` (`Id`),
  CONSTRAINT `FK_PersonDoor_Door` FOREIGN KEY (`DoorId`) REFERENCES `Door` (`Id`),
  CONSTRAINT `FK_PersonDoor_Person` FOREIGN KEY (`PersonId`) REFERENCES `Person` (`Id`),
  CONSTRAINT `FK_PersonDoor_UpdatedUser` FOREIGN KEY (`UpdatedByUserId`) REFERENCES `User` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-04-07 10:28:48
