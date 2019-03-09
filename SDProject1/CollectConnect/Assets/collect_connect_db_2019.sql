CREATE DATABASE  IF NOT EXISTS `collect_connect` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `collect_connect`;
-- MySQL dump 10.13  Distrib 8.0.15, for macos10.14 (x86_64)
--
-- Host: localhost    Database: collect_connect
-- ------------------------------------------------------
-- Server version	8.0.15

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cards`
--

DROP TABLE IF EXISTS `cards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `cards` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `coll_id` int(7) NOT NULL,
  `name` varchar(8) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=59 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cards`
--

LOCK TABLES `cards` WRITE;
/*!40000 ALTER TABLE `cards` DISABLE KEYS */;
INSERT INTO `cards` VALUES (1,1,'1'),(2,1,'2'),(3,1,'3'),(4,1,'2'),(5,1,'4'),(6,1,'5'),(7,1,'6'),(8,1,'4'),(9,1,'7'),(10,1,'8'),(11,2,'1'),(12,2,'2'),(13,2,'3'),(14,2,'4'),(15,3,'1'),(16,3,'2'),(17,3,'3'),(18,3,'4'),(19,3,'5'),(20,3,'6'),(21,4,'1'),(22,4,'2'),(23,4,'3'),(24,4,'4'),(25,4,'5'),(26,4,'6'),(27,4,'7'),(28,4,'8'),(29,4,'9'),(30,4,'10'),(31,5,'1'),(32,5,'2'),(33,5,'3'),(34,5,'4'),(35,5,'5'),(36,5,'6'),(37,5,'7'),(38,5,'8'),(39,6,'1'),(40,6,'2'),(41,6,'3'),(42,6,'4'),(43,6,'5'),(44,6,'6'),(45,6,'7'),(46,6,'8'),(47,7,'1'),(48,7,'2'),(49,7,'3'),(50,7,'4'),(51,7,'5'),(52,7,'6'),(53,7,'7'),(54,7,'8'),(55,7,'9'),(56,8,'1'),(57,8,'2'),(58,8,'3');
/*!40000 ALTER TABLE `cards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `collections`
--

DROP TABLE IF EXISTS `collections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `collections` (
  `id` int(7) NOT NULL,
  `name` varchar(60) NOT NULL,
  `short_name` varchar(8) NOT NULL,
  `curator1_first` varchar(60) DEFAULT NULL,
  `curator1_last` varchar(60) DEFAULT NULL,
  `curator2_first` varchar(60) DEFAULT NULL,
  `curator2_last` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `collections`
--

LOCK TABLES `collections` WRITE;
/*!40000 ALTER TABLE `collections` DISABLE KEYS */;
INSERT INTO `collections` VALUES (1,'Household Poisoning','hgr','John','Posh',NULL,NULL),(2,'Italian Renaissance Ceramic','irc','Barbara','Alvarez',NULL,NULL),(3,'Hadith & Islamic Caligraphy','hic','Barbara','Alvarez',NULL,NULL),(4,'Formally Unusual Artists\' Books','fuab','Heidi','Kumao','Jamie','Vander Broek'),(5,'Board and Card Games','bcg','Bruce','Maxim',NULL,NULL),(6,'Art and Anthropology of Garbage','aag','David','Choberka',NULL,NULL),(7,'Apsara Warrior','aw','Christi','Merrill',NULL,NULL),(8,'Kelsey Roman Games','krg',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `collections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `connections`
--

DROP TABLE IF EXISTS `connections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `connections` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user1_id` int(11) NOT NULL,
  `user2_id` int(11) NOT NULL,
  `card1_id` int(11) NOT NULL,
  `card2_id` int(11) NOT NULL,
  `keyword1_id` int(11) NOT NULL,
  `keyword_match` int(1) NOT NULL,
  `keyword_match_rare` int(1) NOT NULL,
  `keyword_in_coll` int(1) NOT NULL,
  `time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `connections`
--

LOCK TABLES `connections` WRITE;
/*!40000 ALTER TABLE `connections` DISABLE KEYS */;
INSERT INTO `connections` VALUES (1,1,2,1,22,7,0,0,0,'2019-03-04 17:47:35'),(2,1,2,5,19,33,1,1,1,'2019-03-04 17:47:35');
/*!40000 ALTER TABLE `connections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `keywords_current`
--

