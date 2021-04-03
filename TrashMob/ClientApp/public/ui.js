
// Select DOM elements to work with
const signInButton = document.getElementById('signIn');
const signOutButton = document.getElementById('signOut')
const welcomeDiv = document.getElementById('welcome-div');
const editProfileButton = document.getElementById('editProfileButton');
const response = document.getElementById("response");
const label = document.getElementById('label');

function welcomeUser(username) {
    signInButton.classList.add('d-none');
    signOutButton.classList.remove('d-none');
    editProfileButton.classList.remove('d-none');
    welcomeDiv.classList.remove('d-none');
    welcomeDiv.innerHTML = `Welcome ${username}!`
}

function logMessage(s) {
    response.appendChild(document.createTextNode('\n' + s + '\n'));
}