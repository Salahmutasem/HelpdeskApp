
Copy

CREATE DATABASE HelpdeskDB;
GO

USE HelpdeskDB;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(200) NOT NULL UNIQUE,
    Password VARCHAR(200) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Description TEXT NOT NULL,
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    CreatedBy INT FOREIGN KEY REFERENCES Users(Id),
    Status VARCHAR(20) DEFAULT 'Open',
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0
);

CREATE TABLE TicketComments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT FOREIGN KEY REFERENCES Tickets(Id),
    CommentText TEXT NOT NULL,
    CreatedBy INT FOREIGN KEY REFERENCES Users(Id),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

INSERT INTO Users (FullName, Email, Password) VALUES ('Admin User', 'admin@helpdesk.com', 'Admin@123');
INSERT INTO Users (FullName, Email, Password) VALUES ('salah mutasem', 'salah@helpdesk.com', 'Test@1234');

INSERT INTO Categories (Name) VALUES ('Hardware');
INSERT INTO Categories (Name) VALUES ('Software');
INSERT INTO Categories (Name) VALUES ('Network');
INSERT INTO Categories (Name) VALUES ('General');

INSERT INTO Tickets (Title, Description, CategoryId, CreatedBy, Status)
VALUES ('Laptop not turning on', 'My laptop stopped working after the update yesterday.', 1, 1, 'Open');

INSERT INTO Tickets (Title, Description, CategoryId, CreatedBy, Status)
VALUES ('Cannot install VS Code', 'Getting an error when trying to install Visual Studio Code on my machine.', 2, 2, 'InProgress');

INSERT INTO Tickets (Title, Description, CategoryId, CreatedBy, Status)
VALUES ('Wi-Fi keeps disconnecting', 'The office Wi-Fi drops every 10 minutes.', 3, 1, 'Open');

INSERT INTO TicketComments (TicketId, CommentText, CreatedBy)
VALUES (2, 'We are looking into this issue.', 1);
GO