# Mini Games 
A mini project for Training Gym technical interview

[![IMAGE ALT TEXT](http://img.youtube.com/vi/NWPwuCEKPSk/0.jpg)](http://www.youtube.com/watch?v=NWPwuCEKPSk "Mini Games Test")

# Getting Started
1. Clone the repository
```bash
git clone https://github.com/ruben69695/minigames.git
```
2. Install .NET 6.0 SDK
```bash
https://dotnet.microsoft.com/en-us/download/dotnet/6.0
```
3. Open the backend directory with VSCode and build the solution
```bash
dotnet build minigames.sln
```
4. Run unit tests
```bash
dotnet test minigames.sln
```
5. In the backend directory create and trust dev certs and deploy the backend using docker compose
```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/snakeapp.pfx -p ruben123 -t

docker compose up --build -d
```
6. In the frontend directory install node modules
```bash
npm install
```
7. Create a new 'cert' directory in frontend to store the ssl certificate
```bash
mkdir cert
```
8. Inside 'cert' directory create the ssl certificate using mkcert tool
```bash
mkcert localhost
```
9.  Run the web server
```bash
npm run dev
```
8. Enjoy it on your browser!:beer:
- safari
- chrome


# Technologies
- C#
- .NET 6.0
- EF Core
- XUnit
- Seq
- JWT
- PostgreSQL
- HTML - CSS - JS
- Visual Studio Code
- Docker
- Git

# Game
Game based from https://www.w3schools.com/graphics/game_intro.asp

# Contribute
If you want to contribute, please, don't wait and send us a message to add you to collaborate in this open source project. We are waiting for your help. Happy coding!
