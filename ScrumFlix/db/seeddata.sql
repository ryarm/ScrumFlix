-- Seed data file
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE ConcessionSaleItem;
TRUNCATE TABLE ConcessionSale;
TRUNCATE TABLE ConcessionItem;
TRUNCATE TABLE Ticket;
TRUNCATE TABLE Showtime;
TRUNCATE TABLE TheaterScreen;
TRUNCATE TABLE Movies;
TRUNCATE TABLE ScheduleAssignments;
TRUNCATE TABLE Shifts;
TRUNCATE TABLE TimeEntries;
TRUNCATE TABLE Timesheets;
TRUNCATE TABLE PayStubs;
TRUNCATE TABLE Payrolls;
TRUNCATE TABLE PayPeriods;
TRUNCATE TABLE Users;
TRUNCATE TABLE Employees;
TRUNCATE TABLE Location;

SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO Movies (Title, Rating, Genre, RuntimeMinutes, Description) VALUES
('Eclipse War', 'PG-13', 'Action', 120, 'A sci-fi war across collapsing dimensions.'),
('Neon Shadows', 'R', 'Action', 110, 'A cyberpunk detective uncovers corruption.'),
('Ocean’s Secret', 'PG', 'Action', 95, 'A family discovers a hidden underwater world.'),
('Chainsaw Man: The Reze Arc', 'R', 'Action', 130, 'Bomb devil'),
('Skybound', 'PG-13', 'Action', 105, 'A pilot fights to save a falling aircraft.'),
('Lost Kingdom', 'PG-13', 'Action', 140, 'An adventurer finds a forgotten empire.'),
('Code Zero', 'PG-13', 'Action', 100, 'Hackers race to stop a global shutdown.'),
('Silent Echo', 'PG', 'Action', 90, 'A mystery unfolds in a quiet town.'),
('Inferno Run', 'R', 'Action', 115, 'A high-speed chase through a burning city.'),
('Dream Circuit', 'PG', 'Action', 102, 'A teen enters a digital dream world.'),


('Midnight Protocol', 'PG-13', 'Action', 112, 'A rogue agent races against time to stop a covert global assassination network.'),
('Iron Horizon', 'PG-13', 'Action', 125, 'A group of rebels battles a militarized corporation controlling Earth’s last resources.'),
('Phantom Strike', 'R', 'Action', 118, 'An elite sniper is pulled into a deadly conspiracy after a mission goes wrong.'),
('Crimson Tidefall', 'PG-13', 'Action', 108, 'A naval officer fights to prevent a catastrophic war at sea.'),
('Velocity Point', 'PG-13', 'Action', 104, 'Street racers uncover a smuggling ring tied to high-speed competitions.'),
('Blackout City', 'R', 'Action', 122, 'When power grids fail, a former cop navigates chaos to protect his family.'),
('Thunderline', 'PG-13', 'Action', 109, 'A train hijacking spirals into a nationwide crisis.'),
('Steel Reign', 'PG-13', 'Action', 131, 'A futuristic soldier leads a revolt against AI-controlled governments.'),
('Nightfall Unit', 'R', 'Action', 117, 'A covert task force is sent on a mission that reveals hidden betrayals.'),
('Rogue Velocity', 'PG-13', 'Action', 106, 'A getaway driver is forced into one last job with deadly consequences.'),

('The Last Signal', 'PG', 'Drama', 97, 'A grieving father finds meaning through mysterious radio transmissions.'),
('Golden Silence', 'PG-13', 'Drama', 110, 'A pianist struggles to perform after losing her hearing.'),
('Paper Bridges', 'PG', 'Drama', 102, 'Two strangers form a bond while rebuilding their lives after tragedy.'),
('Harbor Lights', 'PG', 'Drama', 95, 'A small-town fisherman faces life-changing decisions during a harsh winter.'),
('Broken Compass', 'PG-13', 'Drama', 108, 'A traveler searches for purpose after losing everything.'),
('Stillwater Road', 'PG', 'Drama', 100, 'A family reconnects while uncovering secrets from their past.'),
('Echoes of June', 'PG-13', 'Drama', 112, 'A woman revisits her hometown to confront unresolved memories.'),
('Second Chances', 'PG', 'Drama', 98, 'A former athlete mentors troubled youth in a struggling community.'),
('Fading Colors', 'PG-13', 'Drama', 115, 'An artist battles personal demons while chasing inspiration.'),
('Open Skies', 'PG', 'Drama', 101, 'A pilot rediscovers life through unexpected friendships.'),

