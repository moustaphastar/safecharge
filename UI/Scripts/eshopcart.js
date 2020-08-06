function getObjectFromLocalStorage(key) {
  try {
    var jsonObject = window.localStorage.getItem(key);
    var object = JSON.parse(jsonObject);
    return object;
  } catch (e) {
    console.log(e);
    return null;
  }
}

function writeObjectToLocalStorage(key, object) {
  try {
    var jsonObject = JSON.stringify(object);
    window.localStorage.setItem(key, jsonObject);
    return true;
  } catch (e) {
    console.log(e);
    return false;
  }
}

function initializeCartTotal() {
  var cart = getObjectFromLocalStorage("cart");
  document.getElementById('cartCount').innerHTML = cart === null ? 0 : cart.items.length;
}

function UpdateCartTotals(itemsCount) {
  console.log(itemsCount);
  var cartCountElem = document.getElementById('cartCount').innerHTML = itemsCount;
  console.log("html element updated");
}

function addToCart(id, name, price) {
  var cart = getObjectFromLocalStorage("cart") || [];
  var newItem = { id: id, name: name, price: price.toFixed(2), quantity: 1, total: price.toFixed(2) };

  var _itemInCart = cart.find(item => item.id === id);
  if (cart.length === 0 || _itemInCart === undefined) {
    cart.push(newItem);
  } else {
    _itemInCart.quantity = parseInt(_itemInCart.quantity) + 1;
    _itemInCart.total = (_itemInCart.quantity * parseFloat(_itemInCart.price)).toFixed(2);
  }

    let isWritten = writeObjectToLocalStorage("cart", cart);
    console.log("item write status is " + isWritten);
    UpdateCartTotals(cart.length);
  }

function updateCart(id, name, price, quantity) {
  var cart = getObjectFromLocalStorage("cart") || [];
  var _itemInCart = cart.find(item => item.id === id);

  if (cart.length === 0 || _itemInCart === undefined) {
    addToCart(id, name, price);
    console.log("Item was not in cart! Added.");
    return;
  } else {
    _itemInCart.quantity = parseInt(quantity);
    _itemInCart.total = (parseFloat(_itemInCart.price) * parseFloat(quantity) * 1).toFixed(2);
  }

  let isUpdated = UpdateCartTotals(cart.length);
  console.log("item write status is " + isUpdated);
  UpdateCartTotals(cart.length);

  return;
};

function setCurrency(select) {
  if (select === null || select === undefined || select === "undefined") {
    writeObjectToLocalStorage("currency", "USD");
  } else {
    writeObjectToLocalStorage("currency", select.value);
  }
}

function populateCartTable() {
  var cart = JSON.parse(localStorage.getItem("cart"));
  var tbody = document.getElementById("tbody");
  var tfoot = document.getElementById("tfoot");
  var cartTotal = 0;

  for (var i = 0; i < cart.length; i++) {
    var item = cart[i];
    var tr = `<tr><td>${(i + 1)}</td><td>${item.name}</td>
              <td>${item.quantity}</td>
              <td>$${item.price}</td>
              <td class="text-right">$${item.total}</td></tr>`;
    tbody.innerHTML += tr;
    cartTotal += parseFloat(item.total);
  }

  tfoot.innerHTML += '<tr class="cart-table-footer"><td colspan="4">Checkout Total</td><td class="text-right font-weight-bold">$' + cartTotal.toFixed(2) + '</td></tr>';
};

function navigateToCheckout() {
  var cart = getObjectFromLocalStorage("cart");
  var currency = getObjectFromLocalStorage("currency");
  var total = 0;

  if (cart === null) {
    return;
  } else {
    cart.forEach(item => {
      total += parseFloat(item.total);
    });
  }

  window.location.href = "/Home/Checkout?amount=" + total.toFixed(2).toString() + "&currency=" + currency + "&cartItems=" + JSON.stringify(cart);
};

function navigateToCheckoutWithSDK() {
  var cart = getObjectFromLocalStorage("cart");
  var currency = getObjectFromLocalStorage("currency");
  var total = 0;

  if (cart === null) {
    return;
  } else {
    cart.forEach(item => {
      total += parseFloat(item.total);
    });
  }

  window.location.href = "/Home/Checkout?amount=" + total.toFixed(2).toString() + "&currency=" + currency;
};
