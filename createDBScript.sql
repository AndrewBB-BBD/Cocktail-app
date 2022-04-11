/*
Assumptions:
- The email attribute in UserLogin is validated before entered
- A user can't favourite a recipe more than once
- A recipe can't have more than one instance of a specific ingredient
- You can't delete instances from the category, flavourProfile, difficulty,
  RecipeType, measurement, ingredient tables
- The rating is inputted in the form 'X,X'
- A recipe discription isn't always necessary
- The average time it takes to make a cocktail is 15 min
*/

USE master;
GO

-- ALTER DATABASE coktailDB SET AUTO_CLOSE OFF

DROP DATABASE IF EXISTS cocktailDB;

CREATE DATABASE cocktailDB;
GO

USE cocktailDB;
GO

CREATE TABLE Category (
	categoryID INT PRIMARY KEY IDENTITY(1,1),
	categoryName VARCHAR(20) NOT NULL
);


CREATE TABLE FlavourProfile (
	flavourID INT PRIMARY KEY IDENTITY(1,1),
	flavourName VARCHAR(20) NOT NULL
);

CREATE TABLE Difficulty (
	difficultyID INT PRIMARY KEY IDENTITY(1,1),
	difficultyName VARCHAR(10) NOT NULL
);

CREATE TABLE RecipeType (
	typeID INT PRIMARY KEY IDENTITY(1,1),
	typeName VARCHAR(20) NOT NULL
);

CREATE TABLE Measurement (
	measurementID INT PRIMARY KEY IDENTITY(1,1),
	measurementName VARCHAR(15) NOT NULL
);

CREATE TABLE Ingredient (
	ingredientID INT PRIMARY KEY IDENTITY(1,1),
	categoryID INT NOT NULL,
	ingredientName VARCHAR(25),
	FOREIGN KEY (categoryID) REFERENCES Category(categoryID)
		ON UPDATE CASCADE
);

CREATE TABLE Recipe (
	recipeID INT PRIMARY KEY IDENTITY(1,1),
	flavourID INT NOT NULL,
	difficultyID INT NOT NULL,
	typeID INT NOT NULL,
	recipeName VARCHAR(50) NOT NULL,
	recipeDescription VARCHAR(100) NULL,
	recipeMethod VARCHAR(MAX) NOT NULL,
	recipeTime INT NOT NULL DEFAULT 15,
	recipeImage VARCHAR(MAX) NOT NULL,
	containtsAlcohol BIT NOT NULL,
	FOREIGN KEY (flavourID) REFERENCES FlavourProfile (flavourID),
	FOREIGN KEY (difficultyID) REFERENCES Difficulty (difficultyID),
	FOREIGN KEY (typeID) REFERENCES RecipeType (typeID)
);

CREATE TABLE IngredientMeasurement (
	ingredientID INT NOT NULL,
	recipeID INT NOT NULL,
	measurementID INT NOT NULL,
	measurementAmount VARCHAR(6) NOT NULL,
	FOREIGN KEY (ingredientID) REFERENCES Ingredient (ingredientID),
	FOREIGN KEY (recipeID) REFERENCES Recipe (recipeID),
	FOREIGN KEY (measurementID) REFERENCES Measurement(measurementID),
	CONSTRAINT PK_IngredientMesaurement PRIMARY KEY (ingredientID, recipeID)
);

CREATE TABLE UserLogin (
	userEmail VARCHAR(320) PRIMARY KEY,
	username VARCHAR(40) NOT NULL,
	userPassword BINARY(64) NOT NULL,
	salt UNIQUEIDENTIFIER NOT NULL
);

CREATE TABLE Rating (
	userEmail VARCHAR(320) NOT NULL,
	recipeID INT NOT NULL,
	numStars DECIMAL(2,1),
	ratingComment VARCHAR(300)
	FOREIGN KEY (userEmail) REFERENCES UserLogin (userEmail),
	FOREIGN KEY (recipeID) REFERENCES Recipe (recipeID),
	CONSTRAINT PK_Rating PRIMARY KEY (userEmail, recipeID)
);

CREATE TABLE Favourite (
	userEmail VARCHAR(320) NOT NULL,
	recipeID INT NOT NULL,
	FOREIGN KEY (userEmail) REFERENCES UserLogin (userEmail),
	FOREIGN KEY (recipeID) REFERENCES Recipe (recipeID),
	CONSTRAINT PK_Favourite PRIMARY KEY (userEmail, recipeID)
);
