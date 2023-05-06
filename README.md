# Customer Registration Application
This project is an application that aims to keep a customer list by verifying the information of customers registered via KPS (Identity Sharing Service).

## Technologies Used
Dapper: Used for database operations.
Redis Cache: Used to improve performance.
Swagger: Used to create API documentation.
JWT: Used for authentication.
Restclient: Used to communicate with KPS.

## Requirements
.NET 7.0 SDK
Visual Studio 2019 or newer
Redis Server

## Installation and Running
Clone or download the project.
bash
Copy code
git clone https://github.com/username/project-name.git
In the appsettings.json file located in the project's root directory, set the database connection string in the ConnectionStrings section and the Redis connection string in the RedisConfiguration section correctly.

```json
{
"JwtSettings": {
"Issuer": "https://localhost:7231/",
"SigningKey": "sigortamnetcustomerapp.06/05/2023-11:17",
"Expire": 3
},
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=Sigortamnet;Trusted_Connection=True;MultipleActiveResultSets=true"
},
"RedisConfiguration": {
"ConnectionString": "localhost:6379"
}
}
```
Open the project files and select the Sigortamnet.CustomerApp.WebApi project in Visual Studio.

Choose the Debug or Release configuration for the project in Visual Studio and run it.

## API Documentation
Swagger is used for API documentation. You can visit https://localhost:7231/swagger while the application is running.

## Contributing
If you want to contribute to this project, please open an issue first and then send a pull request to make the changes. Make sure you follow the guidelines for acceptable contributions.
