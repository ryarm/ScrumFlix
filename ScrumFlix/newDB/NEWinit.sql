DROP DATABASE IF EXISTS defaultdb;
CREATE DATABASE defaultdb;
USE defaultdb;

-- 1. MOVIES
CREATE TABLE Movies (
    MovieId INT PRIMARY KEY AUTO_INCREMENT,
    Title VARCHAR(200) NOT NULL UNIQUE,
    Rating VARCHAR(20) NOT NULL,
    RuntimeMinutes SMALLINT NOT NULL,
    Description VARCHAR(1000) NOT NULL
);

-- 2. LOCATIONS
CREATE TABLE Location (
	LocationId INT PRIMARY KEY AUTO_INCREMENT,
    LocationName VARCHAR(100) NOT NULL UNIQUE,
    LocationAddress VARCHAR(255) UNIQUE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 3. THEATER ROOMS
CREATE TABLE TheaterScreen (
    TheaterScreenId INT PRIMARY KEY AUTO_INCREMENT,
    LocationId INT NOT NULL,
    ScreenName VARCHAR(100) NOT NULL,
    Capacity INT NOT NULL DEFAULT 50,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT fk_TheaterScreen_LocationId
		FOREIGN KEY (LocationId)
        REFERENCES Location(LocationId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- 4. Showtimes
CREATE TABLE Showtime (
    ShowtimeId INT PRIMARY KEY AUTO_INCREMENT,
    MovieId INT NOT NULL,
    TheaterScreenId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    Capacity INT NOT NULL DEFAULT 50,
    PricePerTicket DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_MovieId
        FOREIGN KEY (MovieId)
        REFERENCES Movies(MovieId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_TheaterScreen
        FOREIGN KEY (TheaterScreenId)
        REFERENCES TheaterScreen(TheaterScreenId)	
        ON DELETE RESTRICT
        ON UPDATE CASCADE

    /*CONSTRAINT chk_show_time
        CHECK (end_datetime > start_datetime)*/
);


-- 5. EMPLOYEES
CREATE TABLE Employees (
    EmployeeId INT PRIMARY KEY AUTO_INCREMENT,
    FirstName VARCHAR(50) NOT NULL,
    MiddleName VARCHAR(50),
    LastName VARCHAR(50) NOT NULL,
    DOB DATE NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Address VARCHAR(200),
    PayRate DECIMAL(10,2),
    LocationId INT,
    
    CONSTRAINT fk_Employees_LocationId
		FOREIGN KEY (LocationId)
        REFERENCES Location(LocationId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- 6. ROLES
CREATE TABLE Roles (
	RoleId INT PRIMARY KEY AUTO_INCREMENT,
    RoleName VARCHAR(30)
);

INSERT INTO Roles (RoleName) VALUES
('Admin'),
('Manager'),
('Employee');

-- 7. USERS
CREATE TABLE Users (
	UserId INT PRIMARY KEY AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    UserName VARCHAR(100) NOT NULL,
    UserPassword VARCHAR(20) NOT NULL,
    RoleId INT NOT NULL,
    
    CONSTRAINT fk_EmployeeId
		FOREIGN KEY (EmployeeId)
        REFERENCES Employees(EmployeeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
        
	CONSTRAINT fk_RoleId
		FOREIGN KEY (RoleId)
        REFERENCES Roles(RoleId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- Tickets
CREATE TABLE Ticket (
	TicketId INT PRIMARY KEY AUTO_INCREMENT,
    TicketCode INT NOT NULL,
    ShowtimeId INT NOT NULL,
    UserAtSale INT NOT NULL,
    TimeOfSale DATETIME NOT NULL,
    
    CONSTRAINT fk_ShowtimeId
		FOREIGN KEY (ShowtimeId)
        REFERENCES Showtime(ShowtimeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
        
	CONSTRAINT fk_UserAtSale
		FOREIGN KEY (UserAtSale)
		REFERENCES Users(UserId)
		ON DELETE RESTRICT
		ON UPDATE CASCADE
);
    
-- Concessions
CREATE TABLE ConcessionItem (
    ConcessionItemId INT PRIMARY KEY AUTO_INCREMENT,
    ItemName VARCHAR(100) NOT NULL UNIQUE,
    Price DECIMAL(10,2) NOT NULL,
    QuantityInStock INT NOT NULL DEFAULT 0,
    Minimum INT NOT NULL DEFAULT 5,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);


CREATE TABLE ConcessionSale (
    ConcessionSaleId INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    CustomerEmail VARCHAR(100) NOT NULL,
    TimeOfSale DATETIME NOT NULL,
    Total DECIMAL(10,2) NOT NULL,

    CONSTRAINT fk_ConcessionSale_User
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE ConcessionSaleItem (
    ConcessionSaleItemId INT PRIMARY KEY AUTO_INCREMENT,
    ConcessionSaleId INT NOT NULL,
    ConcessionItemId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    LineTotal DECIMAL(10,2) NOT NULL,

    CONSTRAINT fk_ConcessionSaleItem_Sale
        FOREIGN KEY (ConcessionSaleId)
        REFERENCES ConcessionSale(ConcessionSaleId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_ConcessionSaleItem_Item
        FOREIGN KEY (ConcessionItemId)
        REFERENCES ConcessionItem(ConcessionItemId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- 8. SCHEDULING
CREATE TABLE Shifts (
    ShiftId INT PRIMARY KEY AUTO_INCREMENT,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    RoleId INT NOT NULL,
    LocationId INT NOT NULL,

    CONSTRAINT fk_Shifts_RoleId
        FOREIGN KEY (RoleId)
        REFERENCES Roles(RoleId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_Shifts_LocationId
        FOREIGN KEY (LocationId)
        REFERENCES Location(LocationId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT chk_Shifts_Time
        CHECK (EndTime > StartTime)
);

CREATE TABLE ScheduleAssignments (
    AssignmentId INT PRIMARY KEY AUTO_INCREMENT,
    AssignmentName VARCHAR(50) NOT NULL,
    EmployeeId INT NOT NULL,
    ShiftId INT NOT NULL,
    ShowtimeId INT NULL,

    CONSTRAINT fk_ScheduleAssignments_EmployeeId
        FOREIGN KEY (EmployeeId)
        REFERENCES Employees(EmployeeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_ScheduleAssignments_ShiftId
        FOREIGN KEY (ShiftId)
        REFERENCES Shifts(ShiftId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_ScheduleAssignments_ShowtimeId
        FOREIGN KEY (ShowtimeId)
        REFERENCES Showtime(ShowtimeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- 9. TIME TRACKING
CREATE TABLE TimeEntries (
    TimeEntryId INT PRIMARY KEY AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    ClockIn DATETIME NOT NULL,
    ClockOut DATETIME NULL,

    CONSTRAINT fk_TimeEntries_EmployeeId
        FOREIGN KEY (EmployeeId)
        REFERENCES Employees(EmployeeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT chk_TimeEntries_Time
        CHECK (ClockOut IS NULL OR ClockOut >= ClockIn)
);

-- 10. PAYROLL
CREATE TABLE PayPeriods (
    PayPeriodId INT PRIMARY KEY AUTO_INCREMENT,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    CONSTRAINT chk_PayPeriods_Date
        CHECK (EndDate >= StartDate)
);

CREATE TABLE Timesheets (
    TimesheetId INT PRIMARY KEY AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    PayPeriodId INT NOT NULL,
    TotalHours DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    Approved BOOLEAN NOT NULL DEFAULT FALSE,
    ApprovedByUserId INT NULL,

    CONSTRAINT fk_Timesheets_EmployeeId
        FOREIGN KEY (EmployeeId)
        REFERENCES Employees(EmployeeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_Timesheets_PayPeriodId
        FOREIGN KEY (PayPeriodId)
        REFERENCES PayPeriods(PayPeriodId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_Timesheets_ApprovedByUserId
        FOREIGN KEY (ApprovedByUserId)
        REFERENCES Users(UserId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT chk_Timesheets_TotalHours
        CHECK (TotalHours >= 0)
);

CREATE TABLE Payrolls (
    PayrollId INT PRIMARY KEY AUTO_INCREMENT,
    EmployeeId INT NOT NULL,
    PayPeriodId INT NOT NULL,
    GrossPay DECIMAL(10,2) NOT NULL DEFAULT 0.00,

    CONSTRAINT fk_Payrolls_EmployeeId
        FOREIGN KEY (EmployeeId)
        REFERENCES Employees(EmployeeId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_Payrolls_PayPeriodId
        FOREIGN KEY (PayPeriodId)
        REFERENCES PayPeriods(PayPeriodId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT chk_Payrolls_GrossPay
        CHECK (GrossPay >= 0)
);

CREATE TABLE PayStubs (
    PayStubId INT PRIMARY KEY AUTO_INCREMENT,
    PayrollId INT NOT NULL,
    IssueDate DATETIME NOT NULL,

    CONSTRAINT fk_PayStubs_PayrollId
        FOREIGN KEY (PayrollId)
        REFERENCES Payrolls(PayrollId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);



-- AUDIT LOG
CREATE TABLE AuditLog (
    AuditLogId INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    ActionType VARCHAR(100) NOT NULL,
    TableName VARCHAR(100) NOT NULL,
    ObjectId INT NULL,
    ActionTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    OldValues TEXT NULL,
    NewValues TEXT NULL,
    Description VARCHAR(255) NULL,

    CONSTRAINT fk_AuditLog_UserId
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);






-- SEED DATA
INSERT INTO Movies (Title, Rating, RuntimeMinutes, Description) VALUES
('Eclipse War', 'PG-13', 120, 'A sci-fi war across collapsing dimensions.'),
('Neon Shadows', 'R', 110, 'A cyberpunk detective uncovers corruption.'),
('Ocean’s Secret', 'PG', 95, 'A family discovers a hidden underwater world.'),
('Chainsaw Man: The Reze Arc', 'R', 130, 'Bomb devil'),
('Skybound', 'PG-13', 105, 'A pilot fights to save a falling aircraft.'),
('Lost Kingdom', 'PG-13', 140, 'An adventurer finds a forgotten empire.'),
('Code Zero', 'PG-13', 100, 'Hackers race to stop a global shutdown.'),
('Silent Echo', 'PG', 90, 'A mystery unfolds in a quiet town.'),
('Inferno Run', 'R', 115, 'A high-speed chase through a burning city.'),
('Dream Circuit', 'PG', 102, 'A teen enters a digital dream world.');

INSERT INTO Location (LocationName, LocationAddress) VALUES
('North Theater', '123 Main St'),
('South Theater', '456 Elm St');

INSERT INTO TheaterScreen (LocationId, ScreenName, Capacity) VALUES
(1, 'Screen 1', 50),
(1, 'Screen 2', 60),
(1, 'Screen 3', 70),
(2, 'Screen 1', 50),
(2, 'Screen 2', 60),
(2, 'Screen 3', 70);

INSERT INTO Showtime (MovieId, TheaterScreenId, StartTime, Capacity)
VALUES
(1, 1, '2026-04-01 18:00:00', 50),
(4, 2, '2026-04-01 20:00:00', 60),
(6, 1, '2026-04-01 21:00:00', 50);

INSERT INTO Employees (FirstName, MiddleName, LastName, DOB, Phone, Email, Address) VALUES
('John', 'Ben', 'Doe', '2000-02-23', '1112223333', 'johndoe@gmail.com', '123 Main St, City NY 12345'),
('Jane', 'Lin', 'Doe', '2000-02-23', '1112223333', 'janedoe@gmail.com', '123 Main St, City NY 12345'),
('Joe', 'Tess', 'Doe', '2000-02-23', '1112223333', 'joedoe@gmail.com', '123 Main St, City NY 12345'),
('Gilben', 'Oxymoron', 'Herberth', '2003-02-23', '2147859385', 'oxymorongilben@gmail.com', '1112 Thorn Dr, Dallas TX 75234');

INSERT INTO Users (EmployeeId, UserName, UserPassword, RoleId)
VALUES
(1, 'a1', 'a123', 1),
(2, 'e1', 'e123', 3);

INSERT INTO ConcessionItem (ItemName, Price, QuantityInStock, Minimum)
VALUES
('Popcorn', 8.00, 30, 5),
('Candy', 3.00, 40, 10),
('Drink', 4.00, 20, 5);