# BabysFirstCalendar

Goals: 
To allow a user to self register and edit their account on the local database. Users will be able to add/edit notes/memories on an interactive calendar and upload photos. 

Good to Know:
Folder "Models" is for what the user sees and enters.
Folder "Views" is what the user physically sees
Folder "Controller" calls both Models and DatabaseBusinessLogic processes to submit information from the user
Folder “DatabaseModels” is for the database, includes parameters that the user may not see.
Folder “DatabaseBusinessLogic” is for backend programming that the user does not need to see (i.e. salt and hashing).
AccountProcessor.cs in DatabaseBusinessLogic contains the methodology for account sign up and maintenance

