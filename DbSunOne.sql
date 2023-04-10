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
	Active bit not null,
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
	Published bit not null,
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
	LocationID int,
	District int,
	Ward int,
	CrateDate datetime,
	Password nvarchar(50),
	Salt nchar(6),
	LastLogin datetime,
	Active bit not null
);
go

create table Locations (
	LocationID int identity primary key,
	Name nvarchar(100),
	Type nvarchar(20),
	Slug nvarchar(100),
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
	Deleted bit not null,
	Paid bit not null,
	PaymentDate datetime,
	PaymentID int,
	TotalMoney int,
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
	Published bit not null,
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
	BestSellers bit not null,
	HomeFlag bit not null,
	Active bit not null,
	Tags nvarchar(MAX),
	Title nvarchar(255),
	Alias nvarchar(255),
	MetaDesc nvarchar(255),
	MetaKey nvarchar(255),
	UnitslnStock int default 0
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
	Published bit not null,
	Alias nvarchar(255),
	CreateDate datetime,
	Author nvarchar(255),
	AccountID int,
	Tags nvarchar(MAX),
	CatID int,
	isHot bit not null,
	isNewfeed bit not null,
	MetaKey nvarchar(255),
	MetaDesc nvarchar(255),
	Views int
);
go

create table TransactStatus (
	TransactStatusID int identity primary key,
	Status nvarchar(50),
	Description nvarchar(MAX),
	Class varchar(50)
);
go

create table AttributesPrices (
	AttributesPriceID int identity primary key,
	AttributeID int,
	ProductID int,
	Price int,
	Active bit not null
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

alter table Customers
add foreign key (LocationID) references Locations(LocationID)

alter table Customers
add foreign key (District) references Locations(LocationID)

alter table Customers
add foreign key (Ward) references Locations(LocationID)

alter table Orders
add foreign key (City) references Locations(LocationID)

alter table Orders
add foreign key (District) references Locations(LocationID)

alter table Orders
add foreign key (Ward) references Locations(LocationID)

alter table OrderDetails
add foreign key (ProductID) references Products(ProductID)

insert into Roles
values (1, 'Admin', N'Quản lý'),
	   (2, 'Staff', N'Nhân viên');
go

insert into Accounts
values ('0123456789', 'abc@123', 'admin', '123456', 1, 'ADMIN', 1, '', ''),
	   ('0123456789', 'abc@123', '123', '123456', 1, '123', 2, '', '');
go

insert into Categories(CatName, Published)
values  (N'Cà Vạt', 1),
		(N'Dây Nịt', 1),
		(N'Khăn', 1),
		(N'Nón', 1),
		(N'Thắt Lưng', 1),
		(N'Vớ', 1),
		(N'Khác', 1);
go

insert into TransactStatus
values (N'Chờ xác nhận', N'Đang chờ người bán xác nhận', 'label label-info'),
       (N'Chờ lấy hàng', N'Đã xác nhận và đang chuẩn bị hàng', 'label label-warning'),
	   (N'Đang giao', N'Đơn hàng đang được vận chuyển', 'label label-primary'),
	   (N'Đã giao thành công', N'Đơn hàng đã được giao', 'label label-success'),
	   (N'Đã hủy', N'Đơn hàng đã được hủy', 'label label-danger'),
	   (N'Trả hàng', N'Đơn hàng đã được hoàn trả', 'label label-info');
go