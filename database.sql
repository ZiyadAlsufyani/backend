-- =====================================================
-- Horse Racing Database System
-- MySQL Database Schema and Data
-- =====================================================

-- Drop existing tables if they exist (in reverse order of dependencies)
DROP TABLE IF EXISTS RaceResults;
DROP TABLE IF EXISTS Race;
DROP TABLE IF EXISTS Track;
DROP TABLE IF EXISTS Owns;
DROP TABLE IF EXISTS Trainer;
DROP TABLE IF EXISTS Owner;
DROP TABLE IF EXISTS Horse;
DROP TABLE IF EXISTS Stable;
DROP TABLE IF EXISTS old_info;

-- =====================================================
-- 1. CREATE SCHEMA
-- =====================================================

-- Stable Table
CREATE TABLE Stable (
    stableId VARCHAR(15) PRIMARY KEY,
    stableName VARCHAR(100) NOT NULL,
    location VARCHAR(100),
    colors VARCHAR(50)
);

-- Horse Table
CREATE TABLE Horse (
    horseId VARCHAR(15) PRIMARY KEY,
    horseName VARCHAR(100) NOT NULL,
    age INT,
    gender ENUM('Male', 'Female', 'Gelding'),
    registration VARCHAR(50),
    stableId VARCHAR(15),
    FOREIGN KEY (stableId) REFERENCES Stable(stableId) ON DELETE SET NULL
);

-- Owner Table
CREATE TABLE Owner (
    ownerId VARCHAR(15) PRIMARY KEY,
    lname VARCHAR(50) NOT NULL,
    fname VARCHAR(50) NOT NULL
);

-- Owns Table (Many-to-Many relationship)
CREATE TABLE Owns (
    ownerId VARCHAR(15),
    horseId VARCHAR(15),
    PRIMARY KEY (ownerId, horseId),
    FOREIGN KEY (ownerId) REFERENCES Owner(ownerId) ON DELETE CASCADE,
    FOREIGN KEY (horseId) REFERENCES Horse(horseId) ON DELETE CASCADE
);

-- Trainer Table
CREATE TABLE Trainer (
    trainerId VARCHAR(15) PRIMARY KEY,
    lname VARCHAR(50) NOT NULL,
    fname VARCHAR(50) NOT NULL,
    stableId VARCHAR(15),
    FOREIGN KEY (stableId) REFERENCES Stable(stableId) ON DELETE SET NULL
);

-- Track Table
CREATE TABLE Track (
    trackName VARCHAR(100) PRIMARY KEY,
    location VARCHAR(100),
    length DECIMAL(5,2)
);

-- Race Table
CREATE TABLE Race (
    raceId VARCHAR(15) PRIMARY KEY,
    raceName VARCHAR(100) NOT NULL,
    trackName VARCHAR(100),
    raceDate DATE,
    raceTime TIME,
    FOREIGN KEY (trackName) REFERENCES Track(trackName) ON DELETE SET NULL
);

-- RaceResults Table
CREATE TABLE RaceResults (
    raceId VARCHAR(15),
    horseId VARCHAR(15),
    results INT,
    prize DECIMAL(10,2),
    PRIMARY KEY (raceId, horseId),
    FOREIGN KEY (raceId) REFERENCES Race(raceId) ON DELETE CASCADE,
    FOREIGN KEY (horseId) REFERENCES Horse(horseId) ON DELETE CASCADE
);

