<h1>Duplex.gg</h1>
<h3>ASP.NET Web App - SoftUni Web - Sep 2022</h3>


<h1>Duplex - League of Legends Task Completion Web App</h1>
    <p>This is a web application built with ASP.NET that allows administrators to host events and set a price to enter. Regular users have 5 different roles (Bronze, Silver, Gold, Platinum, Diamond) and a default role of Member. Every user can claim a daily reward, based on their current role. The higher the rank, the more coins they get daily. Every user can join an event and try to complete it. They have to verify that the game account they are using is actually theirs by changing their League of Legends icon to the one specified in the app. After verification, they can complete events and double their entry coins. To check if the task is completed, the app uses Riot Games' API to access their match history and validate whether they have completed the task or not. The app allows them to choose their League of Legends region. The web app has prizes, which only administrators can create, and every user can claim if they have enough coins to redeem.</p>

   <h2>Requirements Met</h2>
   <ul>
       <li>Use the Razor template engine for generating the UI</li>
       <ul>
           <li>Use sections and partial views</li>
       </ul>
        <li>Use Microsoft SQL Server as Database Service</li>
        <li>Use Entity Framework Core to access your database</li>
        <li>Use MVC Areas to separate different parts of your application (e.g. area for administration)</li>
        <li>Use responsive design based on Twitter Bootstrap / Google Material design</li>
        <li>Use the standard ASP.NET Identity System for managing Users and Roles</li>
        <ul>
            <li>Your registered users should have at least one of these roles: User and Administrator</li>
        </ul>
        <li>Implement error handling and data validation to avoid crashes when invalid data is entered</li>
        <ul>
            <li>Both client-side and server-side, even at the database(s)</li>
        </ul>
        <li>Use Dependency Injection</li>
        <li>Prevent security vulnerabilities like SQL Injection, XSS, CSRF, parameter tampering, etc.</li>
        <li>Follow the best practices for Object-Oriented design and high-quality code for the Web application:</li>
        <ul>
            <li>Use the OOP principles properly: data encapsulation, inheritance, abstraction and polymorphism</li>
            <li>Use exception handling properly</li>
            <li>Follow the principles of strong cohesion and loose coupling</li>
            <li>Correctly format and structure your code, name your identifiers and make the code readable</li>
            <li>Use caching where appropriate</li>
            <li>Support all major modern web browsers. Optionally, make the site as responsive as possible – think about tablets and smartphones.</li>
            <li>Make the user interface (UI) good-looking and easy to use. Providing a broken design will result in sanctioned functionality points</li>
        </ul>

    <h2> Bonuses </h2>
    <ul>
        <li>Using Google Drive API to upload the user profile picture</li>
        <li>Using Riot Games API to verify tasks and link user account</li>
    </ul>

    <h2>Prizes</h2>
    <p>The web app has prizes that only administrators can create, and every user can claim them if they have enough coins to redeem.</p>
    
    <h2>Installation and Usage</h2>
    <p>To install and use Duplex, follow these steps:</p>
    <ol>
        <li>Clone this repository.</li>
        <li>Open the project in Visual Studio.</li>
        <li>Configure the database connection string in appsettings.json.</li>
        <li>Configure the user secrets and API Keys needed for the work of Google Drive and Riot Games APIs</li>
        <li>Run the database migration by running the following command in the Package Manager Console:</li>
        <code>update-database</code>
        <li>Run the application.</li>
    </ol>
    
    <h2>Contributing</h2>
    <p>We welcome contributions to Duplex! If you find a bug, please submit an issue. If you want to contribute code, feel free to fork the repository and create a pull request.</p>
