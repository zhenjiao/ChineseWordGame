-- MySQL dump 10.13  Distrib 5.6.10, for Win64 (x86_64)
--
-- Host: localhost    Database: game_db
-- ------------------------------------------------------
-- Server version	5.6.10

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

DROP DATABASE IF EXISTS game_db;

CREATE DATABASE game_db;

use game_db;
--
-- Table structure for table `game_result`
--

DROP TABLE IF EXISTS `game_result`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `game_result` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `game_id` int(11) NOT NULL,
  `level` int(11),
  `username` varchar(256) NOT NULL DEFAULT '',
  `score` int(11) NOT NULL DEFAULT '0',
  `duration` int(11),
  `inserted_datetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `id_idx` (`game_id`),
  KEY `score_idx` (`score`),
  KEY `level_idx` (`level`),
  KEY `duration_idx` (`duration`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `game_type`
--

DROP TABLE IF EXISTS `game_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `game_type` (
  `game_id` int(11) NOT NULL AUTO_INCREMENT,
  `game_name` varchar(64) NOT NULL,
  PRIMARY KEY (`game_id`),
  UNIQUE KEY (`game_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `game_type`
--

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

DROP PROCEDURE IF EXISTS addGameType;

delimiter //
CREATE PROCEDURE addGameType( in game_name varchar(256), out game_id int)
begin
	declare exit handler for 1062 set game_id = -1;
	
	insert into game_type (game_name)
	values (game_name);
	
	set game_id = last_insert_id();
end
//
delimiter ;

DROP PROCEDURE IF EXISTS addGameResult;

delimiter //
CREATE PROCEDURE addGameResult( in game_name varchar(256), in username varchar(256), in score int, in level int, in duration int, out rank int)
label_start:
begin
	declare game_id int;
	select gt.game_id into game_id from game_type gt
	where gt.game_name = game_name;
	
	if (game_id is null) then
		set rank = -1;
		leave label_start;
	end if;
	
	insert into game_result (game_id, level, username, score, duration, inserted_datetime) values
	(game_id, level, username, score, duration, CURRENT_TIMESTAMP ());

	select count(*) into rank 
	from game_result r
	where r.game_id = game_id
	and	((r.score > score)
	or (r.score = score 
	and r.level > level)
	or (r.score = score 
	and r.level = level 
	and r.duration < duration));

	set rank = rank + 1;
end label_start;
//
delimiter ;

DROP PROCEDURE IF EXISTS removeGameResultsAfterRank;

delimiter //
CREATE PROCEDURE removeGameResultsAfterRank( in game_id int, in afterRank int, out removedCount int)
begin
	declare totalCount int;
	declare removingCount int;
	
	select count(*) into totalCount
	from game_result where game_id = game_id;
	
	if (totalCount > afterRank) then
		set removingCount = totalCount - afterRank;
		delete from game_result where score in (select t.score from (select * from game_result r where r.game_id=game_id order by score limit removingCount) as t);
		set removedCount = row_count();
	else
		set removedCount = 0;
	end if;
end
//
delimiter ;

call addGameType('ChineseWordGame v1.0', @gameid);
select @gameid;

call addGameResult('aaa', 'test', 100, 1, 1000, @rank);
select @rank;
call addGameResult('ChineseWordGame v1.0', 'test', 100, 1, 1000, @rank);
select @rank;
call addGameResult('ChineseWordGame v1.0', 'test', 100, 1, 2000, @rank);
select @rank;
call addGameResult('ChineseWordGame v1.0', 'test', 1000, 10, 1000, @rank);
select @rank;
call addGameResult('ChineseWordGame v1.0', 'test', 500, 5, 1000, @rank);
select @rank;

call removeGameResultsAfterRank(1, 3, @removedCount);
select @removedCount;
-- Dump completed on 2013-05-15 22:41:49
