USE RaceDayDB;
GO

-- Drop existing views if they exist
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_EventSummary')
    DROP VIEW vw_EventSummary;
GO

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_ParticipantHistory')
    DROP VIEW vw_ParticipantHistory;
GO

-- Drop existing tables if they exist (in correct order - child tables first)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Results')
    DROP TABLE Results;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Enrolments')
    DROP TABLE Enrolments;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
    DROP TABLE Categories;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EventRoutes')
    DROP TABLE EventRoutes;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Events')
    DROP TABLE Events;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
    DROP TABLE Users;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
    DROP TABLE Roles;
GO

-- Now create all tables fresh
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);
GO

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    RoleID INT NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender CHAR(1) CHECK (Gender IN ('M', 'F', 'O')),
    PhoneNumber NVARCHAR(20),
    ProfileImage NVARCHAR(500)
);
GO

CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    EventDate DATETIME NOT NULL,
    Venue NVARCHAR(255),
    City NVARCHAR(100) NOT NULL,
    Province NVARCHAR(100) NOT NULL,
    EntryFee DECIMAL(10,2) DEFAULT 0.00,
    MaxParticipants INT DEFAULT 1000,
    Status NVARCHAR(50) DEFAULT 'Draft'
);
GO

CREATE TABLE EventRoutes (
    RouteID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL UNIQUE,
    RouteName NVARCHAR(200) NOT NULL,
    StartPoint NVARCHAR(255),
    EndPoint NVARCHAR(255),
    Distance DECIMAL(8,2) NOT NULL,
    ElevationGain DECIMAL(8,2),
    RouteMapURL NVARCHAR(500),
    GPXFileURL NVARCHAR(500)
);
GO

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Distance DECIMAL(6,2) NOT NULL,
    StartTime TIME NOT NULL,
    AgeGroup NVARCHAR(50),
    EntryFee DECIMAL(10,2) DEFAULT 0.00,
    MaxParticipants INT DEFAULT 100
);
GO

CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Registered',
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending',
    BibNumber NVARCHAR(20) UNIQUE,
    VolunteerRole NVARCHAR(100),
    MedicalNotes NVARCHAR(MAX)
);
GO

CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    UserID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    FinishTime TIME(3),
    OverallPosition INT,
    CategoryPosition INT,
    Status NVARCHAR(50) DEFAULT 'Pending'
);
GO

-- Add Foreign Keys
ALTER TABLE Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID);
ALTER TABLE Events ADD CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserID) REFERENCES Users(UserID);
ALTER TABLE EventRoutes ADD CONSTRAINT FK_EventRoutes_Events FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE;
ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Events FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE;
ALTER TABLE Enrolments ADD CONSTRAINT FK_Enrolments_Users FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE Enrolments ADD CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventID) REFERENCES Events(EventID);
ALTER TABLE Enrolments ADD CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID);
ALTER TABLE Results ADD CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID);
ALTER TABLE Results ADD CONSTRAINT FK_Results_Users FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE Results ADD CONSTRAINT FK_Results_Events FOREIGN KEY (EventID) REFERENCES Events(EventID);
ALTER TABLE Results ADD CONSTRAINT FK_Results_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID);
GO

-- Insert Roles
INSERT INTO Roles (RoleName, Description) VALUES 
('Admin', 'System Administrator'),
('Organiser', 'Event Organiser'),
('Participant', 'Regular Participant');
GO

-- Insert Users
INSERT INTO Users (RoleID, Email, PasswordHash, FirstName, LastName, DateOfBirth, Gender, PhoneNumber, ProfileImage) VALUES
(2, 'organiser1@raceday.co.za', 'hash1', 'Thabo', 'Mokoena', '1985-03-15', 'M', '+27821234567', 'profile1.jpg'),
(2, 'organiser2@raceday.co.za', 'hash2', 'Sarah', 'Naidoo', '1990-07-22', 'F', '+27829876543', 'profile2.jpg'),
(3, 'runner1@raceday.co.za', 'hash3', 'Sipho', 'Ndlovu', '1995-11-05', 'M', '+27827654321', 'profile3.jpg'),
(3, 'runner2@raceday.co.za', 'hash4', 'Zandile', 'Petersen', '1988-09-18', 'F', '+27828876543', 'profile4.jpg');
GO

