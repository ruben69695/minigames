var myGamePiece;
var myObstacles = [];
var myScore;
var myGameArea;

var access_dto = null;
var user_game_data = null;

var username_label = document.getElementById("label_username");
var record_label = document.getElementById("label_record");
var total_points_label = document.getElementById("label_total_points");

var game_root = document.getElementById("game_root");
var start_button = document.getElementById("start_game");
var restart_button = document.getElementById("restart_game");
var accelerate_button = document.getElementById("accelerate");

document.body.onload = (event) => {
    let data = sessionStorage.getItem('access_dto');

    if (data == null || data == undefined) {
        window.location.href = "login.html";
    }

    access_dto = JSON.parse(data);
    get_game_data_async();
}

const get_game_data_async = () => {
    var xhttp = new XMLHttpRequest();
    start_button.style.display = 'none';

    xhttp.onreadystatechange = (event) => {
        if (xhttp.readyState == 4) {
            if (xhttp.status === 200) {
                user_game_data = JSON.parse(xhttp.responseText);
                username_label.innerHTML = `Hello ${access_dto.username}!`;
                record_label.innerHTML = `Record: ${user_game_data.record}`;
                total_points_label.innerHTML = `Total Points: ${user_game_data.totalPoints}`;
                start_button.style.display = 'block';
            }
            else if (xhttp.status === 404) {
                console.log("Error: " + xhttp.responseText);
            }
            else if (xhttp.status === 400) {
                console.log("Error: " + xhttp.responseText);
            }
            else if (xhttp.status === 0) {
                console.log("Error, no conection to remote server");
            }
        }
    };

    xhttp.open("GET", "https://localhost:5001/gamedata/me", true);
    xhttp.setRequestHeader('Content-Type', 'application/json');
    xhttp.setRequestHeader('Authorization', 'Bearer ' + access_dto.token);
    xhttp.send();
}

const update_game_data_async = (points) => {
    var xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = (event) => {
        if (xhttp.readyState == 4) {
            if (xhttp.status === 200) {
                console.log("Game data updated" + xhttp.responseText);
            }
            else if (xhttp.status === 404) {
                console.log("Error: " + xhttp.responseText);
            }
            else if (xhttp.status === 400) {
                console.log("Error: " + xhttp.responseText);
            }
            else if (xhttp.status === 0) {
                console.log("Error, no conection to remote server");
            }
        }
    };

    const jsonBody = JSON.stringify({
        "points": points
    });
    xhttp.open("POST", "https://localhost:5001/gamedata/me", true);
    xhttp.setRequestHeader('Content-Type', 'application/json');
    xhttp.setRequestHeader('Authorization', 'Bearer ' + access_dto.token);
    xhttp.send(jsonBody);
}

start_button.onclick = (event) => {
    startGame();
    start_button.style.display = 'none';
    restart_button.style.display = 'block';
    accelerate_button.style.display = 'block';
    document.getElementById("game_separator").style.display = 'block';
}

accelerate_button.onmousedown = (event) => {
    if (myGameArea != null && !myGameArea.pause) {
        accelerate(-0.2);
    }
}

accelerate_button.onmouseup = (event) => {
    if (myGameArea != null && !myGameArea.pause) {
        accelerate(0.1);
    }
}

restart_button.onclick = (event) => {
    myGameArea.stop();
    myGameArea.clear();
    myGameArea = {};
    myGamePiece = {};
    myObstacles = [];
    myScore = {};
    game_root.innerHTML = "";
    accelerate_button.style.display = 'block';
    startGame();
}

function startGame() {
    myGameArea = new gamearea();
    myGamePiece = new component(30, 30, "red", 10, 120);
    myGamePiece.gravity = 0.05;
    myScore = new component("25px", 'system-ui', "black", 280, 40, "text");
    myGameArea.start();
}

