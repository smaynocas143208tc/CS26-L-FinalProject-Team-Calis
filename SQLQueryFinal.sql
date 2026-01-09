CREATE DATABASE UserAuthDB;

SELECT * FROM Books;
SELECT * FROM Members;
SELECT * FROM BorrowingTransactions;
SElECT * FROM BookCopies;
SELECT * FROM BookCopies WHERE BookID = 1;



IF OBJECT_ID('dbo.BorrowingTransactions', 'U') IS NOT NULL DROP TABLE dbo.BorrowingTransactions;
IF OBJECT_ID('dbo.BookCopies', 'U') IS NOT NULL DROP TABLE dbo.BookCopies;


IF OBJECT_ID('dbo.Members', 'U') IS NOT NULL DROP TABLE dbo.Members;
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;







CREATE TABLE Members (
    MemberID INT PRIMARY KEY IDENTITY(1,1),  
    UserName NVARCHAR(50) NOT NULL,          
    PasswordHash NVARCHAR(MAX) NOT NULL,     
    UserRole NVARCHAR(20) DEFAULT 'Member', 
    Name NVARCHAR(100) NOT NULL,            
    Address NVARCHAR(255),                 
    Email NVARCHAR(100) UNIQUE,             
    Phone NVARCHAR(20)                       
);




CREATE TABLE Books (
    BookID INT PRIMARY KEY, 
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100)NOT NULL,
    ISBN NVARCHAR(50) NOT NULL UNIQUE,
    Pages INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    ResourceType NVARCHAR(50) NOT NULL,
    Published NVARCHAR(50) NOT NULL
);




CREATE TABLE BookCopies (
    CopyID NVARCHAR(50) PRIMARY KEY,       
    BookID INT NOT NULL,                   
    Condition NVARCHAR(50) DEFAULT 'Good',
    Status NVARCHAR(50) NOT NULL,         
    CONSTRAINT FK_Book_Copy FOREIGN KEY (BookID) REFERENCES Books(BookID)
);




DECLARE @id INT = 1;                      
SELECT b.*, c.CopyID, c.Status, c.Condition
FROM Books b
INNER JOIN BookCopies c ON b.BookID = c.BookID
WHERE b.BookID = @id;




CREATE TABLE BorrowingTransactions (
    TransactionID INT PRIMARY KEY IDENTITY(1,1), --
    MemberID INT NOT NULL,
    BookID INT NOT NULL,                    
    CopyID NVARCHAR(50) NOT NULL,
    ConditionOnReturn NVARCHAR(100) NULL,
    BorrowDate DATETIME DEFAULT GETDATE(), 
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL, 
    FineAmount DECIMAL(18, 2) DEFAULT 0,
    CONSTRAINT FK_Member FOREIGN KEY (MemberID) REFERENCES Members(MemberID),
    CONSTRAINT FK_BookCopy FOREIGN KEY (CopyID) REFERENCES BookCopies(CopyID),
    CONSTRAINT FK_Transactions_Books FOREIGN KEY (BookID) REFERENCES Books(BookID)
);






ALTER TABLE Books 
ADD IsDeleted BIT NOT NULL DEFAULT 0;
UPDATE Books SET IsDeleted = 0;

ALTER TABLE BorrowingTransactions 
ADD FineAmount DECIMAL(18, 2) DEFAULT 0;