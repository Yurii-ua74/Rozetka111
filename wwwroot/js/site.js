// передача даних в верхнє випадаюче вікно
$(document).ready(function () {
    // Перевірка розміру екрану
    if ($(window).width() >= 1010) {
        //  // Код буде працювати тільки на великих екранах >= 992px

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
    }
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



function getLocationFromSession() {
    // Робимо запит на сервер для отримання локації з сесії
    $.ajax({
        type: 'GET',
        url: '/Location/GetSessionLocation', // Адреса методу контролера
        success: function (response) {
            if (response.hasLocation) {
                
                // Якщо локація є в сесії, виводимо її
                document.getElementById('leftLocationText').innerHTML = `Ваша обрана локація: <br>${response.location}`;
                //console.log(response.location);
            } else {
                // Якщо локації немає в сесії, викликаємо геолокацію через ipapi.co
                getLocationFromAPI();
            }
        },
        error: function (error) {
            console.error('Помилка при отриманні локації з сесії:', error);
            document.getElementById('leftLocationText').innerText = 'Невідомо';
        }
    });
}

function getLocationFromAPI() {
    fetch('https://ipapi.co/json/')
        .then(response => response.json())
        .then(data => {
            const region = data.region;
            const city = data.city;
            const location = `Ви знаходитеся в регіоні: <br>${region},<br> місто: ${city}`;
            document.getElementById('leftLocationText').innerHTML = location;

            // За бажанням: можна також зберегти визначену локацію в сесію
            $.ajax({
                type: 'POST',
                url: '/Location/SaveLocation',
                data: { location: city }, // Або можна передавати цілий рядок з назвою регіону і міста
                success: function (response) {
                    console.log('Локацію збережено в сесію');
                },
                error: function (error) {
                    console.error('Помилка при збереженні локації в сесію', error);
                }
            });
        })
        .catch(err => {
            console.error('Не вдалося отримати дані:', err);
            document.getElementById('leftLocationText').innerText = 'Невідомо';
        });
}

// Викликаємо функцію, коли відкривається бічне вікно
document.getElementById('offcanvasExample').addEventListener('shown.bs.offcanvas', function () {
    getLocationFromSession();
});

// Викликаємо функцію при завантаженні сторінки
//window.onload = getLocation;

// //////  випадаючий список в боковому вікні "Сервіси"  ////// //
// Елементи для стрілки і випадаючого меню
document.getElementById('dropdownIcon').addEventListener('click', function () {
    var servicesList = document.getElementById('servicesList');

    // Перемикаємо видимість списку
    if (servicesList.style.display === 'none' || servicesList.style.display === '') {
        servicesList.style.display = 'block'; // Показуємо список
        this.classList.remove('bi-chevron-down'); // Змінюємо стрілку
        this.classList.add('bi-chevron-up');
    } else {
        servicesList.style.display = 'none'; // Ховаємо список
        this.classList.remove('bi-chevron-up'); // Змінюємо стрілку назад
        this.classList.add('bi-chevron-down');
    }
});



/*   фільтр товару по ціні   */
$(document).ready(function () {
    // Функція для виконання AJAX запиту
    function filterProductsByPrice() {
        var startPrice = $('#startPrice').val();
        var endPrice = $('#endPrice').val();

        // Перевірка наявності значень цін
        if (startPrice && endPrice) {
            // Виконуємо AJAX запит
            $.ajax({
                /*url: '/SubChildCategory/FilterByPrice',*/  // Виклик в  контролер
                url: '/SubChildCategory/FilterProducts',
                type: 'GET',
                data: { startPrice: startPrice, endPrice: endPrice },
                success: function (result) {
                    // Оновлюємо блок з товарами
                    $('#products-block').html(result);
                },
                error: function (xhr, status, error) {
                    console.log('Error:', error);
                }
            });
        }
    }

    // Виклик фільтрації при зміні значення в полях
    $('#startPrice, #endPrice').on('change', function () {
        filterProductsByPrice();
    });
});


/*  фільтр товарів по субпідкатегорії  */
function handleCheckboxChange() {
    // Збираємо всі вибрані чекбокси
    var selectedSubCategories = [];
    document.querySelectorAll('.form-check-input:checked').forEach(function (checkbox) {
        selectedSubCategories.push(checkbox.value);
    });

    // Виконуємо AJAX запит на сервер, щоб отримати товари для вибраних субкатегорій
    if (selectedSubCategories.length > 0) {
        $.ajax({
            /*url: '/SubChildCategory/GetProductsBySubChildCategories',*/
            url: '/SubChildCategory/FilterProducts',
            type: 'POST',
            data: { subchildcategoryIds: selectedSubCategories },
            success: function (result) {
                // Очищаємо блок перед виведенням нових товарів
                $('#products-block').html(result);
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
            }
        });
    } else {
        // Якщо всі чекбокси зняті, очищаємо блок товарів
        $('#products-block').html('');
    }
}

function handleCheckPriceChange() {
    var selectedSubCategories = [];

    // Отримуємо вибрані субкатегорії з чекбоксів
    document.querySelectorAll('.form-control:checked').forEach(function (checkbox) {
        selectedSubCategories.push(checkbox.value);
    });

    // Отримуємо введені значення цін
    var minPrice = $('#minPrice').val();
    var maxPrice = $('#maxPrice').val();

    // Перевіряємо, чи є вибрані субкатегорії або встановлені ціни
    if (selectedSubCategories.length > 0 || minPrice || maxPrice) {
        $.ajax({
            url: '/SubChildCategory/FilterProducts',
            type: 'POST',
            data: {
                subchildcategoryIds: selectedSubCategories,
                startPrice: minPrice,
                endPrice: maxPrice
            },
            success: function (result) {
                // Очищаємо блок перед виведенням нових товарів
                $('#products-block').html(result);
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
            }
        });
    } else {
        // Якщо всі чекбокси зняті і ціни не вказані, очищаємо блок товарів
        $('#products-block').html('');
    }
}




//$(document).ready(function () {
//    // Функція для виконання AJAX запиту з обома фільтрами (ціна + субпідкатегорії)
//    function filterProducts() {
//        var startPrice = $('#startPrice').val();
//        var endPrice = $('#endPrice').val();

//        // Збираємо всі вибрані чекбокси субпідкатегорій
//        var selectedSubCategories = [];
//        document.querySelectorAll('.form-check-input:checked').forEach(function (checkbox) {
//            selectedSubCategories.push(checkbox.value);
//        });

//        // Виконуємо AJAX запит, якщо є значення для фільтрації
//        $.ajax({
//            url: '/SubChildCategory/FilterProducts',  // Виклик в контролер для обробки обох фільтрів
//            type: 'GET',
//            data: {
//                startPrice: startPrice,
//                endPrice: endPrice,
//                subchildcategoryIds: selectedSubCategories
//            },
//            success: function (result) {
//                // Оновлюємо блок з товарами
//                $('#products-block').html(result);
//            },
//            error: function (xhr, status, error) {
//                console.log('Error:', error);
//            }
//        });
//    }

//    // Виклик фільтрації при зміні значення в полях цін
//    $('#startPrice, #endPrice').on('change', function () {
//        filterProducts();
//    });

//    // Виклик фільтрації при зміні чекбоксів субпідкатегорій
//    $('.form-check-input').on('change', function () {
//        filterProducts();
//    });
//});



