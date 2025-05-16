# Project Overview
This project is a budgeting app used to track monthly expenses, income, and current balance to calculate the net gain or loss per month. It is written in C#, .NET 8, MVC architecture using HTML/CSS/JavaScript for front-end design. Firebase is used for datastores and the application is hosted on Azure Web Apps.

# Getting Started
The most recent publish may be viewed here: https://budgetingapp41674.azurewebsites.net
You may create a new account to login to the application. Passwords are hashed, but please do no use any sensitive information regardless.

# User Manual
Here we will walk through how to use the application
## Login Page
![Login Page](Demo%20Screenshots/Login.png)
This is the landing page. You will be able to use your username and password to login to your account.

## Create Account
![Create Account](Demo%20Screenshots/Create%20Account.png)
If you do not have an account, you can create one here.

## Dashboard
![Dashboard](Demo%20Screenshots/Dashboard.png)
After you login, you will be taken to the Dashboard. Here you will find a read-only table that includes your overall balance, your income and monthly expenses, and net monthly gain or loss (income - expenses).
A Chart.js Doughnut Chart is provided to give a visual representation of your monthly expenses.
You may add or subtract money directly to or from the balance.

## Edit Assets
![Edit Assets](Demo%20Screenshots/Edit%20Assets.png)
Here you may set your balance, income, and expenses. Once you have set the values you want, press submit and you will automatically be redirected to the Dashboard.

## What If
![What If](Demo%20Screenshots/What%20If.png)
What If is designed to be a hypothetical situation calculator. You may edit your balance, income, and expenses in the table and your total expenses and net will automatically update with each keystroke. 
None of the input data here will be saved to the database, so there is no need to revert any changes.

## Settings
![Settings](Demo%20Screenshots/Settings.png)
Currently, the only setting is to logout of your account. Once you press the logout button, your session will be deleted and you will be redirected to the login page.

# Flowchart
![Flowchart ](Demo%20Screenshots/Flowchart%20(Primary%20Scenario).png)  
Here is a basic flowchart for the application in the primary scenario.

# Technology Stack
This application is written in C# using .NET 8 MVC architecture for back-end design and HTML/CSS and vanilla JavaScript for front-end design. The project is coded in Visual Studio Community 2022 on a Windows 10 machine. NuGet packages BCrypt.Net-Netx (for hashing passwords) and Google.Cloud.Firestore (for interacting with the Firebase) are necessary to build the project. The database is a No-SQL Cloud Firestore and the project is hosted on Azure Web Apps.
