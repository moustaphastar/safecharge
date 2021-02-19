window.addEventListener('message', receive, false);

function receive(evt) {
  var response = JSON.parse(evt.data).response;
  var encoded = response.split("=");
  console.log(encoded[1]);
  console.log(response);
  $.ajax({
    method: "POST",
    url: "https://localhost:44356/api/threeD/notification",
    data: JSON.stringify(encoded[1]),
    contentType: "application/json",
    success: function (response) {
      console.log(response);
      document.getElementById("methodCompletionInd").value = "Y";
      alert("threeDServerTransID Decoded!");
    }
  });
}

// Wait for ten seconds
function wait(timeout) {
  return new Promise(resolve => {
    setTimeout(resolve, timeout);
  });
}

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

function setCurrencyDefault(value) {
  if (value === null || value === undefined || value === "undefined") {
    writeObjectToLocalStorage("currency", "USD");
  } else {
    writeObjectToLocalStorage("currency", value);
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

function navigateToCheckoutMpiOnly() {
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

  window.location.href = "/Home/CheckoutMpiOnly?amount=" + total.toFixed(2).toString() + "&currency=" + currency;
};

function navigateToCheckoutNon3D() {
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

  window.location.href = "/Home/CheckoutNon3D?amount=" + total.toFixed(2).toString() + "&currency=" + currency;
};

function navigateToCheckoutS2S() {
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

  window.location.href = "/Home/CheckoutS2S?amount=" + total.toFixed(2).toString() + "&currency=" + currency;
};

function navigateToCheckoutZAuth() {
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

  window.location.href = "/Home/CheckoutZeroAuth?amount=" + total.toFixed(2).toString() + "&currency=" + currency;
};

function navigateToCheckoutAuthSettle() {
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

  window.location.href = "/Home/CheckoutAuthSettle?amount=" + total.toFixed(2).toString() + "&currency=" + currency;
};


function cardSelected() {
  var selectedValue = document.getElementById("cardSelect").value;
  var cardholderName = document.getElementById("cardHolderName");
  var cardNumber = document.getElementById("cardNumber");
  var methodUrlIndicator = document.getElementById("methodUrl");

  if (selectedValue === "4000023104662535") {
    cardholderName.value = "John Sample";
    cardNumber.value = "4000023104662535";
    console.log("cardHolder: " + cardholderName.value + "; cardNumber: " + cardNumber.value + "; Flow: Non-3DS Approved Card");
  }
  else if (selectedValue === "4008370896662369") {
    cardholderName.value = "John Sample";
    cardNumber.value = "4008370896662369";
    console.log("cardHolder: " + cardholderName.value + "; cardNumber: " + cardNumber.value + "; Flow: Non-3DS Declined Card");
  }
  else if (selectedValue === "4000020951595032") {
    cardholderName.value = "FL-BRW1";
    cardNumber.value = "4000020951595032";
    console.log("cardHolder: " + cardholderName.value + "; cardNumber: " + cardNumber.value + "; Flow: 3DS v2 Frictionless Flow Card");
  }
  else if (selectedValue === "2221008123677736") {
    cardholderName.value = "CL-BRW1";
    cardNumber.value = "2221008123677736"; 
    methodUrlIndicator.value = "";//"https://localhost:44356/api/ThreeD/Notification";
    console.log("cardHolder: " + cardholderName.value + "; cardNumber: " + cardNumber.value + "; Flow: 3DS v2 Challenge Flow Card");
  }
  else if (selectedValue === "4407106439671112") {
    cardholderName.value = "CL-BRW1";
    cardNumber.value = "4407106439671112";
    methodUrlIndicator.value = "";
    console.log("cardHolder: " + cardholderName.value + "; cardNumber: " + cardNumber.value + "; Flow: 3DS v1 Flow Card");
  }
  else {
    cardholderName.value = "John Sample";
    cardNumber.value = "2221008123677736";
    console.log("cardHolder: " + cardholderName.value + "; cardNumber: " + cardNumber.value + "; Flow: Other");
  }
}