function gamearea() {
    this.canvas = document.createElement("canvas");
    this.canvas.width = 480;
    this.canvas.height = 270;
    this.context = this.canvas.getContext("2d");
    this.pause = false;
    game_root.appendChild(this.canvas);
    this.frameNo = 0;
    this.start = () => {
        this.interval = setInterval(updateGameArea, 20);
    };
    this.clear = () => {
        this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    };
    this.stop = () => {
        clearInterval(this.interval);
        this.pause = true;
    };
}

function component(width, height, color, x, y, type) {
    this.type = type;
    this.score = 0;
    this.width = width;
    this.height = height;
    this.speedX = 0;
    this.speedY = 0;
    this.x = x;
    this.y = y;
    this.gravity = 0;
    this.gravitySpeed = 0;
    this.update = function () {
        ctx = myGameArea.context;
        if (this.type == "text") {
            ctx.font = this.width + " " + this.height;
            ctx.fillStyle = color;
            ctx.fillText(this.text, this.x, this.y);
        } else {
            ctx.fillStyle = color;
            ctx.fillRect(this.x, this.y, this.width, this.height);
        }
    }
    this.newPos = function () {
        this.gravitySpeed += this.gravity;
        this.x += this.speedX;
        this.y += this.speedY + this.gravitySpeed;
        this.hitBottom();
    }
    this.hitBottom = function () {
        var rockbottom = myGameArea.canvas.height - this.height;
        if (this.y > rockbottom) {
            this.y = rockbottom;
            this.gravitySpeed = 0;
        }
    }
    this.crashWith = function (otherobj) {
        var myleft = this.x;
        var myright = this.x + (this.width);
        var mytop = this.y;
        var mybottom = this.y + (this.height);
        var otherleft = otherobj.x;
        var otherright = otherobj.x + (otherobj.width);
        var othertop = otherobj.y;
        var otherbottom = otherobj.y + (otherobj.height);
        var crash = true;
        if ((mybottom < othertop) || (mytop > otherbottom) || (myright < otherleft) || (myleft > otherright)) {
            crash = false;
        }
        return crash;
    }
}

function updateGameArea() {
    var x, height, gap, minHeight, maxHeight, minGap, maxGap;
    for (i = 0; i < myObstacles.length; i += 1) {
        if (myGamePiece.crashWith(myObstacles[i])) {
            myGameArea.stop();
            accelerate_button.style.display = 'none';
            update_game_data_async(myGameArea.frameNo);
            return;
        }
    }
    if (!myGameArea.pause) {
        myGameArea.clear();
        myGameArea.frameNo += 1;
        if (myGameArea.frameNo == 1 || everyinterval(150)) {
            x = myGameArea.canvas.width;
            minHeight = 20;
            maxHeight = 200;
            height = Math.floor(Math.random() * (maxHeight - minHeight + 1) + minHeight);
            minGap = 50;
            maxGap = 200;
            gap = Math.floor(Math.random() * (maxGap - minGap + 1) + minGap);
            myObstacles.push(new component(10, height, "green", x, 0));
            myObstacles.push(new component(10, x - height - gap, "green", x, height + gap));
        }
        for (i = 0; i < myObstacles.length; i += 1) {
            myObstacles[i].x += -1;
            myObstacles[i].update();
        }
        myScore.text = "SCORE: " + myGameArea.frameNo;
        myScore.update();
        myGamePiece.newPos();
        myGamePiece.update();

        if (myGameArea.frameNo > user_game_data.record) {
            user_game_data.record = myGameArea.frameNo;
            record_label.innerHTML = `Record: ${user_game_data.record}`;
        }
        user_game_data.totalPoints += 1;
        total_points_label.innerHTML = `Total Points: ${user_game_data.totalPoints}`;
    }
}

function everyinterval(n) {
    if ((myGameArea.frameNo / n) % 1 == 0) { return true; }
    return false;
}

function accelerate(n) {
    myGamePiece.gravity = n;
}