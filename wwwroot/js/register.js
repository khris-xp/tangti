document.getElementById("registerForm").addEventListener("submit", function(event) {
    event.preventDefault();
    var username = document.getElementById("username").value;
    var email = document.getElementById("email").value;
    var password = document.getElementById("password").value;

    // Simple validation
    if (username.trim() === "" || email.trim() === "" || password.trim() === "") {
        document.getElementById("message").innerText = "Please fill in all fields.";
        return;
    }

    // You can perform further validation here if needed

    // If validation passes, you can send the data to the server using AJAX or any other method
    // For now, we'll just submit the form
    document.getElementById("registerForm").submit();
});