DROP TABLE IF EXISTS `keywords_current`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `keywords_current` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `coll_id` int(7) NOT NULL,
  `rare` int(1) NOT NULL,
  `time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=129 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `keywords_current`
--

LOCK TABLES `keywords_current` WRITE;
/*!40000 ALTER TABLE `keywords_current` DISABLE KEYS */;
INSERT INTO `keywords_current` VALUES (1,'deadly',1,1,'2019-03-04 16:56:55'),(2,'domesticated',1,1,'2019-03-04 16:56:55'),(3,'toxic',1,1,'2019-03-04 16:56:55'),(4,'plagued',1,1,'2019-03-04 16:56:55'),(5,'decorative',1,1,'2019-03-04 16:56:55'),(6,'forensic',1,1,'2019-03-04 16:56:55'),(7,'liminality',1,1,'2019-03-04 16:56:55'),(8,'contamination',1,0,'2019-03-04 16:56:55'),(9,'poisonous',1,0,'2019-03-04 16:56:55'),(10,'health',1,0,'2019-03-04 16:56:55'),(11,'epidemic',1,0,'2019-03-04 16:56:55'),(12,'sickness',1,0,'2019-03-04 16:56:55'),(13,'danger',1,0,'2019-03-04 16:56:55'),(14,'household',1,0,'2019-03-04 16:56:55'),(15,'hazardous',1,0,'2019-03-04 16:56:55'),(16,'face',2,1,'2019-03-04 16:56:55'),(17,'grotesque',2,1,'2019-03-04 16:56:55'),(18,'roundness',2,1,'2019-03-04 16:56:55'),(19,'gifted',2,1,'2019-03-04 16:56:55'),(20,'fragile',2,1,'2019-03-04 16:56:55'),(21,'breakable',2,1,'2019-03-04 16:56:55'),(22,'serve',2,1,'2019-03-04 16:56:55'),(23,'portrait',2,0,'2019-03-04 16:56:55'),(24,'clay',2,0,'2019-03-04 16:56:55'),(25,'circle',2,0,'2019-03-04 16:56:55'),(26,'translation',3,1,'2019-03-04 16:56:55'),(27,'Muslim',3,1,'2019-03-04 16:56:55'),(28,'Islamic',3,1,'2019-03-04 16:56:55'),(29,'Arabic',3,1,'2019-03-04 16:56:55'),(30,'authority',3,1,'2019-03-04 16:56:55'),(31,'non-roman',3,1,'2019-03-04 17:05:43'),(32,'guide',3,1,'2019-03-04 17:05:43'),(33,'direction',3,1,'2019-03-04 17:05:43'),(34,'en-face',3,1,'2019-03-04 17:05:43'),(35,'inked',3,1,'2019-03-04 17:05:43'),(36,'tradition',3,0,'2019-03-04 17:05:43'),(37,'moral',3,0,'2019-03-04 17:05:43'),(38,'religious',3,0,'2019-03-04 17:05:43'),(39,'law',3,0,'2019-03-04 17:05:43'),(40,'wisdom',3,0,'2019-03-04 17:05:43'),(41,'Gandhi',3,0,'2019-03-04 17:05:43'),(42,'bliss',3,0,'2019-03-04 17:05:43'),(43,'gardening',3,0,'2019-03-04 17:05:43'),(44,'sayings',3,0,'2019-03-04 17:05:43'),(45,'calligraphy',3,0,'2019-03-04 17:05:43'),(46,'multidimensional',4,1,'2019-03-04 17:05:43'),(47,'hand-colored',4,1,'2019-03-04 17:05:43'),(48,'constructed',4,1,'2019-03-04 17:05:43'),(49,'flat',4,1,'2019-03-04 17:05:43'),(50,'model',4,1,'2019-03-04 17:05:43'),(51,'kit',4,1,'2019-03-04 17:05:43'),(52,'intertextual',4,1,'2019-03-04 17:05:43'),(53,'hidden',4,1,'2019-03-04 17:05:43'),(54,'interiors',4,1,'2019-03-04 17:05:43'),(55,'milky',4,1,'2019-03-04 17:05:43'),(56,'graphic',4,1,'2019-03-04 17:05:43'),(57,'mixed',4,1,'2019-03-04 17:05:43'),(58,'box',4,0,'2019-03-04 17:05:43'),(59,'fiber',4,0,'2019-03-04 17:05:43'),(60,'textile',4,0,'2019-03-04 17:06:54'),(61,'scroll',4,0,'2019-03-04 17:06:54'),(62,'handmade',4,0,'2019-03-04 17:06:54'),(63,'carousel',4,0,'2019-03-04 17:06:54'),(64,'structured',4,0,'2019-03-04 17:06:54'),(65,'pop-up',4,0,'2019-03-04 17:06:54'),(66,'3D',4,0,'2019-03-04 17:06:54'),(67,'layers',4,0,'2019-03-04 17:06:54'),(68,'reveal',4,0,'2019-03-04 17:06:54'),(69,'craft',4,0,'2019-03-04 17:06:54'),(70,'playful',5,1,'2019-03-04 17:06:54'),(71,'unbreakable',5,1,'2019-03-04 17:06:54'),(72,'turns',5,1,'2019-03-04 17:06:54'),(73,'together',5,1,'2019-03-04 17:06:54'),(74,'war',5,1,'2019-03-04 17:06:54'),(75,'categorically',5,1,'2019-03-04 17:06:54'),(76,'hand',5,1,'2019-03-04 17:06:54'),(77,'lettered',5,1,'2019-03-04 17:06:54'),(78,'wordless',5,1,'2019-03-04 17:06:54'),(79,'illustrative',5,1,'2019-03-04 17:06:54'),(80,'viral',5,1,'2019-03-04 17:06:54'),(81,'against',5,0,'2019-03-04 17:06:54'),(82,'humanity',5,0,'2019-03-04 17:06:54'),(83,'music',5,0,'2019-03-04 17:06:54'),(84,'pieces',5,0,'2019-03-04 17:06:54'),(85,'pawns',5,0,'2019-03-04 17:06:54'),(86,'tiles',5,0,'2019-03-04 17:06:54'),(87,'pandemic',5,0,'2019-03-04 17:06:54'),(88,'fun',5,0,'2019-03-04 17:06:54'),(89,'multiplayer',5,0,'2019-03-04 17:06:54'),(90,'contained',6,1,'2019-03-04 17:08:35'),(91,'mirroring',6,1,'2019-03-04 17:08:35'),(92,'figurative',6,1,'2019-03-04 17:08:35'),(93,'cityscape',6,1,'2019-03-04 17:08:35'),(94,'sung',6,1,'2019-03-04 17:08:35'),(95,'incarcerated',6,1,'2019-03-04 17:08:35'),(96,'loopy',6,1,'2019-03-04 17:08:35'),(97,'junk',6,1,'2019-03-04 17:08:35'),(98,'bodily',6,1,'2019-03-04 17:08:35'),(99,'concentration',6,1,'2019-03-04 17:08:35'),(100,'industrial',6,1,'2019-03-04 17:08:35'),(101,'keys',6,1,'2019-03-04 17:08:35'),(102,'assemblage',6,0,'2019-03-04 17:08:35'),(103,'camp',6,0,'2019-03-04 17:08:35'),(104,'urban',6,0,'2019-03-04 17:08:35'),(105,'street',6,0,'2019-03-04 17:08:35'),(106,'2D',6,0,'2019-03-04 17:08:35'),(107,'celestial',7,1,'2019-03-04 17:08:35'),(108,'cosmopolis',7,1,'2019-03-04 17:08:35'),(109,'explosive',7,1,'2019-03-04 17:08:35'),(110,'repurposed',7,1,'2019-03-04 17:08:35'),(111,'reused',7,1,'2019-03-04 17:08:35'),(112,'decommissioned',7,1,'2019-03-04 17:08:35'),(113,'retold',7,1,'2019-03-04 17:08:35'),(114,'nearly-divine',7,1,'2019-03-04 17:08:35'),(115,'framed',7,1,'2019-03-04 17:08:35'),(116,'hierarchy',7,1,'2019-03-04 17:08:35'),(117,'Apsara',7,0,'2019-03-04 17:08:35'),(118,'Sanskrit',7,0,'2019-03-04 17:08:35'),(119,'language',7,0,'2019-03-04 17:08:35'),(120,'Cambodia',7,0,'2019-03-04 17:09:35'),(121,'dancer',7,0,'2019-03-04 17:09:35'),(122,'animal',8,1,'2019-03-04 17:09:35'),(123,'gamble',8,1,'2019-03-04 17:09:35'),(124,'irregular',8,1,'2019-03-04 17:09:35'),(125,'excavation',8,1,'2019-03-04 17:09:35'),(126,'toys',8,0,'2019-03-04 17:09:35'),(127,'inscribe',8,0,'2019-03-04 17:09:35'),(128,'illegal',8,0,'2019-03-04 17:09:35');
/*!40000 ALTER TABLE `keywords_current` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `keywords_log`
--

DROP TABLE IF EXISTS `keywords_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `keywords_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `coll_id` int(7) NOT NULL,
  `rare` int(1) NOT NULL,
  `time` datetime NOT NULL,
  `notes` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=129 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `keywords_log`
--

LOCK TABLES `keywords_log` WRITE;
/*!40000 ALTER TABLE `keywords_log` DISABLE KEYS */;
INSERT INTO `keywords_log` VALUES (1,'deadly',1,1,'2019-03-04 17:12:49',NULL),(2,'domesticated',1,1,'2019-03-04 17:12:49',NULL),(3,'toxic',1,1,'2019-03-04 17:12:49',NULL),(4,'plagued',1,1,'2019-03-04 17:12:50',NULL),(5,'decorative',1,1,'2019-03-04 17:12:50',NULL),(6,'forensic',1,1,'2019-03-04 17:12:50',NULL),(7,'liminality',1,1,'2019-03-04 17:12:50',NULL),(8,'contamination',1,0,'2019-03-04 17:12:50',NULL),(9,'poisonous',1,0,'2019-03-04 17:12:50',NULL),(10,'health',1,0,'2019-03-04 17:12:50',NULL),(11,'epidemic',1,0,'2019-03-04 17:12:50',NULL),(12,'sickness',1,0,'2019-03-04 17:12:50',NULL),(13,'danger',1,0,'2019-03-04 17:12:50',NULL),(14,'household',1,0,'2019-03-04 17:12:50',NULL),(15,'hazardous',1,0,'2019-03-04 17:12:50',NULL),(16,'face',2,1,'2019-03-04 17:12:50',NULL),(17,'grotesque',2,1,'2019-03-04 17:12:50',NULL),(18,'roundness',2,1,'2019-03-04 17:12:50',NULL),(19,'gifted',2,1,'2019-03-04 17:12:50',NULL),(20,'fragile',2,1,'2019-03-04 17:12:50',NULL),(21,'breakable',2,1,'2019-03-04 17:12:50',NULL),(22,'serve',2,1,'2019-03-04 17:12:50',NULL),(23,'portrait',2,0,'2019-03-04 17:12:50',NULL),(24,'clay',2,0,'2019-03-04 17:12:50',NULL),(25,'circle',2,0,'2019-03-04 17:12:50',NULL),(26,'translation',3,1,'2019-03-04 17:12:50',NULL),(27,'Muslim',3,1,'2019-03-04 17:12:50',NULL),(28,'Islamic',3,1,'2019-03-04 17:12:50',NULL),(29,'Arabic',3,1,'2019-03-04 17:12:50',NULL),(30,'authority',3,1,'2019-03-04 17:12:50',NULL),(31,'non-roman',3,1,'2019-03-04 17:13:04',NULL),(32,'guide',3,1,'2019-03-04 17:13:04',NULL),(33,'direction',3,1,'2019-03-04 17:13:04',NULL),(34,'en-face',3,1,'2019-03-04 17:13:04',NULL),(35,'inked',3,1,'2019-03-04 17:13:04',NULL),(36,'tradition',3,0,'2019-03-04 17:13:04',NULL),(37,'moral',3,0,'2019-03-04 17:13:04',NULL),(38,'religious',3,0,'2019-03-04 17:13:04',NULL),(39,'law',3,0,'2019-03-04 17:13:04',NULL),(40,'wisdom',3,0,'2019-03-04 17:13:04',NULL),(41,'Gandhi',3,0,'2019-03-04 17:13:04',NULL),(42,'bliss',3,0,'2019-03-04 17:13:04',NULL),(43,'gardening',3,0,'2019-03-04 17:13:04',NULL),(44,'sayings',3,0,'2019-03-04 17:13:04',NULL),(45,'calligraphy',3,0,'2019-03-04 17:13:04',NULL),(46,'multidimensional',4,1,'2019-03-04 17:13:04',NULL),(47,'hand-colored',4,1,'2019-03-04 17:13:04',NULL),(48,'constructed',4,1,'2019-03-04 17:13:04',NULL),(49,'flat',4,1,'2019-03-04 17:13:04',NULL),(50,'model',4,1,'2019-03-04 17:13:04',NULL),(51,'kit',4,1,'2019-03-04 17:13:04',NULL),(52,'intertextual',4,1,'2019-03-04 17:13:04',NULL),(53,'hidden',4,1,'2019-03-04 17:13:04',NULL),(54,'interiors',4,1,'2019-03-04 17:13:04',NULL),(55,'milky',4,1,'2019-03-04 17:13:04',NULL),(56,'graphic',4,1,'2019-03-04 17:13:04',NULL),(57,'mixed',4,1,'2019-03-04 17:13:04',NULL),(58,'box',4,0,'2019-03-04 17:13:04',NULL),(59,'fiber',4,0,'2019-03-04 17:13:04',NULL),(60,'textile',4,0,'2019-03-04 17:13:04',NULL),(61,'scroll',4,0,'2019-03-04 17:13:26',NULL),(62,'handmade',4,0,'2019-03-04 17:13:26',NULL),(63,'carousel',4,0,'2019-03-04 17:13:26',NULL),(64,'structured',4,0,'2019-03-04 17:13:26',NULL),(65,'pop-up',4,0,'2019-03-04 17:13:26',NULL),(66,'3D',4,0,'2019-03-04 17:13:26',NULL),(67,'layers',4,0,'2019-03-04 17:13:26',NULL),(68,'reveal',4,0,'2019-03-04 17:13:26',NULL),(69,'craft',4,0,'2019-03-04 17:13:26',NULL),(70,'playful',5,1,'2019-03-04 17:13:26',NULL),(71,'unbreakable',5,1,'2019-03-04 17:13:26',NULL),(72,'turns',5,1,'2019-03-04 17:13:26',NULL),(73,'together',5,1,'2019-03-04 17:13:26',NULL),(74,'war',5,1,'2019-03-04 17:13:26',NULL),(75,'categorically',5,1,'2019-03-04 17:13:26',NULL),(76,'hand',5,1,'2019-03-04 17:13:26',NULL),(77,'lettered',5,1,'2019-03-04 17:13:26',NULL),(78,'wordless',5,1,'2019-03-04 17:13:26',NULL),(79,'illustrative',5,1,'2019-03-04 17:13:26',NULL),(80,'viral',5,1,'2019-03-04 17:13:26',NULL),(81,'against',5,0,'2019-03-04 17:13:26',NULL),(82,'humanity',5,0,'2019-03-04 17:13:26',NULL),(83,'music',5,0,'2019-03-04 17:13:26',NULL),(84,'pieces',5,0,'2019-03-04 17:13:26',NULL),(85,'pawns',5,0,'2019-03-04 17:13:26',NULL),(86,'tiles',5,0,'2019-03-04 17:13:26',NULL),(87,'pandemic',5,0,'2019-03-04 17:13:26',NULL),(88,'fun',5,0,'2019-03-04 17:13:26',NULL),(89,'multiplayer',5,0,'2019-03-04 17:13:26',NULL),(90,'contained',6,1,'2019-03-04 17:13:26',NULL),(91,'mirroring',6,1,'2019-03-04 17:13:43',NULL),(92,'figurative',6,1,'2019-03-04 17:13:43',NULL),(93,'cityscape',6,1,'2019-03-04 17:13:43',NULL),(94,'sung',6,1,'2019-03-04 17:13:43',NULL),(95,'incarcerated',6,1,'2019-03-04 17:13:43',NULL),(96,'loopy',6,1,'2019-03-04 17:13:43',NULL),(97,'junk',6,1,'2019-03-04 17:13:43',NULL),(98,'bodily',6,1,'2019-03-04 17:13:43',NULL),(99,'concentration',6,1,'2019-03-04 17:13:43',NULL),(100,'industrial',6,1,'2019-03-04 17:13:43',NULL),(101,'keys',6,1,'2019-03-04 17:13:43',NULL),(102,'assemblage',6,0,'2019-03-04 17:13:43',NULL),(103,'camp',6,0,'2019-03-04 17:13:43',NULL),(104,'urban',6,0,'2019-03-04 17:13:43',NULL),(105,'street',6,0,'2019-03-04 17:13:43',NULL),(106,'2D',6,0,'2019-03-04 17:13:43',NULL),(107,'celestial',7,1,'2019-03-04 17:13:43',NULL),(108,'cosmopolis',7,1,'2019-03-04 17:13:43',NULL),(109,'explosive',7,1,'2019-03-04 17:13:43',NULL),(110,'repurposed',7,1,'2019-03-04 17:13:43',NULL),(111,'reused',7,1,'2019-03-04 17:13:43',NULL),(112,'decommissioned',7,1,'2019-03-04 17:13:43',NULL),(113,'retold',7,1,'2019-03-04 17:13:43',NULL),(114,'nearly-divine',7,1,'2019-03-04 17:13:43',NULL),(115,'framed',7,1,'2019-03-04 17:13:43',NULL),(116,'hierarchy',7,1,'2019-03-04 17:13:43',NULL),(117,'Apsara',7,0,'2019-03-04 17:13:43',NULL),(118,'Sanskrit',7,0,'2019-03-04 17:13:43',NULL),(119,'language',7,0,'2019-03-04 17:13:43',NULL),(120,'Cambodia',7,0,'2019-03-04 17:13:57',NULL),(121,'dancer',7,0,'2019-03-04 17:13:57',NULL),(122,'animal',8,1,'2019-03-04 17:13:57',NULL),(123,'gamble',8,1,'2019-03-04 17:13:57',NULL),(124,'irregular',8,1,'2019-03-04 17:13:57',NULL),(125,'excavation',8,1,'2019-03-04 17:13:57',NULL),(126,'toys',8,0,'2019-03-04 17:13:57',NULL),(127,'inscribe',8,0,'2019-03-04 17:13:57',NULL),(128,'illegal',8,0,'2019-03-04 17:13:57',NULL);
/*!40000 ALTER TABLE `keywords_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(45) NOT NULL,
  `user_first` varchar(45) NOT NULL,
  `user_last` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'jposh_test','John','Posh'),(2,'cmerrill_test','Christi','Merrill');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-03-06 17:32:01
