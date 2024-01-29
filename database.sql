CREATE DATABASE classroom_management;
GO
USE classroom_management;
GO

-- Authorization table for accounts: administrators, class leaders and teachers
CREATE TABLE roles
(
	id INT PRIMARY KEY IDENTITY,
	role_name VARCHAR(50)
);
-- INSERT DATA FOR TABLE ROLES
GO
INSERT INTO roles(role_name) VALUES('ADMIN');
INSERT INTO roles(role_name) VALUES('LEADER_CLASS');
INSERT INTO roles(role_name) VALUES('TEACHER');
GO

-- TABLE ACCOUNT
CREATE TABLE accounts
(
 id INT PRIMARY KEY IDENTITY,
 username NVARCHAR(255) NOT NULL,
 password NVARCHAR(255) NOT NULL,
 fullname NVARCHAR(255) NOT NULL,
 role_id INT DEFAULT 3, -- Account teacher
 is_deleted BIT NOT NULL DEFAULT 0,   -- 0: chua xoa, 1: da xoa
 CONSTRAINT FK_account_role FOREIGN KEY(role_id) REFERENCES roles(id)
);
-- INITIAL ACCOUNT
GO
INSERT INTO accounts(username, password, fullname, role_id) VALUES(N'admin', '123', N'Quản trị viên', 1);
INSERT INTO accounts(username, password, fullname, role_id) VALUES(N'hungleader', '123', N'Nguyễn Bá Hùng', 2);
INSERT INTO accounts(username, password, fullname, role_id) VALUES(N'linhteacher', '123', N'Đỗ Thị Mai Linh', 3);
GO
-- TABLE CLASS
CREATE TABLE rooms
(
 id INT PRIMARY KEY IDENTITY,
 room_name NVARCHAR(100) NOT NULL,
 capacity INT NOT NULL,
 location NVARCHAR(100),
 leader_id INT,
 is_deleted BIT NOT NULL DEFAULT 0,   -- 0: chua xoa, 1: da xoa
 CONSTRAINT FK_room_leader FOREIGN KEY(leader_id) REFERENCES accounts(id)
);
GO
INSERT [dbo].[rooms] ([room_name], [capacity], [location]) VALUES 
('A5 201', '60', N'Khu A'),
('A5 202', '60', N'Khu A'),
('A5 203', '60', N'Khu A'),
('B 101', '60', N'Khu B'),
('B 201', '60', N'Khu B'),
('F 206', '60', N'Khu F1'),
('F 207', '60', N'Khu F1'),
('D 208', '60', N'Khu D'),
('D 209', '60', N'Khu D');

GO
-- Equipments
CREATE TABLE equipments
(
 id INT PRIMARY KEY IDENTITY,
 equipment_number VARCHAR(255) NOT NULL, 
 equipment_type NVARCHAR(255) NOT NULL, -- Loai trang bi: Laptop, PC, May chieu
 origin NVARCHAR(255) NOT NULL,
 production_year INT NOT NULL,
 voltage INT DEFAULT 220,	-- Mac dinh la 220V (Giong o Viet Nam)
 status BIT NOT NULL DEFAULT 1,    -- 1: Hoat dong(active), 2: Khong con hoat dong(inactive)
 room_id INT,
 is_deleted BIT NOT NULL DEFAULT 0,   -- 0: chua xoa, 1: da xoa
 CONSTRAINT FK_equip_room FOREIGN KEY(room_id) REFERENCES rooms(id)
);
GO
INSERT [dbo].[equipments] ([equipment_number], [equipment_type], [origin], [production_year], [voltage], [status]) VALUES (N'TIVI2001', N'Tivi', N'VN', 2015, 220, 0)
INSERT [dbo].[equipments] ([equipment_number], [equipment_type], [origin], [production_year], [voltage], [status]) VALUES (N'FAN2001', N'Quạt', N'VN', 2016, 220, 0)
INSERT [dbo].[equipments] ([equipment_number], [equipment_type], [origin], [production_year], [voltage], [status]) VALUES (N'ML001', N'Máy lạnh', N'NGA', 2015, 220, 1)
INSERT [dbo].[equipments] ([equipment_number], [equipment_type], [origin], [production_year], [voltage], [status]) VALUES (N'B2001', N'Bảng', N'NGA', 2023, 110, 1)
INSERT [dbo].[equipments] ([equipment_number], [equipment_type], [origin], [production_year], [voltage], [status]) VALUES (N'LOA2003', N'Loa', N'VN', 2023, 220, 0)
INSERT [dbo].[equipments] ([equipment_number], [equipment_type], [origin], [production_year], [voltage], [status]) VALUES (N'MIC2005', N'Micro', N'VN', 2023, 110, 0)

GO
CREATE TABLE bookings(
 id INT PRIMARY KEY IDENTITY,
 room_id INT,
 user_id INT,
 booking_date DATE,
 booking_status BIT,  -- 0: sáng, 1: chiều
 confirmation_status TINYINT, --0: chưa xác nhận, 1: Đã xác nhận, 2: Từ chối
 FOREIGN KEY(room_id) REFERENCES rooms(id),
 FOREIGN KEY(user_id) REFERENCES accounts(id)
);
