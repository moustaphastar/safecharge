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

function cardSelected() {
  var selectedValue = document.getElementById("cardSelect").value;
  var cardholderName = document.getElementById("cardHolderName");

  if (selectedValue === "2221008123677736") {
    cardholderName.value = "CL-BRW1";
  } else if (selectedValue === "4000027891380961") {
    cardholderName.value = "FL-BRW1";
  } else {
    cardholderName.value = "John Sample";
  }
}

function countBack() {
  var date = new Date().getTime();

  // Update the count down every 1 second
  var x = setInterval(function () {

    // Get today's date and time
    var now = new Date().getTime();

    // Find the distance between now and the count down date
    var distance = date - now;

    // Time calculations for days, hours, minutes and seconds
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

    // Display the result in the element with id="demo"
    document.getElementById("demo").innerHTML = days + "d " + hours + "h "
      + minutes + "m " + seconds + "s ";

    // If the count down is finished, write some text
    if (distance < 0) {
      clearInterval(x);
      document.getElementById("demo").innerHTML = "EXPIRED";
    }
  }, 1000);

}

//function makeFrame(form, methodUrl) {
//  var frame = document.createElement("iframe");
//  //frame.setAttribute("name", "fingerprintiframe");
//  frame.setAttribute("src", methodUrl);
//  frame.style.width = "0px";
//  frame.style.height = "0px";
//  document.body.appendChild(frame);
//}

//function makeFrame(methodUrl) {
//  var frame = document.createElement("iframe");
//  frame.setAttribute("name", "fingerprintiframe");
//  frame.setAttribute("src", methodUrl);
//  frame.style.width = "0px";
//  frame.style.height = "0px";
//  document.body.appendChild(frame);
//}
window.top.addEventListener("message", receiveMessage, false);
function receiveMessage(event) {
  // Do we trust the sender of this message?
  //if (event.origin !== "http://example.com:8080")
  //  return;

  // event.source is window.opener
  // event.data is "hello there!"

  // Assuming you've verified the origin of the received message (which
  // you must do in any case), a convenient idiom for replying to a
  // message is to call postMessage on event.source and provide
  // event.origin as the targetOrigin.
  console.log(event);
  event.source.postMessage("hi there yourself!  the secret response " +
    "is: rheeeeet!",
    event.origin);
}


