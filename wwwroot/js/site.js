function addToCartAndOpenSidebar(productId, brandTitle, productName, productPhoto, productPrice) {
    // Виконати AJAX-запит для додавання товару до кошика 
    fetch(`/Cart/AddToCart/${productId}`, {
        method: "post",
        headers: {
            "accept": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                return response.json(); 
            } else {
                throw new Error('Щось пішло не так при додаванні товару в кошик');
            }
        })
        .then(cartData => {
            // Оновлюємо дані у бічному вікні
            const sidebarCart = document.getElementById('sidebarCart');
            // Приклад оновлення бічного вікна:
        //    sidebarCart.innerHTML = `
        //    <div class="cart-item">
        //        <img src="${productPhoto}" alt="${productName}" />
        //        <div>${productName}</div>
        //        <div>${productPrice} UAH</div>
        //    </div>
        //`;
            // Відкриваємо бічне вікно
            document.getElementById('cartSidebar').classList.add('open');
        })
        .catch(error => console.error('Помилка:', error));

    // Оновлюємо дані у бічному вікні
    const cartData = {
        items: [{
            id: productId,
            brand: brandTitle,
            name: productName,
            photoUrl: productPhoto,
            price: productPrice,
            quantity: 1,
            totalPrice: productPrice // Оскільки кількість 1, загальна ціна дорівнює ціні
        }]
    };

    updateCartSidebar(cartData);

    // Відкриваємо бічне вікно
    openCartSidebar();
}

// Відкриття бічного вікна
function openCartSidebar() {
    document.getElementById("cartSidebar").style.width = "400px";
}

// Відкриття бічного вікна
function closeCartSidebar() {
    document.getElementById("cartSidebar").style.width = "0";
}

function updateCartSidebar(cartData) {
    const sidebarContent = document.querySelector('.cart-items');
    let html = '';

    cartData.items.forEach((item, index) => {
        html += `<div class="cart-item">
                    <div class="item-header">
                        <h4>${item.name}</h4>
                    </div>
                    <div class="item-details">
                        <p><strong>#:</strong> ${index + 1}</p>
                        <p><strong>Photo:</strong> <img src="${item.photoUrl}" alt="Product Photo" width="50"></p>
                        <p><strong>Product Name:</strong> ${item.name}</p>
                        <p><strong>Product Price, UAH:</strong> ${item.price} UAH</p>
                        <p><strong>Count:</strong> ${item.quantity}</p>
                        <p><strong>Total Price, UAH:</strong> ${item.totalPrice} UAH</p>
                        <button class="btn btn-danger btn-sm" onclick="removeFromCart(${item.id})">Видалити</button>
                    </div>
                </div>`;
    });

    sidebarContent.innerHTML = html;
}