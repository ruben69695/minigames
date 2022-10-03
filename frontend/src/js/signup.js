const email_form = document.getElementById('email');
const username_form = document.getElementById('username');
const password_form = document.getElementById('password');
const confirm_password_form = document.getElementById('confirm_password');
const signup_button = document.getElementById('btn_signup');
const signup_button_loading = document.getElementById('btn_signup_loading');
const alert_message = document.getElementById('alert_message');

const signup_async = (email, username, password, confirm_password) => {
    var xhttp = new XMLHttpRequest();

    signup_button.style.display = 'none';
    signup_button_loading.style.display = ''

    xhttp.onreadystatechange = (event) => {

        if (xhttp.readyState == 4) {

            signup_button.style.display = '';
            signup_button_loading.style.display = 'none'

            if (xhttp.status === 200) {
                sessionStorage.setItem('access_dto', xhttp.responseText);
                window.location.href = "index.html";
            }
            else if (xhttp.status === 409)
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
        "username": username,
        "password": password,
        "passwordConfirmation": confirm_password
    });
    xhttp.open("POST", "https://localhost:5001/identity/signup", true);
    xhttp.setRequestHeader('Content-Type', 'application/json');
    xhttp.send(jsonBody);
}

const show_alert = (type, message) => {
    alert_message.style.opacity = 1;
    alert_message.className = type;
    alert_message.innerHTML = message;
}

signup_button.onclick = (event) => {

    var email = email_form.value;
    var username = username_form.value;
    var password = password_form.value;
    var confirm_password = confirm_password_form.value;

    if (email == undefined || email == null || email == '') {
        show_alert('alert alert-warning', '&#128548 Email is required &#128548');
    }
    else if (username == undefined || username == null || username == '') {
        show_alert('alert alert-warning', '&#128548 Username is required &#128548');
    }
    else if (password == undefined || password == null || password == '') {
        show_alert('alert alert-warning', '&#128548 Password is required &#128548');
    }
    else if (confirm_password == undefined || confirm_password == null || confirm_password == '') {
        show_alert('alert alert-warning', '&#128548 Confirm password is required &#128548');
    }
    else if (password != confirm_password) {
        show_alert('alert alert-warning', '&#128548 Passwords do not match &#128548');
    }
    else {
        signup_async(email, username, password, confirm_password);
    }

    return false;
}