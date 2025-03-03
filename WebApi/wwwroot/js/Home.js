const crown = "../data/jewelrys.json";
let jewelryItems = [];

function getJewelryItems() {
  fetch(crown)
    .then((response) => response.json())
    .then((data) => _displayJewelryItems(data))
    .catch((error) => console.error("Unable to get items.", error));
}

function addJewelry() {
  const name = document.getElementById("add-name").value.trim();
  const price = document.getElementById("add-price").value.trim();

  const item = { name, price };

  fetch(crown, {
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

    // יצירת כותרת המוצר
    const title = document.createElement("h3");
    title.textContent = item.Name;

    // יצירת מחיר
    const price = document.createElement("p");
    price.textContent = `₪${item.Price || "N/A"}`;

    // יצירת כפתור הוספה לסל
    const addToCartButton = document.createElement("button");
    addToCartButton.textContent = "Add to Cart";
    addToCartButton.style.border = "black solid 2px";
    addToCartButton.onclick = () => addToCart(item.name, item.price);

    // יצירת כפתור עריכה לסל
    const editToCartButton = document.createElement("button");
    editToCartButton.textContent = "edit Cart";
    editToCartButton.style.border = "black solid 2px";
    editToCartButton.onclick = () => addToCart(item.name, item.price);

    // הוספת כל האלמנטים ל-div
    productDiv.appendChild(img);
    productDiv.appendChild(title);
    productDiv.appendChild(price);
    productDiv.appendChild(addToCartButton);
    productDiv.appendChild(editToCartButton);

    // הוספת ה-div לתוך ה-section
    section.appendChild(productDiv);
  });
}

function _displayCount(count) {
  const counter = document.getElementById("counter");
  counter.textContent = `${count} ${count === 1 ? "item" : "items"}`;
}

function addToCart(product, price) {
  alert(`${product} has been added to your cart for $${price}!`);
}
const saveToken = (token) => {
  sessionStorage.setItem("token", token);
  console.log("Token saved:", token);
  window.location.href = "/dashboard.html"; // ניתוב לדף הרלוונטי
};

// קבלי את הטוקן מהשרת אחרי התחברות
fetch("/auth/google-token") // כתובת ה-API שמחזירה את הטוקן
  .then((res) => res.json())
  .then((data) => {
    if (data.token) {
      saveToken(data.token);
    } else {
      console.error("No token received", data);
    }
  })
  .catch((err) => console.error("Error fetching token:", err));

const init = () => {
  const token = sessionStorage.getItem("token");

  if (!token) {
    alert("אין הרשאה. נא להתחבר.");
    window.location.href = "/index.html"; // מחזיר לדף ההתחברות
  } else
    getJewelryItems();
}

init();