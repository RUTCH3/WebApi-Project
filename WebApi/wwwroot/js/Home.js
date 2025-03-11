const crown = "/Jewelry";
let jewelryItems = [];


// קבלי את הטוקן מהשרת אחרי התחברות
fetch("/Google/GoogleResponse") // כתובת ה-API שמחזירה את הטוקן
  .then(response => {
    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return response.text(); // קרא כטקסט תחילה
  })
  .then(text => {
    if (!text) {
      throw new Error("Empty response received");
    }
    return JSON.parse(text); // הפוך ל-JSON ידנית
  })
  .then((data) => {
    if (data.token) {
      console.log("reached to token in google....");
      saveToken(data.token);
      loadPicture();
      writeName(data.claims);
      init();
    } else {
      console.error("No token received", data);
    }
  })
  .catch((err) => {
    console.log("An error: ", err);
    loadPicture();
    writeName();
    init();
  });


function getJewelryItems() {
  fetch(`${crown}/Get`, {
    method: "GET",
    headers: {
      "Accept": "application/json",
      "Authorization": `Bearer ${sessionStorage.getItem("token")}`
    }
  }).then(response => {
    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return response.headers.get("content-length") === "0" ? [] : response.json();
  })
    .then((data) => _displayJewelryItems(data))
    .catch((error) => console.error("Unable to get items.", error));
}

function addJewelry() {
  const name = document.getElementById("add-name").value.trim();
  const price = document.getElementById("add-price").value.trim();

  const item = { name, price };

  fetch(`${crown}`, {
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

function deleteJewelry(id) {
  fetch(`${crown}/${id}`, { method: "DELETE" })
    .then(() => getJewelryItems())
    .catch((error) => console.error("Unable to delete item.", error));
}

function displayEditForm(id) {
  const item = jewelryItems.find((item) => item.id === id);
  document.getElementById("edit-id").value = item.id;
  document.getElementById("edit-name").value = item.name;
  document.getElementById("edit-price").value = item.price;
  document.getElementById("editForm").style.display = "block";
}

function updateJewelry() {
  const id = parseInt(document.getElementById("edit-id").value, 10);
  const name = document.getElementById("edit-name").value.trim();
  const price = document.getElementById("edit-price").value.trim();

  const item = { id, name, price };

  fetch(`${crown}/${id}`, {
    method: "PUT",
    headers: { Accept: "application/json", "Content-Type": "application/json" },
    body: JSON.stringify(item),
  })
    .then(() => getJewelryItems())
    .catch((error) => console.error("Unable to update item.", error));

  closeInput();
}

function closeInput() {
  document.getElementById("editForm").style.display = "none";
}

function _displayJewelryItems(data) {
  const section = document.getElementById("products2");
  section.innerHTML = "";

  let count = 5;
  data.forEach((item) => {
    // יצירת אלמנט div עם class="product"
    const productDiv = document.createElement("div");
    productDiv.classList.add("product");

    // יצירת אלמנט תמונה
    const img = document.createElement("img");
    img.src = item.image || `../pictures/${count++}.jpg`; // ברירת מחדל אם  תמונה
    img.alt = item.name;

    // הצגת מק"ט מוצר
    const id = document.createElement("p");
    id.textContent = item.Id;

    // יצירת כותרת המוצר
    const title = document.createElement("h3");
    title.textContent = item.Name;

    // יצירת מחיר
    const price = document.createElement("p");
    price.textContent = `₪${item.Price || "N/A"}`;

    // // יצירת כפתור הוספה לסל
    const deleteButton = document.createElement("button");
    deleteButton.textContent = "Delete";
    // deleteButton.style.border = "black solid 2px";
    deleteButton.style.backgroundColor = "pink";
    deleteButton.onclick = () => addToCart(item.Name, item.Price);

    // יצירת כפתור עריכה לסל
    const editButton = document.createElement("button");
    editButton.textContent = "Edit";
    // editButton.style.border = "black solid 2px";
    editButton.style.backgroundColor = "pink";
    editButton.onclick = () => addToCart(item.Name, item.Price);
    editButton.onsubmit = () => addJewelry();

    // הוספת כל האלמנטים ל-div
    productDiv.appendChild(img);
    productDiv.appendChild(id);
    productDiv.appendChild(title);
    productDiv.appendChild(price);
    productDiv.appendChild(deleteButton);
    productDiv.appendChild(editButton);

    // הוספת ה-div לתוך ה-section
    section.appendChild(productDiv);
  });
}

function _displayCount(count) {
  const counter = document.getElementById("counter");
  counter.textContent = `${count} ${count === 1 ? "item" : "items"}`;
}

const saveToken = (token) => {
  sessionStorage.setItem("token", token);
  console.log("Token saved");
};

const writeName = (claims) => {
  let nameElement = document.getElementById("helloUser");
  let userName;
  try {
    userName = claims[8].value || claims[4].value || "משתמש";
    console.log(claims[8].value);
  }
  catch (err) {
    userName = "משתמש";
    console.error("An error in writeName: ", err)
  }
  nameElement.textContent += ` ${userName}`;
};

// נקרא לפונקציה אחרי שהעמוד נטען
//document.addEventListener("DOMContentLoaded", async () => {
const loadPicture = async () => {
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
};

const init = () => {
  const token = sessionStorage.getItem("token");

  if (!token) {
    alert("אין הרשאה. נא להתחבר.");
    window.location.href = "/index.html"; // מחזיר לדף ההתחברות
  }
  getJewelryItems();
}