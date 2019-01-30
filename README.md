# Student Outcomes Assessment and Evaluation Plan

### INTRODUCTION
>The software is a computerized assessment plan that contains student outcomes coming from a course syllabus, performance indicators, summative course, assessment tool to be graded from blackboard account or by the professors, assessment targets and results, quarter and school year, evaluation, and recommendation. It is an online evaluation plan for the professors for easier computing and evaluating of the students’ grades if the desired assessment of Student Outcomes has been met. The program imports data from Microsoft Excel to be processed. The Excel file contains the grades of the students that is manually recorded by the professors or downloaded from the Blackboard account.
>
>ASP.Net MVC 5 was used together with Entity Framework 6. The database were created with the help of XAMPP's phpmyadmin. 

### FUNCTIONS
>There are two account types available: **Admin** and **User (Instructor)**. There are functionalities which are restricted for the admin only, vice versa. On the other hand, they also have their common function which can be used by both account types.
> 
> **ADMIN**
> - **Create** and **Delete** existing user accounts. 
> - **Edit** relationship between Student Outcome, Performance Indicator, Course, Assessment Tools, and Target.
> - **View file logs** or the plan generation activities of the user.
> - **Change Password**.
> 
> **USER**
> - **Generate** Assessment Plan.
> - View all generated files, which varies per user, and has the ability to **Download** the file.
> - **Change Password**.

### HOW TO RUN
>  1. Create first the Database, you can refer to the ERD included. You can use any aside from phpmyadmin as long as it can be detected by ADO.NET EF6. 
>  2. Connect the Database in the program using the ADO.Net. After that, edit the Web.config and edit the connection strings' information including the name and then change all the **projectEntities** instances within the program with your new entity name.
>  3. Download all the package dependencies listed in packages folder using Visual Studio's NuGet Package Manager.
>  4. Try to build the program, the browser must redirect you to the Login Page.

### DISCLAIMER
> This is made for school project purpose only.
> COE131L-C1-2Q1819 Group 2
