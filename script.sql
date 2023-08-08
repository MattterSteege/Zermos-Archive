USE `REDACTED_DATABASE_NAME`;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;

CREATE TABLE `users` (
    `email` varchar(255) NOT NULL,
    `name` longtext NULL,
    `school_id` longtext NULL,
    `zermelo_access_token` longtext NULL,
    `zermelo_access_token_expires_at` datetime(6) NULL,
    `somtoday_access_token` longtext NULL,
    `somtoday_refresh_token` longtext NULL,
    `somtoday_student_id` longtext NULL,
    `infowijs_access_token` longtext NULL,
    `theme` longtext NULL,
    `VerificationToken` longtext NULL,
    `CreatedAt` datetime(6) NULL,
    `VerifiedAt` datetime(6) NULL,
    PRIMARY KEY (`email`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230707141354_Initial', '7.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE `users` ADD `custom_huiswerk` json NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230707141705_custom homework', '7.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE `users` ADD `cached_somtoday_grades` longtext NULL;

ALTER TABLE `users` ADD `cached_somtoday_homework` longtext NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230719181633_somtoday_cache', '7.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE `users` ADD `cached_infowijs_calendar` longtext NULL;

ALTER TABLE `users` ADD `cached_infowijs_news` longtext NULL;

ALTER TABLE `users` ADD `cached_school_informationscreen` longtext NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230720123910_more_cache', '7.0.8');

COMMIT;

