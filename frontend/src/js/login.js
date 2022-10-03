const email_form = document.getElementById('email');
const password_form = document.getElementById('password');
const login_button = document.getElementById('btn_login');
const login_button_loading = document.getElementById('btn_login_loading');
const alert_message = document.getElementById('alert_message');

const login_async = (email, password) => {
    var xhttp = new XMLHttpRequest();

    login_button.style.display = 'none';
    login_button_loading.style.display = 'block'

    xhttp.onreadystatechange = (event) => {

        if (xhttp.readyState == 4) {

            login_button.style.display = 'block';
            login_button_loading.style.display = 'none'

            if (xhttp.status === 200) {
                sessionStorage.setItem('access_dto', xhttp.responseText);
                window.location.href = "index.html";
            }
            else if (xhttp.status === 403)
            {
                error = JSON.parse(xhttp.responseText);
                show_alert('alert alert-warning', error.message);
            }
            else if (xhttp.status === 400) {
                error = JSON.parse(xhttp.responseText);
                show_alert('alert alert-warning', error.message ?? 'Bad request');
            }
            else if (xhttp.status === 0) {
                show_alert('alert alert-danger', '&#128561 Error, no connection with the remote server, status code: ' + xhttp.status + ' &#128561');
            }
        }

    };

    const jsonBody = JSON.stringify({
        "email": email,
        "password": password
    });
    xhttp.open("POST", "https://localhost:5001/identity/login", true);
    xhttp.setRequestHeader('Content-Type', 'application/json');
    xhttp.send(jsonBody);
}

const show_alert = (type, message) => {
    alert_message.style.opacity = 1;
    alert_message.className = type;
    alert_message.innerHTML = message;
}

login_button.onclick = (event) => {

    var email = email_form.value;
    var password = password_form.value;

    if (email == undefined || email == null || email == '') {
        show_alert('alert alert-warning', '&#128548 Email is required &#128548');
    }
    else if (password == undefined || password == null || password == '') {
        show_alert('alert alert-warning', '&#128548 Password is required &#128548');
    }
    else {
        login_async(email, password)
    }

    return false;
}