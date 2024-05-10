
function checkEmail() {
    var email = document.getElementById("email").value;
    var errorElement = document.getElementById("error_message3");
    email = email.trim();
    if (email != "") {
        if (isEmail(email)) {
            errorElement.innerHTML = "";
            return true;
        } else {
            errorElement.innerHTML = "Invalid email address";
            return false;
        }
    }
    else {
        return true;
    }
}

function checkPasswordMatch() {
    var newPassword = document.getElementById("confirmpass").value;
    var confirmNewPassword = document.getElementById("pass").value;
    var errorElement = document.getElementById("error_message1");

    if (newPassword != "" && confirmNewPassword != "") {
        // Check if passwords match
        if (newPassword !== confirmNewPassword) {
            errorElement.innerHTML = "Passwords does not match!";
            return false;
        } else {
            errorElement.innerHTML = "";
            return true;
        }

    } else {
        return true;
    }

}

function isEmail(email) {

    var emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

    if (!emailRegex.test(email)) {
        return false;
    }
    // If all conditions are met, the password is strong
    return true;
}


function checkStrong() {
    var newPassword = document.getElementById("pass").value;
    var errorElement = document.getElementById("error_message2");
    if (newPassword != "") {
        if (isStrongPassword(newPassword)) {
            errorElement.innerHTML = ""; // Clear the error message if passwords match and password is strong
            return true;
        } else {
            errorElement.innerHTML = "Password is not strong.\nAt least 6 characters(1number, 1UpperCase, 1LowCase, 1SpecialChars)";
            return false;
        }
    }
    else {
        return true;
    }



}


// Function to check if a password is strong
function isStrongPassword(password) {
    // Check if the password has at least 6 characters
    if (password.length < 6) {
        return false;
    }

    // Check for strong password criteria
    // Criteria: At least one uppercase letter, one lowercase letter, one digit, and one special character
    var uppercaseRegex = /[A-Z]/;
    var lowercaseRegex = /[a-z]/;
    var digitRegex = /\d/;
    var specialCharRegex = /[!@#$%^&*()_+{}[\]:;<>,.?~\\-]/;

    if (!uppercaseRegex.test(password) || !lowercaseRegex.test(password) || !digitRegex.test(password) || !specialCharRegex.test(password)) {
        return false;
    }

    // If all conditions are met, the password is strong
    return true;
}

function checkFullNAme() {
    var username = document.getElementById("name").value;
    var errorElement = document.getElementById("error_message4");
    var specialCharRegex = /[!@#$%^&*()_+{}[\]:;<>,.?~\\-]/;
    var numberRegex = /\d/;
    username = username.trim();
    if (username != "") {
        if (username === "") {
            errorElement.innerHTML = "Full name cannot be empty.";
            return false;

        } else if (specialCharRegex.test(username)) {
            errorElement.innerHTML = "Full name cannot consist of special chars";
            return false;
        } else if (numberRegex.test(username)) {

            errorElement.innerHTML = "Full name cannot include number.";
            return false;
        } else {

            errorElement.innerHTML = "";
            return true;
        }
    }
    else {
        return true;
    }


}




function togglePasswordVisibility(inputId) {
    var x = document.getElementById(inputId);
    var icon = document.getElementById(inputId + "-toggle");
    if (x.type === "password") {
        x.type = "text";
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        x.type = "password";
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}


/*        ---------------------------------------------*/


function checkInput() {
    var errorElement = document.getElementById("error_count");
    var flag = false;
    var i = 0;
    if (checkEmail()) {
        flag = true;
    } else {
        checkEmail();
        flag = false;
        i++;

    }

    if (checkStrong()) {
        flag = true;
    } else {
        checkStrong();
        flag = false;
        i++;

    }

    if (checkFullNAme()) {
        flag = true;
    } else {
        checkFullNAme();
        flag = false;
        i++;

    }

    if (checkPasswordMatch()) {
        flag = true;
    } else {
        checkPasswordMatch();
        flag = false;
        i++;

    }


    if (flag) {
        errorElement.innerHTML = "";
    } else {
        errorElement.innerHTML = "have " + i + " error";
    }

}