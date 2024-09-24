// передача даних в верхнє випадаюче вікно
$(document).ready(function () {
    // Обробка наведення на посилання категорії
    $('.category-link').on('mouseenter', function () {
        var category = $(this).data('category'); // Отримуємо назву категорії
        console.log("Запит категорії: " + category); // Виводимо в консоль

        $.ajax({
            url: '/Categories/GetChildAndSubChildCategories', // маршрут
            method: 'GET',
            data: { category: category },
            success: function (data) {
                $('.subcategory-container').html(data); // Вставляємо отримані дані
                console.log("Отримані дані: ", data); // Виводимо отримані дані в консоль
            },
            error: function (xhr, status, error) {
                console.error("Помилка: ", error); // Виводимо помилку в консоль
            }
        });
    });
});



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


// додавання кількості товару над кошиком
$(document).ready(function() {
    // Handle 'Add to Cart' button click
    $('.btn-add-to-cart').on('click', function (event) {
        event.preventDefault();
        var $button = $(this);
        var url = $button.attr('href');
        $.ajax({
            url: url,
            type: 'POST',
            success: function (response) {
                if (response.success) {
                    // Update cart count
                    $('#cart-count').text(response.cartCount).show();
                }
            },
            error: function () {
                alert('Error adding item to cart.');
            }
        });
    });
});


// для оновлення лічильника
function updateCartCount() {
    fetch('/Cart/GetCartCount')
        .then(response => response.json())
        .then(data => {
            document.getElementById('cart-count').innerText = data.cartCount;
            document.getElementById('cart-count').style.display = data.cartCount > 0 ? 'block' : 'none';
        });
}
document.addEventListener('DOMContentLoaded', updateCartCount);


// ////// отримання місцезнаходження ////// //
// Додаємо функцію для отримання даних
//function getLocation() {
//    fetch('https://ipinfo.io/json?token=675fcbb92ab7c6') // Мій токен
//        .then(response => {
//            if (!response.ok) {
//                throw new Error('Network response was not ok');
//            }
//            return response.json();
//        })
//        .then(data => {
//            const location = data.city; // Отримуємо назву міста
//            document.getElementById('location').innerHTML = `Ваше місце знаходження: <br>${location} ?`;
//        })
//        .catch(err => {
//            console.error('Не вдалося отримати дані:', err);
//            document.getElementById('location').innerText = 'Не вдалося отримати ваше місцезнаходження';
//        });
//}

/*document.getElementById('getLocationBtn').addEventListener('click', function () {*/
function getLocation() {
    fetch('https://ipapi.co/json/')
        .then(response => response.json())
        .then(data => {
            const location = data.city;
            document.getElementById('location').innerText = `Ви знаходитеся в: ${location}?`;
        })
        .catch(err => {
            console.error('Не вдалося отримати дані:', err);
        });
}/*);*/

// Викликаємо функцію при завантаженні сторінки
window.onload = getLocation;



