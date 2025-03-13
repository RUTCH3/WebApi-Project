const userUrl = "/User";
let users = [];

function getUsers() {
    fetch(userUrl)
        .then((response) => response.json())
        .then((data) => _displayUsers(data))
        .catch((error) => console.error("Unable to get items.", error));
}

function addUser() {
    const name = document.getElementById("add-name").value.trim();
    const price = document.getElementById("add-price").value.trim();

    const item = { name, price };

    fetch(`${userUrl}/${id}`, {
        method: "POST",
        headers: { Accept: "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(item),
    })
        .then(() => {
            getJewelryItems();
            document.getElementById("add-form").reset();
        })
        .catch((error) => console.error("Unable to add item.", error));
}


// נקרא לפונקציה אחרי שהעמוד נטען
document.addEventListener("DOMContentLoaded", async function () {
    const imgElement = document.getElementById("profile-pic");
    try {
        const response = await fetch('/Google/profile-picture');
        if (!response.ok) throw new Error("Failed to fetch profile picture");

        const imageBlob = await response.blob();
        const imageUrl = URL.createObjectURL(imageBlob);

        console.log("img before");
        imgElement.src = imageUrl; // טוען את התמונה מהשרת
        console.log(`img after ${imgElement.src}`);
        imgElement.style.display = "block";
    } catch (error) {
        imgElement.style.display = "none";
        const i = document.getElementById("i-user");
        i.style.display = "block";
        console.error("Error loading profile picture:", error);
    }
});

function _displayUsers(data) {
    const section = document.getElementById("users");
    section.innerHTML = "";

    let count = 5;
    data.forEach((item) => {
        // יצירת אלמנט div עם class="product"
        const productDiv = document.createElement("div");
        productDiv.classList.add("product");

        // הצגת מק"ט מוצר
        const id = document.createElement("p");
        id.textContent = item.Id;

        // יצירת כותרת המוצר
        const userName = document.createElement("h3");
        userName.textContent = item.UserName;

        // יצירת כותרת המוצר
        const Type = document.createElement("h3");
        Type.textContent = item.Type;

        // יצירת מחיר
        const password = document.createElement("p");
        password.textContent = `${item.Password || ""}`;

        // // יצירת כפתור הוספה לסל
        const deleteButton = document.createElement("button");
        deleteButton.textContent = "Delete";
        deleteButton.style.backgroundColor = "pink";
        deleteButton.onclick = () => deleteJewelry(item.Id);

        // יצירת כפתור עריכה לסל
        const editButton = document.createElement("button");
        editButton.textContent = "Edit";
        editButton.style.backgroundColor = "pink";
        editButton.onclick = () => updateJewelry();

        // הוספת כל האלמנטים ל-div
        productDiv.appendChild(id);
        productDiv.appendChild(userName);
        productDiv.appendChild(Type);
        productDiv.appendChild(password);
        productDiv.appendChild(deleteButton);
        productDiv.appendChild(editButton);

        // הוספת ה-div לתוך ה-section
        section.appendChild(productDiv);
    });
}

const loadPicture = async () => {
    const imgElement = document.getElementById("profile-pic");
    try {
        const response = await fetch('/Google/profile-picture');
        if (!response.ok) throw new Error("Failed to fetch profile picture");

        const imageBlob = await response.blob();
        console.log(imageBlob.bytes);
        const imageUrl = URL.createObjectURL(imageBlob);
        console.log(imageUrl);

        console.log("img before");
        imgElement.src = imageUrl; // טוען את התמונה מהשרת
        console.log(`img after ${imgElement.src}`);
        imgElement.style.display = "block";
    } catch (error) {
        imgElement.style.display = "none";
        const i = document.getElementById("i-user");
        i.style.display = "block";
        console.error("Error loading profile picture:", error);
    }
};

const init = () => {
    const token = sessionStorage.getItem("token");

    if (!token) {
        alert("אין הרשאה. נא להתחבר.");
        window.location.href = "/index.html"; // מחזיר לדף ההתחברות
    }
    writeName();
    loadPicture();
    getUsers();
}

const writeName = (claims) => {
    try {
        let userName = claims[8].value || claims[4].value || "משתמש";
        console.log(claims[8].value);
        let nameElement = document.getElementById("helloUser");
        nameElement.textContent += ` ${userName}`;
    }
    catch (err) {
        console.error("An error in writeName: ", err)
    }
};

init();