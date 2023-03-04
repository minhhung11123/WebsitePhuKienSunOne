create database dbSunOne
go

use dbSunOne
go

create table Accounts (
	AccountID int identity primary key,
	Phone varchar(10),
	Email nvarchar(50),
	Password nvarchar(50),
	Salt nchar(6),
	Active bit not null default 1,
	FullName nvarchar(150),
	RoleID int,
	LastLogin datetime,
	CreateDate datetime
);
go

create table Categories (
	CatID int identity primary key,
	CatName nvarchar(250),
	Description nvarchar(MAX),
	ParentID int,
	Levels int,
	Ordering int,
	Published int not null default 1,
	Thumb nvarchar(250),
	Title nvarchar(250),
	Alias nvarchar(250),
	MetaDesc nvarchar(250),
	MetaKey nvarchar(250),
	Cover nvarchar(255),
	SchemaMarkup nvarchar(MAX)
);
go

create table Customers (
	CustomerID int identity primary key,
	FullName nvarchar(255),
	Birthday datetime,
	Avatar nvarchar(255),
	Address nvarchar(255),
	Email nchar(150),
	Phone varchar(10),
	Location int,
	District int,
	Ward int,
	CrateDate datetime,
	Password nvarchar(50),
	Salt nchar(6),
	LastLogin datetime,
	Active bit not null default 1
);
go

create table Locations (
	LocationID int identity primary key,
	Name nvarchar(100),
	Type nvarchar(20),
	Slug nvarchar(100),
	NameWithType nvarchar(255),
	PathWithType nvarchar(255),
	ParentCode int,
	Levels int
);
go

create table Orders (
	OrderID int identity primary key,
	CustomerID int,
	OrderDate datetime,
	ShipDate datetime,
	TransactStatusID int,
	Deleted bit,
	Paid bit,
	PaymentDate datetime,
	PaymentID int,
	Note nvarchar(MAX)
);
go

create table OrderDetails (
	OrderDetailID int identity primary key,
	OrderID int,
	ProductID int,
	OrderNumber int,
	Quantity int,
	Discount int,
	Total int,
	ShipDate datetime
);
go

create table Pages (
	PageID int identity primary key,
	PageName nvarchar(250),
	Contents nvarchar(MAX),
	Thumb nvarchar(250),
	Published bit not null default 1,
	Title nvarchar(250),
	MetaDesc nvarchar(250),
	MetaKey nvarchar(250),
	Alias nvarchar(250),
	CreateDate datetime,
	Ordering int
);
go

create table Products (
	ProductID int identity primary key,
	ProductName nvarchar(255) not null,
	ShortDesc nvarchar(255),
	Description nvarchar(MAX),
	CatID int,
	Price int,
	Discount int,
	Thumb nvarchar(255),
	Video nvarchar(255),
	DateCreated datetime,
	DateModified datetime,
	BestSellers bit not null default 0,
	HomeFlag bit not null default 0,
	Active bit not null default 1,
	Tags nvarchar(MAX),
	Title nvarchar(255),
	Alias nvarchar(255),
	MetaDesc nvarchar(255),
	MetaKey nvarchar(255),
	UnitslnStock int
);
go

create table Roles (
	RoleID int primary key,
	RoleName nvarchar(50),
	Description	nvarchar(50)
);
go

create table Shippers (
	ShipperID int primary key,
	ShipperName nvarchar(150),
	Phone nchar(10),
	Company nvarchar(150),
	ShipDate datetime
);
go

create table News (
	PostID int identity primary key,
	Title nvarchar(255),
	SContents nvarchar(255),
	Contents nvarchar(MAX),
	Thumb nvarchar(255),
	Published bit not null default 1,
	Alias nvarchar(255),
	CreateDate datetime,
	Author nvarchar(255),
	AccountID int,
	Tags nvarchar(MAX),
	CatID int,
	isHot bit,
	isNewfeed bit,
	MetaKey nvarchar(255),
	MetaDesc nvarchar(255),
	Views int
);
go

create table TransactStatus (
	TransactStatusID int identity primary key,
	Status nvarchar(50),
	Description nvarchar(MAX)
);
go

create table AttributesPrices (
	AttributesPriceID int identity primary key,
	AttributeID int,
	ProductID int,
	Price int,
	Active bit
);
go

create table Attributes (
	AttributeID int identity primary key,
	Name nvarchar(250)
);
go

alter table OrderDetails
add foreign key (OrderID) references Orders(OrderID)

alter table Orders
add foreign key (TransactStatusID) references TransactStatus(TransactStatusID)

alter table Orders
add foreign key (CustomerID) references Customers(CustomerID)

alter table Products
add foreign key (CatID) references Categories(CatID)

alter table AttributesPrices
add foreign key (ProductID) references Products(ProductID)

alter table AttributesPrices
add foreign key (AttributeID) references Attributes(AttributeID)

alter table Accounts
add foreign key (RoleID) references Roles(RoleID)
go
