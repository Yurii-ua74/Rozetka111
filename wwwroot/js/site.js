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


// /// скріпт для обробки змін фільтрів субпідкатегорій та цін /// //
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


// ///// скріпт для теми (кнопки) Стати продавцем ///// //
document.addEventListener('DOMContentLoaded', function () {
    const becomeSellerButton = document.getElementById('becomeSellerButton');

    if (becomeSellerButton) {
        becomeSellerButton.addEventListener('click', function () {
            // AJAX-запит для перевірки авторизації
            $.ajax({
                url: '/Account/IsUserLoggedIn',
                method: 'GET',
                success: function (response) {
                    if (response.isAuthenticated) {
                        // Якщо користувач авторизований, відкриваємо модальне вікно Стати продавцем
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
                        $('#authModal').modal('show');
                    }
                },
                error: function (err) {
                    console.error('Помилка під час перевірки авторизації', err);
                }
            });
        });
    }
});




/* ///// для заповнення списків на сторінці Подати оголошення - метод GetAdvertisement ///// */
$(document).ready(function () {
    $('#category-select').change(function () {
        var categoryId = $(this).val();

        // Очищуємо і відключаємо поля Childcategory та SubChildcategory
        $('#childcategory-select').empty().prop('disabled', true);
        $('#subchildcategory-select').empty().prop('disabled', true);

        // Очищуємо і відключаємо поля Childcategory та SubChildcategory
        //$('#childcategory-select').empty().append('<option value="">Оберіть категорію спочатку</option>').prop('disabled', true);
        //$('#subchildcategory-select').empty().append('<option value="">Оберіть розділ категорії спочатку</option>').prop('disabled', true);

        if (categoryId) {
            $.ajax({
                url: '/Advertisement/GetChildCategories', // Вкажіть правильний маршрут контролера
                type: 'GET',
                data: { categoryId: categoryId },
                success: function (data) {
                    if (data.length > 0) {
                        $('#childcategory-select').prop('disabled', false).append('<option value="">Оберіть розділ категорії</option>');
                        $.each(data, function (i, item) {
                            $('#childcategory-select').append($('<option>', {
                                value: item.id,
                                text: item.name
                            }));
                        });
                    } else {
                        /*$('#childcategory-select').append('<option value="">Немає розділів категорії</option>');*/
                        $('#childcategory-select').append('<option value="">Оберіть категорію спочатку</option>');
                        $('#subchildcategory-select').append('<option value="">Оберіть розділ категорії спочатку</option>');
                    }
                }
            });
        }
    });

    $('#childcategory-select').change(function () {
        var childCategoryId = $(this).val();

        // Очищуємо і відключаємо поле SubChildcategory
        //$('#subchildcategory-select').empty().append('<option value="">Оберіть розділ категорії спочатку</option>').prop('disabled', true);
        $('#subchildcategory-select').empty().prop('disabled', true);

        if (childCategoryId) {
            $.ajax({
                url: '/Advertisement/GetSubChildCategories', // Вкажіть правильний маршрут контролера
                type: 'GET',
                data: { childCategoryId: childCategoryId },
                success: function (data) {
                    if (data.length > 0) {
                        $('#subchildcategory-select').prop('disabled', false).append('<option value="">Оберіть розділ підкатегорії</option>');
                        $.each(data, function (i, item) {
                            $('#subchildcategory-select').append($('<option>', {
                                value: item.id,
                                text: item.name
                            }));
                        });
                    } else {
                        $('#subchildcategory-select').append('<option value="">Немає розділів підкатегорії</option>');
                    }
                }
            });
        } else {
            $('#subchildcategory-select').append('<option value="">Оберіть розділ категорії спочатку</option>');
        }
    });
});

//////////////// Сектор изменения и обновления избранных товаров ///////////////////
$(document).ready(function () {
    // Обработчик для добавления в избранное
    $('.add-to-favorites').on('click', function () {
        var productId = $(this).data('product-id');

        $.post('/Favorites/AddToFavorites', { productId: productId }, function (data) {
            if (data.success) {
                // Показать сообщение об успехе
                window.location.reload();
                //alert(data.message);
            } else {
                // Показать сообщение об ошибке
                alert(data.message);
            }
        }).fail(function () {
            // Обработка ошибок при AJAX запросе
            alert('Сталася помилка при додаванні товару в обране.');
        });
    });
});

