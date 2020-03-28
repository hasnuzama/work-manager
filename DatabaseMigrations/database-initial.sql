-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.4.12-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for work_manager
CREATE DATABASE IF NOT EXISTS `work_manager` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `work_manager`;

-- Dumping structure for table work_manager.users
CREATE TABLE IF NOT EXISTS `users` (
  `user_id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `role` enum('admin','employee') DEFAULT 'employee',
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `email_UNIQUE` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- Dumping data for table work_manager.users: ~2 rows (approximately)
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`user_id`, `name`, `email`, `password`, `role`) VALUES
	(1, 'Test Employee', 'test1@gmail.com', '1afb69a1e528660b21feee8e734e0165', 'employee'),
	(2, 'Test Admin', 'test2@gmail.com', '1afb69a1e528660b21feee8e734e0165', 'admin');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;

-- Dumping structure for table work_manager.work_plans
CREATE TABLE IF NOT EXISTS `work_plans` (
  `work_plan_id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(10) unsigned NOT NULL,
  `work_date` date NOT NULL,
  `task_name` varchar(500) NOT NULL,
  `project_name` varchar(100) DEFAULT NULL,
  `estimated_hours` double(4,2) DEFAULT NULL,
  `pinestem_task_id` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`work_plan_id`),
  KEY `fk-users-work_plans-user_id_idx` (`user_id`),
  CONSTRAINT `fk-users-work_plans-user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table work_manager.work_plans: ~0 rows (approximately)
/*!40000 ALTER TABLE `work_plans` DISABLE KEYS */;
/*!40000 ALTER TABLE `work_plans` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