-- Insert Events
INSERT INTO Events (OrganiserID, EventName, Description, EventDate, Venue, City, Province, EntryFee, MaxParticipants, Status) VALUES
(1, 'Cape Town Cycle Tour', 'World''s largest timed cycle race', '2026-03-08 06:00:00', 'Cape Town Stadium', 'Cape Town', 'Western Cape', 350.00, 35000, 'Open'),
(1, 'Comrades Marathon', 'The ultimate human race', '2026-06-16 05:30:00', 'Durban City Hall', 'Durban', 'KwaZulu-Natal', 800.00, 25000, 'Open'),
(2, 'Soweto Marathon', 'Iconic marathon through Soweto', '2026-11-02 05:00:00', 'Orlando Stadium', 'Johannesburg', 'Gauteng', 500.00, 15000, 'Open');
GO

-- Insert Event Routes
INSERT INTO EventRoutes (EventID, RouteName, StartPoint, EndPoint, Distance, ElevationGain, RouteMapURL, GPXFileURL) VALUES
(1, 'Cape Peninsula Loop', 'Cape Town Stadium', 'Cape Town Stadium', 109.00, 1650, 'map1.jpg', 'gpx1.gpx'),
(2, 'Durban to PMB', 'Durban City Hall', 'Pietermaritzburg', 90.00, 1250, 'map2.jpg', 'gpx2.gpx'),
(3, 'Soweto Streets', 'Orlando Stadium', 'Orlando Stadium', 42.20, 550, 'map3.jpg', 'gpx3.gpx');
GO

-- Insert Categories
INSERT INTO Categories (EventID, CategoryName, Distance, StartTime, AgeGroup, EntryFee, MaxParticipants) VALUES
(1, 'Elite Men', 109.00, '06:00:00', '18-39', 350.00, 500),
(1, 'Elite Women', 109.00, '06:00:00', '18-39', 350.00, 500),
(1, 'Veterans Men', 109.00, '06:15:00', '40-49', 350.00, 2000),
(2, 'Ultra Men', 90.00, '05:30:00', '18-39', 800.00, 10000),
(2, 'Ultra Women', 90.00, '05:30:00', '18-39', 800.00, 10000),
(3, 'Full Marathon Men', 42.20, '05:00:00', '18-39', 500.00, 3000);
GO

-- Insert Enrolments
INSERT INTO Enrolments (UserID, EventID, CategoryID, Status, PaymentStatus, BibNumber, VolunteerRole, MedicalNotes) VALUES
(3, 1, 1, 'Confirmed', 'Paid', 'BIB001', NULL, 'None'),
(3, 2, 4, 'Confirmed', 'Paid', 'BIB002', NULL, 'Knee injury'),
(4, 1, 2, 'Confirmed', 'Paid', 'BIB003', NULL, NULL),
(4, 2, 5, 'Confirmed', 'Paid', 'BIB004', NULL, 'Allergies: peanuts'),
(3, 3, 6, 'Registered', 'Paid', 'BIB005', NULL, NULL);
GO

-- Insert Results
INSERT INTO Results (EnrolmentID, UserID, EventID, CategoryID, FinishTime, OverallPosition, CategoryPosition, Status) VALUES
(1, 3, 1, 1, '02:45:30.000', 10, 2, 'Completed'),
(3, 4, 1, 2, '02:50:15.000', 15, 3, 'Completed'),
(5, 3, 3, 6, '03:30:00.000', 50, 5, 'Completed');
GO

-- Create Views
CREATE VIEW vw_EventSummary AS
SELECT 
    e.EventID,
    e.EventName,
    e.EventDate,
    e.City,
    e.Province,
    e.Status,
    COUNT(DISTINCT en.UserID) AS TotalParticipants,
    COUNT(DISTINCT c.CategoryID) AS TotalCategories
FROM Events e
LEFT JOIN Categories c ON e.EventID = c.EventID
LEFT JOIN Enrolments en ON e.EventID = en.EventID
GROUP BY e.EventID, e.EventName, e.EventDate, e.City, e.Province, e.Status;
GO

CREATE VIEW vw_ParticipantHistory AS
SELECT 
    u.UserID,
    u.FirstName + ' ' + u.LastName AS FullName,
    e.EventName,
    e.EventDate,
    c.CategoryName,
    c.Distance,
    en.Status AS EnrolmentStatus,
    r.FinishTime,
    r.OverallPosition,
    r.CategoryPosition
FROM Users u
LEFT JOIN Enrolments en ON u.UserID = en.UserID
LEFT JOIN Events e ON en.EventID = e.EventID
LEFT JOIN Categories c ON en.CategoryID = c.CategoryID
LEFT JOIN Results r ON en.EnrolmentID = r.EnrolmentID
WHERE u.RoleID = 3;
GO

-- Check everything
SELECT 'SUCCESS!' AS Status;
SELECT COUNT(*) AS TotalTables FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
SELECT * FROM vw_EventSummary;
SELECT * FROM vw_ParticipantHistory;