-- old_info Table (for deleted horses)
CREATE TABLE old_info (
    horseId VARCHAR(15),
    horseName VARCHAR(100),
    age INT,
    gender ENUM('Male', 'Female', 'Gelding'),
    registration VARCHAR(50),
    stableId VARCHAR(15),
    deleted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- 2. CREATE PROCEDURAL SQL
-- =====================================================

-- Drop trigger if exists
DROP TRIGGER IF EXISTS before_horse_delete;

-- Create Trigger: before_horse_delete
DELIMITER //
CREATE TRIGGER before_horse_delete
BEFORE DELETE ON Horse
FOR EACH ROW
BEGIN
    INSERT INTO old_info (horseId, horseName, age, gender, registration, stableId)
    VALUES (OLD.horseId, OLD.horseName, OLD.age, OLD.gender, OLD.registration, OLD.stableId);
END//
DELIMITER ;

-- Drop stored procedure if exists
DROP PROCEDURE IF EXISTS sp_DeleteOwner;

-- Create Stored Procedure: sp_DeleteOwner
DELIMITER //
CREATE PROCEDURE sp_DeleteOwner(IN p_ownerId VARCHAR(15))
BEGIN
    -- Delete from Owns table first (child records)
    DELETE FROM Owns WHERE ownerId = p_ownerId;
    
    -- Delete from Owner table
    DELETE FROM Owner WHERE ownerId = p_ownerId;
END//
DELIMITER ;

-- =====================================================
-- 3. INSERT DATA
-- =====================================================

/* Stables */
INSERT INTO Stable VALUES ('stablel', 'Zobair Farm', 'Riyadh', 'orange');
INSERT INTO Stable VALUES ('stable2', 'Zayed Farm', 'Dubai', 'kiwi');
INSERT INTO Stable VALUES ('stable3', 'Zahra Farm', 'Jeddah', 'cinnamon');
INSERT INTO Stable VALUES ('stable4', 'Sunny Stables', 'Jubail', 'lemon');
INSERT INTO Stable VALUES ('stable5', 'Ajman Stables', 'Ajman', 'lemon');
INSERT INTO Stable VALUES ('stable6', 'Dubai Stables', 'Dubai', 'bright blue');

/* Horses */
INSERT INTO Horse VALUES ('horsel', 'Warrior', 2, 'C', '11111', 'stablel');
INSERT INTO Horse VALUES ('horse2', 'Conquerer', 2, 'F', '22222', 'stable6');
INSERT INTO Horse VALUES ('horse3', 'Dove of Peace', 3, 'C', '33333', 'stablel');
INSERT INTO Horse VALUES ('horse4', 'Ever Faster', 3, 'F', '44444', 'stable3');
INSERT INTO Horse VALUES ('horse5', 'Slow Winner', 2, 'C', '55555', 'stable3');
INSERT INTO Horse VALUES ('horse6', 'Windrunner', 2, 'F', '66666', 'stable2');
INSERT INTO Horse VALUES ('horse7', 'Catapult', 4, 'M', '77777', 'stable6');
INSERT INTO Horse VALUES ('horse8', 'Flying Force', 2, 'C', '88888', 'stable4');
INSERT INTO Horse VALUES ('horse9', 'Laggard', 2, 'F', '99999', 'stable4');
INSERT INTO Horse VALUES ('horse10', 'Formula One', 6, 'G', '10101', 'stable2');
INSERT INTO Horse VALUES ('horsell', 'Frisky Frolic', 3, 'C', '11011', 'stable4');
INSERT INTO Horse VALUES ('horse12', 'Fantastic', 3, 'F', '12121', 'stable2');
INSERT INTO Horse VALUES ('horse13', 'Midnight', 2, 'C', '13131', 'stable3');
INSERT INTO Horse VALUES ('horse14', 'Running Wild', 4, 'S', '14141', 'stable2');
INSERT INTO Horse VALUES ('horse15', 'FastOffMyFeet', 3, 'C', '15151', 'stablel');
INSERT INTO Horse VALUES ('horse16', 'Slow Poke', 2, 'C', '16161', 'stable3');
INSERT INTO Horse VALUES ('horse17', 'Slinger', 3, 'F', '17171', 'stable2');
INSERT INTO Horse VALUES ('horse18', 'Sublime', 5, 'M', '18181', 'stable6');
INSERT INTO Horse VALUES ('horse19', 'Front Runner', 4, 'G', '19191', 'stable4');
INSERT INTO Horse VALUES ('horse20', 'Night', 3, 'C', '20200', 'stablel');
INSERT INTO Horse VALUES ('horse21', 'Negative', 3, 'F', '21210', 'stable3');
INSERT INTO Horse VALUES ('horse22', 'Lightening', 2, 'C', '22220', 'stable6');
INSERT INTO Horse VALUES ('horse23', 'Lazy Loser', 4, 'G', '23230', 'stablel');
INSERT INTO Horse VALUES ('horse24', 'Leaping Lizard', 2, 'C', '24240', 'stablel');
INSERT INTO Horse VALUES ('horse25', 'Beautiful Brown ', 3, 'F', '25250', 'stable6');
INSERT INTO Horse VALUES ('horse26', 'Sick Winner', 5, 'M', '26260', 'stable2');

/* Owners */
INSERT INTO Owner VALUES('ownerl', 'Saeed', 'Ahmed');
INSERT INTO Owner VALUES('owner2', 'Mohammed', 'Khalid');
INSERT INTO Owner VALUES('owner3', 'Mohammed', 'Faisal');
INSERT INTO Owner VALUES('owner4', 'Fahd', 'Abdul Rahman');
INSERT INTO Owner VALUES('owner5', 'Nasr', '');
INSERT INTO Owner VALUES('owner6', 'Mohammed', 'Sheikh');
INSERT INTO Owner VALUES('owner7', 'Abed', 'Ahmed');
INSERT INTO Owner VALUES('owner8', 'Mashour', '');
INSERT INTO Owner VALUES('owner9', 'Said', 'Sheikh');
INSERT INTO Owner VALUES('owner10', 'Faisal', 'Khan');
INSERT INTO Owner VALUES('ownerll', 'Jabr', 'Mohammed');
INSERT INTO Owner VALUES('owner12', 'Faleh', 'Mahmood');
INSERT INTO Owner VALUES('owner13', 'Yahya', 'Mohammed');
INSERT INTO Owner VALUES('owner14', 'Sulaiman', '');
INSERT INTO Owner VALUES('owner15', 'Saeed', 'Ali');
INSERT INTO Owner VALUES('owner16', 'Ahmed', 'Faisal');
INSERT INTO Owner VALUES('owner17', 'Saud', 'Mohammed');
INSERT INTO Owner VALUES('owner18', 'Nazir', 'Mohammed');
INSERT INTO Owner VALUES('owner19', 'Saleh', 'Fahd');
INSERT INTO Owner VALUES('owner20', 'Mohammed', 'Naeem');

/* Owns */
INSERT INTO Owns VALUES('owner14', 'horsel');
INSERT INTO Owns VALUES('owner3', 'horse2');
INSERT INTO Owns VALUES ('owner2', 'horse3');
INSERT INTO Owns VALUES('owner2', 'horse4');
INSERT INTO Owns VALUES('ownerl', 'horse5');
INSERT INTO Owns VALUES('owner12', 'horse5');
INSERT INTO Owns VALUES('owner14', 'horse5');
INSERT INTO Owns VALUES('ownerl', 'horse6');
INSERT INTO Owns VALUES('owner5', 'horse6');
INSERT INTO Owns VALUES('owner20', 'horse7');
INSERT INTO Owns VALUES('owner19', 'horse8');
INSERT INTO Owns VALUES('owner2', 'horse9');
INSERT INTO Owns VALUES('owner18', 'horse10');
INSERT INTO Owns VALUES('owner3', 'horse10');
INSERT INTO Owns VALUES('owner4', 'horse12');
INSERT INTO Owns VALUES('owner16', 'horse13');
INSERT INTO Owns VALUES('owner17', 'horse14');
INSERT INTO Owns VALUES('owner15', 'horse14');
INSERT INTO Owns VALUES('owner15', 'horse15');
INSERT INTO Owns VALUES('owner20', 'horse16');
INSERT INTO Owns VALUES('owner4', 'horse17');
INSERT INTO Owns VALUES('owner6', 'horse18');
INSERT INTO Owns VALUES('owner12', 'horse20');
INSERT INTO Owns VALUES('owner7', 'horse19');
INSERT INTO Owns VALUES('owner7', 'horse21');
INSERT INTO Owns VALUES('ownerl10', 'horse22');
INSERT INTO Owns VALUES('owner12', 'horse23');
INSERT INTO Owns VALUES('ownerl3', 'horse24');
INSERT INTO Owns VALUES('owner2', 'horse25');
INSERT INTO Owns VALUES('owner9', 'horse26');
INSERT INTO Owns VALUES('owner8', 'horse23');

/* Trainers */
INSERT INTO Trainer VALUES('trainerl', 'Mohammed', 'Fahd', 'stable2');
INSERT INTO Trainer VALUES('trainer2', 'Saleh', 'Saeed', 'stablel');
INSERT INTO Trainer VALUES('trainer3', 'Ali', 'Raad', 'stable4');
INSERT INTO Trainer VALUES('trainer4', 'Sayed', 'Wasim', 'stable3');
INSERT INTO Trainer VALUES('trainer5', 'Ahmed', 'Ali', 'stable3');
INSERT INTO Trainer VALUES('trainer6', 'Faisal', 'Salah', 'stable5');
INSERT INTO Trainer VALUES('trainer7', 'Hamid', 'Ahmed', 'stable6');
INSERT INTO Trainer VALUES('trainer8', 'Khalid', 'Ahmed', 'stable6');

/* Tracks */
INSERT INTO Track VALUES ('Doha', 'QT', 20);
INSERT INTO Track VALUES('Jubail', 'SA', 15);
INSERT INTO Track VALUES ('Yanbu', 'SA', 18);
INSERT INTO Track VALUES('Dubai', 'UE', 17);
INSERT INTO Track VALUES('Jeddah', 'SA', 19);
INSERT INTO Track VALUES ('Bahrain', 'BH', 18);
INSERT INTO Track VALUES('Sharjah', 'UE', 20);
INSERT INTO Track VALUES('Riyadh', 'SA', 22);
INSERT INTO Track VALUES('Dhahran', 'SA', 20);

/* Races */
INSERT INTO Race VALUES('racel', 'Kings Cup', 'Riyadh', '2007-05-03', '14:00');
INSERT INTO Race VALUES('race2', '2-year-old fillies', 'Doha', '2007-05-03', '13:00');
INSERT INTO Race VALUES('race3', '2-year-old colts', 'Doha', '2007-05-03', '13:30');
INSERT INTO Race VALUES('race4', 'Handicap', 'Doha', '2007-05-03', '12:00');
INSERT INTO Race VALUES('race5', 'Claiming Stake', 'Sharjah', '2007-05-03', '12:30');
INSERT INTO Race VALUES('race6', '3-year-old fillies', 'Jubail', '2007-06-02', '12:30');
INSERT INTO Race VALUES ('race7', 'Handicap', 'Jubail', '2007-06-02','9:30');
INSERT INTO Race VALUES('race8', '2-year-old colts', 'Riyadh', '2007-06-02', '10:30');
INSERT INTO Race VALUES('race9', '2-year-old fillies', 'Jubail', '2007-06-02', '11:30');
INSERT INTO Race VALUES('race10', 'Claiming Stake', 'Sharjah', '2007-06-02', '12:30');
INSERT INTO Race VALUES('racell', '3-year-old fillies', 'Dubai', '2007-04-02', '10:30');
INSERT INTO Race VALUES('race12', 'Handicap', 'Yanbu', '2007-05-03', '11:30');
INSERT INTO Race VALUES('race13', '3-year-old fillies', 'Yanbu', '2007-05-03', '11:00');
INSERT INTO Race VALUES('race14', 'Handicap', 'Dhahran', '2007-05-10', '10:00');
INSERT INTO Race VALUES('race15', '3-year-old colts', 'Dubai', '2007-05-12','15:00');
INSERT INTO Race VALUES('race16', 'Claiming Stake', 'Yanbu', '2007-05-20', '14:30');
INSERT INTO Race VALUES('race17', 'Handicap', 'Doha', '2007-05-20', '13:00');
INSERT INTO Race VALUES('race18', '3-year-old fillies', 'Sharjah', '2007-05-21', '8:00');
INSERT INTO Race VALUES('race19', '2-year-old colts', 'Dhahran', '2007-05-25', '11:00');
INSERT INTO Race VALUES('race20', 'Claiming Stake', 'Jeddah', '2007-05-25','8:30');
INSERT INTO Race VALUES('race21', '3-year-old colts', 'Riyadh', '2007-03-19', '14:30');
INSERT INTO Race VALUES('race22', 'Handicap', 'Dhahran', '2007-03-27', '15:00');
INSERT INTO Race VALUES('race23', '3-year-old fillies', 'Jeddah', '2007-03-28','9:30');
INSERT INTO Race VALUES('race24', '3-year-old colts', 'Jubail', '2007-03-28', '13:30');
INSERT INTO Race VALUES('race25', 'Claiming Stake', 'Jeddah', '2007-03-29', '10:00');
INSERT INTO Race VALUES('race26', '3-year-old colts', 'Yanbu', '2007-03-30', '12:30');
INSERT INTO Race VALUES('race27', 'Handicap', 'Dubai', '2007-04-03', '14:00');
INSERT INTO Race VALUES('race28', '2-year-old fillies', 'Jeddah', '2007-04-04','8:30');
INSERT INTO Race VALUES ('race29', '3-year-old colts', 'Bahrain', '2007-04-05','8:00');
INSERT INTO Race VALUES('race30', 'Claiming Stake', 'Dhahran', '2007-04-08', '9:30');
INSERT INTO Race VALUES('race31', 'Handicap', 'Dhahran', '2007-04-08', '9:00');
INSERT INTO Race VALUES ('race32', '2-year-old colts', 'Jubail', '2007-04-09', '11:00');
INSERT INTO Race VALUES ('race33', 'Claiming Stake', 'Bahrain', '2007-04-10', '13:00');
INSERT INTO Race VALUES('race34', '3-year-old colts', 'Dubai', '2007-05-12', '12:00');
INSERT INTO Race VALUES('race35', 'Handicap', 'Dubai', '2007-04-13', '10:30');
INSERT INTO Race VALUES('race36', '3-year-old colts', 'Jeddah', '2007-05-03', '14:30');

/* RaceResults */
INSERT INTO RaceResults VALUES('racel', 'horse3', 'first', 500000);
INSERT INTO RaceResults VALUES('racel', 'horsell', 'second', 200000);
INSERT INTO RaceResults VALUES('racel', 'horse15', 'third', 500000);
INSERT INTO RaceResults VALUES('race2', 'horse6', 'first', 100000);
INSERT INTO RaceResults VALUES('race2', 'horse2', 'second', 50000);
INSERT INTO RaceResults VALUES('race2', 'horse20', 'third', 20000);
INSERT INTO RaceResults VALUES('race3', 'horse22', 'first', 70000);
INSERT INTO RaceResults VALUES('race3', 'horse5', 'second', 50000);
INSERT INTO RaceResults VALUES('race3', 'horsel', 'third', 20000);
INSERT INTO RaceResults VALUES('race4', 'horse19', 'first', 50000);
INSERT INTO RaceResults VALUES('race4', 'horsel8', 'no show', 0);
INSERT INTO RaceResults VALUES('race4', 'horse14', 'no show', 0);
INSERT INTO RaceResults VALUES('race6', 'horse25', 'first', 5000);
INSERT INTO RaceResults VALUES('race7', 'horse7', 'second', 2000);
INSERT INTO RaceResults VALUES('race9', 'horsell', 'last', 0);
INSERT INTO RaceResults VALUES('race10', 'horse18', 'fourth', 500);
INSERT INTO RaceResults VALUES('racell', 'horsel2', 'first', 50000);
INSERT INTO RaceResults VALUES('racell', 'horse17', 'second', 25000);
INSERT INTO RaceResults VALUES('racell', 'horse21', 'fourth', 10000);
INSERT INTO RaceResults VALUES('race12', 'horse14', 'first', 6000);
INSERT INTO RaceResults VALUES('race12', 'horse18', 'second', 5000);
INSERT INTO RaceResults VALUES('race13', 'horse25', 'first', 100000);
INSERT INTO RaceResults VALUES('race13', 'horse4', 'second', 50000);
INSERT INTO RaceResults VALUES('race13', 'horse12', 'third', 30000);
INSERT INTO RaceResults VALUES('race14', 'horse23', 'first', 25000);
INSERT INTO RaceResults VALUES('race14', 'horse26', 'second', 20000);
INSERT INTO RaceResults VALUES('race14', 'horse24', 'third', 10000);
INSERT INTO RaceResults VALUES('race15', 'horsell', 'second', 8000);
INSERT INTO RaceResults VALUES('race15', 'horse10', 'third', 4000);
INSERT INTO RaceResults VALUES('race16', 'horse14', 'first', 70000);
INSERT INTO RaceResults VALUES('race16', 'horse7', 'first', 15000);
INSERT INTO RaceResults VALUES('race17', 'horse10', 'second',1100);
INSERT INTO RaceResults VALUES('race17', 'horse22', 'first', 1000000);
INSERT INTO RaceResults VALUES('race18', 'horse6', 'first', 70000);
INSERT INTO RaceResults VALUES('race19', 'horse10', 'second', 80000);
INSERT INTO RaceResults VALUES('race19', 'horse20', 'third', 60000);
INSERT INTO RaceResults VALUES('race19', 'horse24', 'first', 1500);
INSERT INTO RaceResults VALUES('race20', 'horse20', 'second', 1000);
INSERT INTO RaceResults VALUES('race20', 'horsel', 'third', 800);
INSERT INTO RaceResults VALUES('race20', 'horse8', 'fourth', 500);
INSERT INTO RaceResults VALUES('race21', 'horse23', 'first', 70000);
INSERT INTO RaceResults VALUES('race21', 'horse14', 'second', 55000);
INSERT INTO RaceResults VALUES('race21', 'horse3', 'third', 40000);
INSERT INTO RaceResults VALUES('race22', 'horse26', 'first', 10000);
INSERT INTO RaceResults VALUES('race22', 'horse10', 'second', 8000);
INSERT INTO RaceResults VALUES('race23', 'horse24', 'first', 150000);
INSERT INTO RaceResults VALUES('race23', 'horse15', 'third', 40000);
INSERT INTO RaceResults VALUES('race24', 'horse18', 'first', 90000);
INSERT INTO RaceResults VALUES('race25', 'horse19', 'first', 80000);
INSERT INTO RaceResults VALUES('race25', 'horse25', 'second', 65000);
INSERT INTO RaceResults VALUES('race26', 'horse7', 'first', 10000);
INSERT INTO RaceResults VALUES('race26', 'horse18', 'second', 8000);
INSERT INTO RaceResults VALUES('race26', 'horse23', 'fourth', 2000);
INSERT INTO RaceResults VALUES('race27', 'horse25', 'first', 8000);
INSERT INTO RaceResults VALUES('race27', 'horse15', 'third', 3000);
INSERT INTO RaceResults VALUES('race28', 'horse3', 'second', 70000);
INSERT INTO RaceResults VALUES('race29', 'horse24', 'first', 90000);
INSERT INTO RaceResults VALUES('race29', 'horse14', 'second', 70000);
INSERT INTO RaceResults VALUES('race29', 'horse10', 'third', 50000);
INSERT INTO RaceResults VALUES('race29', 'horse16', 'fourth', 30000);
INSERT INTO RaceResults VALUES('race30', 'horse23', 'first', 150000);
INSERT INTO RaceResults VALUES('race30', 'horse26', 'second', 125000);
INSERT INTO RaceResults VALUES('race30', 'horsell', 'third', 100000);
INSERT INTO RaceResults VALUES('race31', 'horse10', 'fourth', 500);
INSERT INTO RaceResults VALUES('race31', 'horse22', 'second', 1700);
INSERT INTO RaceResults VALUES('race31', 'horse7', 'first', 90000);
INSERT INTO RaceResults VALUES('race31', 'horse15', 'third', 1200);
INSERT INTO RaceResults VALUES('race32', 'horse26', 'first', 40000);
INSERT INTO RaceResults VALUES('race32', 'horse13', 'second', 30000);
INSERT INTO RaceResults VALUES('race32', 'horse23', 'fourth', 2000);
INSERT INTO RaceResults VALUES('race33', 'horse7', 'first', 45000);
INSERT INTO RaceResults VALUES('race33', 'horse19', 'second', 25000);
INSERT INTO RaceResults VALUES('race34', 'horsell', 'first', 50000);
INSERT INTO RaceResults VALUES('race34', 'horse15', 'second', 30000);
INSERT INTO RaceResults VALUES('race35', 'horse19', 'first', 100000);
INSERT INTO RaceResults VALUES('race35', 'horse20', 'second', 80000);
INSERT INTO RaceResults VALUES('race36', 'horsell', 'first', 100000);
INSERT INTO RaceResults VALUES('race36', 'horse15', 'second', 80000);
INSERT INTO RaceResults VALUES('race36', 'horse20', 'third', 50000);

-- =====================================================
-- END OF SQL SCRIPT
-- =====================================================
