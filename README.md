# Project Overview
This project is a budgeting app used to track monthly expenses, income, and current balance to calculate the net gain or loss per month. It is written in C# .NET 8 MVC architecture using HTML/CSS/JavaScript for front-end design. Firebase is used for datastores and the application is hosted on Azure Web Apps.

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
After you login, you will be taken to the Dashboard. Here you will find a read-only table that includes your overall balance, your income and expenses, and net gain or loss (income - expenses) per month and per year.
A Chart.js Doughnut Chart is provided to give a visual representation of your monthlyexpenses.
You may add or subtract money directly to or from the balance.

## Edit Assets
![Edit Assets](Demo%20Screenshots/Edit%20Assets.png)
Here you may set your balance, income, and expenses. Once you have set the values you want, press submit and you will automatically be redirected to the Dashboard.

## What If
![What If](Demo%20Screenshots/What%20If.png)
What If is designed to be a hypothetical situation calculator. You may edit your balance, income, and expenses in the table and your total expenses and net will automatically update with each keystroke. 
None of the input data here will be saved to the database, so there is no need to revert any changes.

## Transactions
![Transactions](Demo%20Screenshots/Transactions.png)
Transactions are a way to visualize and keep track of non-recurring charges. Here you will see a list of all manually-entered transactions consisting of the transaction date, the merchant name, and the amount cost. 
You will also see your budgeted vs actual expenses based on your transaction history to help you see if you are staying within or exceeding your budget for each category.

## Add Transaction
![Add Transaction](Demo%20Screenshots/Add%20Transaction.png)
You may add a new transaction by entering the date, merchant name, and amount cost. The amount will automatically be deducted from your balance.

## Settings
![Settings](Demo%20Screenshots/Settings.png)
Currently, the only setting is to logout of your account. Once you press the logout button, your session will be deleted and you will be redirected to the login page.

# Flowchart
![Flowchart ](Demo%20Screenshots/Flowchart%20(Primary%20Scenario).png)  
Here is a basic flowchart for the application in the primary scenario.

# Technology Stack
This application is written in C# using .NET 8 MVC architecture for back-end design and HTML/CSS and JavaScript using jQuery for front-end design. The project is coded in Visual Studio Community 2022 on a Windows 10 machine. NuGet packages BCrypt.Net-Next (for hashing passwords) and Google.Cloud.Firestore (for interacting with the Firebase) are necessary to build the project. The database is a No-SQL Cloud Firestore and the project is hosted on Azure Web Apps.