('Laugh Track', 'PG', 'Comedy', 92, 'A failed comedian accidentally becomes an overnight sensation.'),
('Office Mayhem', 'PG-13', 'Comedy', 97, 'Coworkers compete in absurd ways for a promotion.'),
('Roommate Roulette', 'PG', 'Comedy', 95, 'A college student deals with increasingly bizarre roommates.'),
('Wedding Crash Plan', 'PG-13', 'Comedy', 104, 'A group of friends sabotage a wedding for the right reasons.'),
('Vacation Disaster', 'PG', 'Comedy', 99, 'A family trip spirals into chaos across multiple countries.'),
('Neighbor Wars', 'PG-13', 'Comedy', 101, 'Two neighbors escalate a petty feud into full-scale warfare.'),
('Pet Problems', 'PG', 'Comedy', 88, 'A man struggles to manage a house full of mischievous animals.'),
('Comedy of Errors', 'PG', 'Comedy', 93, 'A series of misunderstandings leads to hilarious consequences.'),
('Startup Shenanigans', 'PG-13', 'Comedy', 102, 'Tech founders fake success to secure funding.'),
('Late Night Mishaps', 'PG', 'Comedy', 94, 'A talk show host deals with unpredictable guests.'),

('Haunted Hollow', 'PG-13', 'Horror', 105, 'A group of teens explores a town cursed by a forgotten legend.'),
('The Red Door', 'R', 'Horror', 110, 'Opening a mysterious door unleashes terrifying consequences.'),
('Shadow House', 'PG-13', 'Horror', 102, 'A family moves into a home that watches them back.'),
('Echoes Below', 'R', 'Horror', 108, 'Explorers uncover something ancient beneath the earth.'),
('Night Whisper', 'PG-13', 'Horror', 99, 'Voices in the dark guide a woman toward danger.'),
('Blood Moon Rising', 'R', 'Horror', 115, 'A rare lunar event awakens something deadly.'),
('The Forgotten Room', 'PG-13', 'Horror', 101, 'A hidden room reveals a sinister past.'),
('Graveyard Shift', 'R', 'Horror', 97, 'Workers encounter horrors during a late-night shift.'),
('Silent Screams', 'PG-13', 'Horror', 103, 'A town hides its darkest secrets behind silence.'),
('Dark Reflection', 'R', 'Horror', 109, 'Mirrors begin showing a terrifying alternate reality.');

INSERT INTO Location (LocationName, LocationAddress) VALUES
('North Theater', '123 Main St'),
('South Theater', '456 Elm St'),
('East Theater', '111 Street St'),
('West Theater', '222 Street St');

INSERT INTO TheaterScreen (LocationId, ScreenName, Capacity) VALUES
(1, 'N Screen 1 Small', 50),
(1, 'N Screen 2 Medium', 60),
(1, 'N Screen 3 Large', 70),
(2, 'S Screen 1 Small', 50),
(2, 'S Screen 2 Medium', 60),
(2, 'S Screen 3 Large', 70),
(3, 'E Screen 1 Small', 50),
(3, 'E Screen 2 Medium', 60),
(3, 'E Screen 3 Large', 70),
(4, 'W Screen 1 Small', 50),
(4, 'W Screen 2 Medium', 60),
(4, 'W Screen 3 Large', 70);

INSERT INTO Showtime (MovieId, TheaterScreenId, StartTime, Capacity, PricePerTicket)
VALUES
(1, 1, '2026-05-01 09:00:00', 50, 8.00),
(2, 1, '2026-05-01 11:30:00', 50, 9.00),
(3, 1, '2026-05-01 14:00:00', 50, 11.00),
(4, 1, '2026-05-01 16:30:00', 50, 13.00),
(5, 1, '2026-05-01 19:30:00', 50, 15.00),

(6, 2, '2026-05-02 09:15:00', 60, 8.00),
(7, 2, '2026-05-02 12:05:00', 60, 10.00),
(8, 2, '2026-05-02 14:15:00', 60, 11.00),
(9, 2, '2026-05-02 16:15:00', 60, 13.00),
(10, 2, '2026-05-02 18:40:00', 60, 15.00),

(11, 3, '2026-05-03 10:00:00', 70, 8.00),
(12, 3, '2026-05-03 12:30:00', 70, 10.00),
(13, 3, '2026-05-03 15:05:00', 70, 12.00),
(14, 3, '2026-05-03 17:35:00', 70, 14.00),

(15, 4, '2026-05-04 09:30:00', 50, 8.00),
(16, 4, '2026-05-04 12:00:00', 50, 10.00),
(17, 4, '2026-05-04 14:35:00', 50, 12.00),
(18, 4, '2026-05-04 17:05:00', 50, 14.00),
(19, 4, '2026-05-04 20:00:00', 50, 16.00),

