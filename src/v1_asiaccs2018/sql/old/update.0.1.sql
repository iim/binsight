-- Creates Application category table
CREATE TABLE ApplicationCategory
(
  apcUId INT IDENTITY(1,1) PRIMARY KEY,
  apcName NVARCHAR(256) NOT NULL,
  apcDescription NVARCHAR(256) NOT NULL,
)
GO

-- Insert all the categories
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('UNDEFINED', 'Not set')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('BOOKSANDREFS', 'Books & References')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('BUSINESS', 'Document editor/reader, package tracking, remote desktop, email management, job search')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('COMICS', 'Comic players, comic titles')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('COMMUNICATION', 'Messaging, chat/IM, dialers, address books, browsers, call management')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('EDUCATION', 'Exam preparations, study-aids, vocabulary, educational games, language learning')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('ENTERTAINMENT', 'Streaming video, Movies, TV, interactive entertainment')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('FINANCE', 'Banking, payment, ATM finders, financial news, insurance, taxes, portfolio/trading, tip calculators')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('HEALTHANDFITNESS', 'Personal fitness, workout tracking, diet and nutritional tips, health & safety etc.')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('LIBRARIESANDDEMOS', 'Software Libraries, technical demos')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('LIFESTYLE', 'Recipes, style guides')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('MEDIAANDVIDEO', 'Subscription movie services, remote controls, media/video players')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('MEDICAL', 'Drug & clinical references, calculators, handbooks for health-care providers, medical journals & news')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('MUSICANDAUDIO', 'Music services, radios, music players')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('NEWSANDMAGAZINES', 'Newspapers, news aggregators, magazines, blogging')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('PERSONALIZATION', 'Wallpapers, live wallpapers, home screen, lock screen, ringtones')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('PHOTOGRAPHY', 'Cameras, photo editing tools, photo management and sharing')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('PRODUCTIVITY', 'Notepad, to do list, keyboard, printing, calendar, backup, calculator, conversion')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('SHOPPING', 'Online shopping, auctions, coupons, price comparison, grocery lists, product reviews')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('SOCIAL', 'Social networking, check-in, blogging')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('SPORTS', 'Sports News & Commentary, score tracking, fantasy team management, game Coverage')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('TOOLS', 'Tools for Android devices')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('TRANSPORTATION', 'Public transportation, navigation tools, driving')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('TRAVELANDLOCAL', 'Maps, City guides, local business information, trip management tools')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('WALLPAPER', 'Wallpaper apps')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('WEATHER', 'Weather reports')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('WIDGET', 'Wigets')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_ACTION', 'Games - Action')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_ADVENTURE', 'Games - Adventure')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_ARCADE', 'Games - Arcade')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_BOARD', 'Games - Board')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_CARD', 'Games - Card')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_CASINO', 'Games - Casino')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_CASUAL', 'Games - Casual')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_EDUCATIONAL', 'Games - Educational')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_MUSIC', 'Games - Music')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_PUZZLE', 'Games - Puzzle')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_RACING', 'Games - Racing')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_ROLE_PLAYING', 'Games - Role Playing')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_SIMULATION', 'Games - Simulation')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_SPORTS', 'Games - Sports')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_STRATEGY', 'Games - Strategy')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_TRIVIA', 'Games - Trivia')
INSERT INTO ApplicationCategory (apcName, apcDescription) VALUES ('GAME_WORD', 'Games - Word')
GO

-- Table that contains all actual binary content.
CREATE TABLE BinaryObjectContent
(
  bocUId INT IDENTITY(1,1) PRIMARY KEY,
  bocHash NVARCHAR(256) NOT NULL,
  bocContent VARBINARY(MAX) NOT NULL,
  bocLength INT NOT NULL,
)
GO

CREATE UNIQUE INDEX IDX_bocHash ON BinaryObjectContent (bocHash)
GO

CREATE TABLE BinaryObject
(
  bioUId INT IDENTITY(1,1) PRIMARY KEY,
  bioBocContentId INT NOT NULL CONSTRAINT FK_bioBocContentId FOREIGN KEY REFERENCES BinaryObjectContent(bocUId),
  bioApcCategoryId INT NOT NULL CONSTRAINT FK_bioApcCategoryId FOREIGN KEY REFERENCES ApplicationCategory(apcUId),
  bioHash NVARCHAR(256) NOT NULL,
  bioFileName NVARCHAR(256) NOT NULL,
  bioRankInCategory INT NOT NULL,
  bioIsRoot INT NOT NULL,
)

CREATE NONCLUSTERED  INDEX IDX_bioHash ON BinaryObject (bioHash)
GO
