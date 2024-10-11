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

/*  для виводу карти  */
function getLocationFromSession() {
    // запит на сервер для отримання локації з сесії
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



/*  ///////////////////////////////////////////////////////  */


// скріпт для обробки змін фільтрів субпідкатегорій та цін //
$(document).ready(function () {
    // Функція для обробки змін чекбоксів та цін
    function handleCheckboxChange() {
        var selectedSubCategories = [];
        // Отримуємо всі обрані чекбокси
        $('.subCategoryCheckbox:checked').each(function () {
            selectedSubCategories.push($(this).val());
        });

        // Отримуємо значення полів цін
        var minPrice = $('#minPrice').val();
        var maxPrice = $('#maxPrice').val();

        // AJAX-запит для фільтрації продуктів
        $.ajax({
            url: '/Childcategories/GetProductsByFilter', // Поставте правильний шлях до вашого контролера
            type: 'POST',
            data: {
                subChildCategoryIds: selectedSubCategories,
                minPrice: minPrice,
                maxPrice: maxPrice
            },
            success: function (response) {
                // Оновлення часткового представлення продуктів
                $('#productsPartial').html(response);   // Id з HTML в представленні
            },
            error: function (xhr, status, error) {
                console.log('Помилка при отриманні фільтрованих даних:', error);
            }
        });
    }

    // Виклик функції при зміні чекбоксів
    $('.subCategoryCheckbox').on('change', handleCheckboxChange);

    // Виклик функції при зміні полів ціни
    $('#minPrice, #maxPrice').on('change', handleCheckboxChange);
});



document.getElementById('becomeSellerButton').addEventListener('click', function () {
    // AJAX-запит для перевірки авторизації
    $.ajax({
        url: '/Account/IsUserLoggedIn',
        method: 'GET',
        success: function (response) {
            if (response.isAuthenticated) {
                // Якщо користувач авторизований, відкриваємо модальне вікно
                $('#sellerModal').modal('show');

                // AJAX-запит для отримання даних користувача
                $.ajax({
                    url: '/Account/GetUserData',
                    method: 'GET',
                    success: function (userData) {
                        console.log(userData);  // Додаємо це, щоб перевірити, що повертається з сервера

                        // Переконайтеся, що JSON-об'єкт має правильні назви полів
                        if (userData && userData.email && userData.firstName && userData.lastName) {
                            document.getElementById('seller-email').value = userData.email;
                            document.getElementById('seller_first_name').value = userData.firstName;
                            document.getElementById('seller_second_name').value = userData.lastName;
                        } else {
                            console.error('Неправильні або відсутні дані користувача');
                        }
                    },
                    error: function (err) {
                        console.error('Помилка під час отримання даних користувача', err);
                    }
                });
            } else {
                // Якщо не авторизований, відкриваємо модальне вікно реєстрації
                $('#registerModal').modal('show');
            }
        },
        error: function (err) {
            console.error('Помилка під час перевірки авторизації', err);
        }
    });
});

