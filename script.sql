-- =========================
-- СОЗДАНИЕ БД
-- =========================
CREATE DATABASE GameShop;
GO

USE GameShop;
GO

-- =========================
-- РОЛИ
-- =========================
CREATE TABLE Role (
    Id_Role INT IDENTITY PRIMARY KEY,
    Name_Role NVARCHAR(50) NOT NULL
);

-- =========================
-- ПОЛЬЗОВАТЕЛИ
-- =========================
CREATE TABLE [User] (
    Id_User INT IDENTITY PRIMARY KEY,
    Login NVARCHAR(50),
    Password NVARCHAR(50),
    Email NVARCHAR(100),
    Role_Id INT,
    FOREIGN KEY (Role_Id) REFERENCES Role(Id_Role)
);

-- =========================
-- ЖАНРЫ
-- =========================
CREATE TABLE Genre (
    Id_Genre INT IDENTITY PRIMARY KEY,
    Name_Genre NVARCHAR(100)
);

-- =========================
-- РАЗРАБОТЧИКИ
-- =========================
CREATE TABLE Developer (
    Id_Developer INT IDENTITY PRIMARY KEY,
    Name_Developer NVARCHAR(100)
);

-- =========================
-- ИЗДАТЕЛИ
-- =========================
CREATE TABLE Publisher (
    Id_Publisher INT IDENTITY PRIMARY KEY,
    Name_Publisher NVARCHAR(100)
);

-- =========================
-- ИГРЫ
-- =========================
CREATE TABLE Game (
    Id_Game INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(200),
    Description NVARCHAR(MAX),
    Price DECIMAL(10,2),
    Discount FLOAT,
    Genre_Id INT,
    Developer_Id INT,
    Publisher_Id INT,
    ReleaseDate DATE,
    Image NVARCHAR(255),

    FOREIGN KEY (Genre_Id) REFERENCES Genre(Id_Genre),
    FOREIGN KEY (Developer_Id) REFERENCES Developer(Id_Developer),
    FOREIGN KEY (Publisher_Id) REFERENCES Publisher(Id_Publisher)
);

-- =========================
-- БИБЛИОТЕКА (ПОКУПКИ)
-- =========================
CREATE TABLE UserGame (
    Id INT IDENTITY PRIMARY KEY,
    User_Id INT,
    Game_Id INT,
    PurchaseDate DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (User_Id) REFERENCES [User](Id_User),
    FOREIGN KEY (Game_Id) REFERENCES Game(Id_Game)
);

-- =========================
-- РЕЙТИНГИ
-- =========================
CREATE TABLE Review (
    Id_Review INT IDENTITY PRIMARY KEY,
    User_Id INT,
    Game_Id INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(500),

    FOREIGN KEY (User_Id) REFERENCES [User](Id_User),
    FOREIGN KEY (Game_Id) REFERENCES Game(Id_Game)
);

-- =========================
-- ЗАПОЛНЕНИЕ ДАННЫМИ
-- =========================

INSERT INTO Role VALUES ('Admin'), ('User');

INSERT INTO [User] (Login, Password, Email, Role_Id)
VALUES 
('admin', '123', 'admin@mail.com', 1),
('user', '123', 'user@mail.com', 2);

INSERT INTO Genre VALUES
('Action'), ('RPG'), ('Shooter'), ('Strategy'), ('Indie');

INSERT INTO Developer VALUES
('Valve'), ('CD Projekt'), ('Rockstar'), ('Game Science'), ('Indie Studio');

INSERT INTO Publisher VALUES
('Steam'), ('EA'), ('Ubisoft'), ('Game Science Publishing'), ('Indie Publisher');

-- Добавляем игры (включая FEMBOY FUTA HOUSE и Black Myth: Wukong)
INSERT INTO Game (Title, Description, Price, Discount, Genre_Id, Developer_Id, Publisher_Id, ReleaseDate, Image)
VALUES
('CS2', 'Шутер от первого лица', 0, 0.1, 3, 1, 1, '2023-09-27', 'cs2.jpg'),
('Cyberpunk 2077', 'RPG в будущем', 1999, 0.2, 2, 2, 2, '2020-12-10', 'cyberpunk.jpg'),
('GTA V', 'Открытый мир', 1499, 0.15, 1, 3, 3, '2013-09-17', 'gta5.jpg'),
('Dota 2', 'MOBA', 0, 0, 1, 1, 1, '2013-07-09', 'dota2.jpg'),
('Witcher 3', 'Фэнтези RPG', 999, 0.25, 2, 2, 2, '2015-05-19', 'witcher3.jpg'),
('FEMBOY FUTA HOUSE', 'Необычная инди-игра для ценителей', 299, 0.0, 5, 5, 5, '2024-02-14', 'ffh.jpg'),
('Black Myth: Wukong', 'Ролевой экшен по мотивам китайской мифологии', 3599, 0.0, 1, 4, 4, '2024-08-20', 'wukong.jpg');

-- =========================
-- ЗАПОЛНЕНИЕ ОТЗЫВОВ (Review)
-- =========================
INSERT INTO Review (User_Id, Game_Id, Rating, Comment)
VALUES
-- Отзывы на CS2 (Id = 1)
(2, 1, 4, 'Отличный шутер, но иногда попадаются читеры на серверах.'),
-- Отзывы на Cyberpunk 2077 (Id = 2)
(1, 2, 5, 'После выхода дополнения Phantom Liberty игра стала настоящим шедевром!'),
-- Отзывы на GTA V (Id = 3)
(2, 3, 5, 'Классика. Обожаю играть в GTA Online с друзьями.'),
-- Отзывы на Dota 2 (Id = 4)
(1, 4, 3, 'Токсичное комьюнити, но затягивает на тысячи часов.'),
-- Отзывы на Witcher 3 (Id = 5)
(2, 5, 5, 'Лучшая игра десятилетия, невероятный сюжет и музыка.'),
-- Отзывы на FEMBOY FUTA HOUSE (Id = 6)
(1, 6, 5, 'Купил случайно... Сюжет 10/10, геймплей на кончиках пальцев!'),
-- Отзывы на Black Myth: Wukong (Id = 7)
(2, 7, 5, 'Обезьяна бьет палкой! Невероятно красивая графика и крутые боссы.');

-- =========================
-- ИМИТАЦИЯ ПОКУПОК В БИБЛИОТЕКЕ
-- =========================
-- Дадим пользователю 'user' несколько купленных игр для отображения в Библиотеке
INSERT INTO UserGame (User_Id, Game_Id)
VALUES
(2, 1), -- user купил CS2
(2, 3), -- user купил GTA V
(2, 7); -- user купил Black Myth: Wukong