document.getElementById("loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const user = {
        id: 1,
        userName: String(document.getElementById("username").value),
        password: String(document.getElementById("password").value),
        Type: "Admin"
    };

    try {
        const response = await fetch("/Admin/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(user)
        });
        console.log(`the response is: ${response}`);

        if (!response.ok) {
            const errorMessage = await response.text();
            throw new Error(errorMessage);
        }

        const token = await response.text(); // קבלת הטוקן מהשרת
        sessionStorage.setItem("token", token); // שמירת הטוקן ב-sessionStorage
        window.location.href = "html/Home.html"; // מעבר לדף התכשיטים

    } catch (error) {
        alert("שגיאה בהתחברות: " + error.message);
    }
});