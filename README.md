# BabysFirstCalendar

Summary:
Through the Baby’s First Calendar applications, users can have calendar displays of special moments as their child grows. Users create an account and make notes/upload pictures on a calendar. The calendar displays snippets of the text they entered and displays a notification that there is a photo attached (if necessary). 

Users may reset passwords, update their account information, or request for reminders. The reminders are automated and sent to the user’s email account, depending on how often the user wants to be reminded.

Folders:
Content - Contains CSS script
Controllers - Written in C#, contains the controllers which process the View, Models, and DatabaseBusinessLogic
DataAccess - Written in C#. It is for accessing SQL
DatabaseBusinessLogic - Written in C#. It takes information from Controllers, processes DatabaseModels and connects to SQL.
DatabaseModels - Written in C#. It is for obtaining information that the user does not need to see, or send need-to-know information to DatabaseBusinessLogic
Models - Written in C#. It is the groundwork for what the user will need to enter for data and what the user sees in the View.
Scripts - Written in Javascript
Views - Written in HTML/Razor. It is what users see as they traverse the site.

Good to Know:
Folder "Models" is for what the user sees and enters.
Folder "Views" is what the user physically sees
Folder "Controller" calls both Models and DatabaseBusinessLogic processes to submit information from the user
Folder “DatabaseModels” is for the database, includes parameters that the user may not see.
Folder “DatabaseBusinessLogic” is for backend programming that the user does not need to see (i.e. salt and hashing).
AccountProcessor.cs in DatabaseBusinessLogic contains the methodology for account sign up and maintenance

