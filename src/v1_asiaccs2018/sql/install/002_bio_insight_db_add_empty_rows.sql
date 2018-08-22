-- Insert empty string
SET IDENTITY_INSERT StringValue ON
INSERT StringValue(strUId, strValue, strHash, strType)
VALUES (0, '', 0x, 0)
SET IDENTITY_INSERT StringValue OFF
GO

SET IDENTITY_INSERT StringValueSMLN ON
INSERT StringValueSMLN(strUId, strValue, strHash)
VALUES (0, '', 0x)
SET IDENTITY_INSERT StringValueSMLN OFF
GO

SET IDENTITY_INSERT StringValueSRCN ON
INSERT StringValueSRCN(strUId, strValue, strHash)
VALUES (0, '', 0x)
SET IDENTITY_INSERT StringValueSRCN OFF
GO

SET IDENTITY_INSERT StringValuePKGN ON
INSERT StringValuePKGN(strUId, strValue, strHash)
VALUES (0, '', 0x)
SET IDENTITY_INSERT StringValuePKGN OFF
GO

SET IDENTITY_INSERT StringValuePATH ON
INSERT StringValuePATH(strUId, strValue, strHash)
VALUES (0, '', 0x)
SET IDENTITY_INSERT StringValuePATH OFF
GO

SET IDENTITY_INSERT StringValueMTHD ON
INSERT StringValueMTHD(strUId, strValue, strHash)
VALUES (0, '', 0x)
SET IDENTITY_INSERT StringValueMTHD OFF
GO

SET IDENTITY_INSERT StringValueFILD ON
INSERT StringValueFILD(strUId, strValue, strHash)
VALUES (0, '', 0x)
SET IDENTITY_INSERT StringValueFILD OFF
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

SET IDENTITY_INSERT BinaryObjectContent ON
INSERT BinaryObjectContent(bocUId, bocHash, bocContent, bocLength)
VALUES (0, 0x, 0x, 0)
SET IDENTITY_INSERT BinaryObjectContent OFF
GO

SET IDENTITY_INSERT BinaryObject ON
INSERT BinaryObject(
    bioUId, bioDstcatDataSetApplicationCategoryId, bioBocContentId,
    bioBioParentApkId, bioBopPathId,
    bioHash, bioFileName,
    bioRankInCategory, bioIsRoot, bioProcessingStage) 
VALUES (
    0, 1, 0,
    0, 0,
    0x, '',
    0, 0, 0)
SET IDENTITY_INSERT BinaryObject OFF
GO

SET IDENTITY_INSERT BinaryObjectPath ON
INSERT BinaryObjectPath(bopUId, bopBopParentId, bopName, bopParentPath)
VALUES (0, 0, '', '')
SET IDENTITY_INSERT BinaryObjectPath OFF
GO

SET IDENTITY_INSERT JavaType ON
INSERT INTO JavaType (
  jtypUId, jtypBioParentApkId, jtypBocParentContentId, jtypAccessControl, 
  jtypStrPackageNameId, jtypStrSmaliFullNameId, jtypStrPathId, jtypStrFileNameId,
  jtypIsClass, jtypIsInterface, jtypIsFinal, jtypIsEnum, jtypIsAbstract,
  jtypIsAnnotation, jtypIsStatic, jtypIsReferenceOnly ,
  jtypJtypOuterClassId, jtypJtypSuperClassId, jtypDbgSourceNotFound)
VALUES(
  0, 0, 0, 0,
  0, 0, 0, 0,
  0, 0, 0, 0, 0,
  0, 0, 1, 
  0, 0, 0
)
SET IDENTITY_INSERT JavaType OFF
GO

SET IDENTITY_INSERT DataSet ON
INSERT INTO DataSet (dstUId, dstName, dstSource, dstBioCount, dstDownloadDateBeg, dstDownloadDateEnd) 
VALUES (0, '0', '0', 0, '01-01-1900', '01-01-1900')
INSERT INTO DataSet (dstUId, dstName, dstSource, dstBioCount, dstDownloadDateBeg, dstDownloadDateEnd) 
VALUES (1, 'TOP 100 PerCategory', 'LERSSE/UBC', 0, '06-01-2015', '06-30-2015')
INSERT INTO DataSet (dstUId, dstName, dstSource, dstBioCount, dstDownloadDateBeg, dstDownloadDateEnd) 
VALUES (2, 'Random 150K', 'Sophos150K', 0, '04-01-2016', '06-01-2016')
SET IDENTITY_INSERT DataSet OFF
GO

SET IDENTITY_INSERT DataSetApplicationCategories ON
INSERT INTO DataSetApplicationCategories (dstcatUId, dstcatDstDataSetId, dstcatApcApplicationCategoryId, dstcatBioCount) 
VALUES (0, 0, 1, 0)
SET IDENTITY_INSERT DataSetApplicationCategories OFF
GO

INSERT INTO DataSetApplicationCategories (dstcatDstDataSetId, dstcatApcApplicationCategoryId, dstcatBioCount)
SELECT 1, apcUId, 0
FROM ApplicationCategory
GO

INSERT INTO DataSetApplicationCategories (dstcatDstDataSetId, dstcatApcApplicationCategoryId, dstcatBioCount)
SELECT 2, apcUId, 0
FROM ApplicationCategory
GO

SET IDENTITY_INSERT JavaTypeMethod ON
INSERT INTO JavaTypeMethod (jtmUId, jtmJtypInTypeId, jtmStrSmaliNameId, jtmJtypReturnTypeId, jtmAccessControl, jtmIsAbstract, jtmIsConstructor, jtmIsStatic, jtmSourceCodeIndexBeg, jtmSourceCodeIndexEnd) 
VALUES (0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT JavaTypeMethod OFF
GO

SET IDENTITY_INSERT JavaTypeField ON
INSERT INTO JavaTypeField (jtfUId, jtfJtypInTypeId, jtfStrSmaliNameId, jtfJtypOfTypeId, jtfAccessControl, jtfIsArray, jtfIsStatic, jtfIsFinal, jtfIsSynthetic, jtfIsEnum, jtfSourceCodeIndex) 
VALUES (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT JavaTypeField OFF
GO

SET IDENTITY_INSERT LibraryPropertyTypes ON
INSERT INTO LibraryPropertyTypes (lptUId, lptName, lptDescription) VALUES (1, 'Advertisement', 'A library that provides advertisement capabilities')
INSERT INTO LibraryPropertyTypes (lptUId, lptName, lptDescription) VALUES (2, 'Analytics', 'A library that provides analytics on how users use the app')
SET IDENTITY_INSERT LibraryPropertyTypes OFF
GO