(20, 5, '2026-05-05 09:45:00', 60, 8.00),
(21, 5, '2026-05-05 12:05:00', 60, 10.00),
(22, 5, '2026-05-05 14:15:00', 60, 12.00),
(23, 5, '2026-05-05 16:35:00', 60, 14.00),
(24, 5, '2026-05-05 19:00:00', 60, 15.00),

(25, 6, '2026-05-06 10:15:00', 70, 8.00),
(26, 6, '2026-05-06 12:35:00', 70, 10.00),
(27, 6, '2026-05-06 15:00:00', 70, 12.00),
(28, 6, '2026-05-06 17:25:00', 70, 14.00),

(29, 7, '2026-05-07 09:00:00', 50, 8.00),
(30, 7, '2026-05-07 11:25:00', 50, 9.00),
(31, 7, '2026-05-07 14:00:00', 50, 11.00),
(32, 7, '2026-05-07 16:05:00', 50, 13.00),
(33, 7, '2026-05-07 18:15:00', 50, 15.00),

(34, 8, '2026-05-08 09:20:00', 60, 8.00),
(35, 8, '2026-05-08 11:55:00', 60, 10.00),
(36, 8, '2026-05-08 14:05:00', 60, 12.00),
(37, 8, '2026-05-08 16:15:00', 60, 13.00),
(38, 8, '2026-05-08 18:15:00', 60, 15.00),

(39, 9, '2026-05-09 10:00:00', 70, 8.00),
(40, 9, '2026-05-09 12:05:00', 70, 10.00),
(41, 9, '2026-05-09 14:10:00', 70, 12.00),
(42, 9, '2026-05-09 16:25:00', 70, 14.00),

(43, 10, '2026-05-10 09:30:00', 50, 8.00),
(44, 10, '2026-05-10 12:00:00', 50, 10.00),
(45, 10, '2026-05-10 14:25:00', 50, 12.00),
(46, 10, '2026-05-10 16:35:00', 50, 14.00),

(47, 11, '2026-05-11 09:45:00', 60, 8.00),
(48, 11, '2026-05-11 12:15:00', 60, 10.00),
(49, 11, '2026-05-11 14:25:00', 60, 12.00),
(50, 11, '2026-05-11 16:40:00', 60, 14.00);

INSERT INTO Employees (FirstName, MiddleName, LastName, DOB, Phone, Email, Address, PayRate, LocationId) VALUES
('John', 'Ben', 'Doe', '2000-02-23', '1112223333', 'johndoe@gmail.com', '123 Main St, City NY 12345', '15.00', '1'),
('Jane', 'Lin', 'Doe', '2000-02-23', '1112223333', 'janedoe@gmail.com', '123 Main St, City NY 12345', '16.00', '1'),
('Joe', 'Tess', 'Doe', '2000-02-23', '1112223333', 'joedoe@gmail.com', '123 Main St, City NY 12345', '15.00', '1'),
('Gilben', 'Oxymoron', 'Herberth', '2003-02-23', '2147859385', 'oxymorongilben@gmail.com', '1112 Thorn Dr, Dallas TX 75234', '250.00', '1');

INSERT INTO Users (EmployeeId, UserName, UserPassword, RoleId)
VALUES
(1, 'a1', 'a123', 1),
(2, 'e1', 'e123', 3);

INSERT INTO ConcessionItem (ItemName, Price, QuantityInStock, Minimum)
VALUES
('Popcorn', 8.00, 30, 5),
('Candy', 3.00, 40, 10),
('Drink', 4.00, 20, 5);

INSERT INTO Ticket (TicketCode, ShowtimeId, UserAtSale, TimeOfSale)
VALUES
('481237', '1', '2', '2026-04-29 14:45:20'),
('438832', '2', '2', '2026-04-29 14:45:31'),
('107416', '3', '2', '2026-04-29 14:45:40'),
('906221', '1', '2', '2026-04-29 14:46:05'),
('955114', '3', '2', '2026-04-29 14:46:14');

INSERT INTO Shifts (StartTime, EndTime, RoleId, LocationId)
VALUES
('2026-05-01 08:00:00', '2026-05-01 12:00:00', 3, 1),
('2026-05-01 09:00:00', '2026-05-01 13:00:00', 2, 1),
('2026-05-01 13:00:00', '2026-05-01 17:00:00', 3, 1),
('2026-05-01 15:00:00', '2026-05-01 20:00:00', 2, 1),

('2026-05-02 08:30:00', '2026-05-02 12:30:00', 3, 2),
('2026-05-02 10:00:00', '2026-05-02 14:00:00', 2, 2),
('2026-05-02 14:00:00', '2026-05-02 18:00:00', 3, 2),
('2026-05-02 16:00:00', '2026-05-02 21:00:00', 2, 2);