//////////////// Сектор изменения и обновления корзины ///////////////////
$(document).ready(function () {
    $('#cartModal').on('show.bs.modal', function () {
        $('.custom-modal-body').html('<span>Loading...</span>');  // Показать индикатор загрузки
        $.get('/Cart/LoadCartModal', function (data) {
            $('.custom-modal-body').html(data);  // Вставить данные в модальное окно
        });
    });
});
//$(document).ready(function () {
//    $('#cartModal').on('show.bs.modal', function () {
//        $.ajax({
//            url: '/Cart/LoadCartModal', // Обращение к методу LoadCartModal
//            type: 'GET',
//            success: function (data) {
//                $('#cartModal .modal-content').html(data); // Заполняем модальное окно содержимым
//            },
//            error: function (xhr, status, error) {
//                console.error("Ошибка при загрузке корзины: ", status, error);
//            }
//        });
//    });
//});

$(document).ready(function () {
    // Получение количества товаров в корзине при загрузке страницы
    $.get('/Cart/GetCartCount', function (data) {
        if (data.count > 0) {
            $('#cart-count').text(data.count).show();
        } else {
            $('#cart-count').hide();
        }
    }).fail(function (xhr, status, error) {
        console.error("Ошибка при получении количества товаров в корзине: ", status, error);
    });
});

function updateCart(productId, action) {
    let url = '/Cart/' + action; // Либо AddToCart, либо RemoveFromCart
    $.post(url, { productId: productId, count: 1 }, function (data) {
        $('#cart-count').text(data.count);
        if (data.count > 0) {
            $('#cart-count').show().text(data.count);
        } else {
            $('#cart-count').hide();
        }
    }).fail(function (xhr, status, error) {
        alert("Ошибка: " + xhr.responseText); // Выводим ошибку на экран
        console.error("Ошибка при добавлении товара в корзину: ", status, error);
    });
}

$(document).on('change', '.quantity-input', function () {
    var productId = $(this).data('product-id');
    var newCount = $(this).val();

    $.post('/Cart/UpdateCartItem', { productId: productId, newCount: newCount }, function (data) {
        $('tr[data-product-id="' + productId + '"] td:nth-child(5)').text(data.itemTotalPrice.toFixed(0));
        $('#total-price').text(data.totalPrice.toFixed(0));
        if (data.count > 0) {
            $('#cart-count').show().text(data.count);
        } else {
            $('#cart-count').hide();
        }
    }).fail(function (xhr, status, error) {
        alert("Ошибка: " + xhr.responseText);
    });
});

$(document).on('click', '.remove-item', function () {
    var productId = $(this).data('product-id');

    $.post('/Cart/RemoveFromCart', { productId: productId }, function (data) {
        $('tr[data-product-id="' + productId + '"]').remove();
        $('#total-price').text(data.totalPrice.toFixed(0));
        if (data.count > 0) {
            $('#cart-count').show().text(data.count);
        } else {
            $('#cart-count').hide();
        }
    });
});

// //////////   для відкриття тост вікон   ///////////// //
document.addEventListener("DOMContentLoaded", function () {
    var successToastElement = document.getElementById('successToast');
    var errorToastElement = document.getElementById('errorToast');

    // Ініціалізація тостів за допомогою Bootstrap Toast
    if (successToastElement) {
        var successToast = new bootstrap.Toast(successToastElement);
        successToast.show();  // Показати тост успіху
    }

    if (errorToastElement) {
        var errorToast = new bootstrap.Toast(errorToastElement);
        errorToast.show();  // Показати тост помилки
    }
});


// ///// Функція для показу прогрес-бару ///// //
// Показати прогрес-бар
function showProgressBar() {
    document.getElementById('progressBar').style.display = 'block';
}
// Сховати прогрес-бар
function hideProgressBar() {
    document.getElementById('progressBar').style.display = 'none';
}
// Показати прогрес-бар при переході на нову сторінку
window.addEventListener('beforeunload', function () {
    showProgressBar();
});
// Сховати прогрес-бар після завантаження сторінки
window.addEventListener('load', function () {
    hideProgressBar();
});

