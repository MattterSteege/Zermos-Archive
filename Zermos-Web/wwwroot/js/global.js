document.addEventListener("DOMContentLoaded", function () {
  const elements = document.querySelectorAll(".svgText");
  // console.log(elements);

  elements.forEach(function (element) {
    element.addEventListener("mouseover", function () {
      element.classList.add("animating");
    });

    element.addEventListener("transitionend", function () {
      element.classList.remove("animating");
    });
  });

  //remove the version after 5 seconds, fade
  setTimeout(() => {
    //fade .version out in 1s
    $(".version").fadeOut(1000, () => {
      //remove .version from the DOM
      $(".version").remove();
    });
  }, 5000);

  const delay = (ms) => new Promise((res) => setTimeout(res, ms));

  //for each notification, add the .animate class, but only if the previous notification has been animated with an delay of 500ms, the first one also gets a delay of 500ms

  async function animateNotifications() {
    const notifications = $(".notification");

    let charsCount = 0;

    notifications.removeClass("animate");

    for (let i = notifications.length - 1; i >= 0; i--) {
      const notification = notifications[i];
      charsCount += notification.getElementsByTagName("p")[0].innerText.length;
      await delay(500);
      $(notification).addClass("animate");

      //when clicked: $(notification).removeClass("animate");
      notification.addEventListener("click", function () {
        $(notification).removeClass("animate");
      });
    }

    //a person reads around 265 characters per minute, as calculated here: https://ux.stackexchange.com/questions/22520/how-long-does-it-take-to-read-x-number-of-characters
    //so we wait 2 seconds and add 60 seconds for each 265 words, with a character average if 4.7 per word, which means +/- 1245,5 characters per minute
    //so: 2 seconds + 60 seconds for each 1245,5 characters
    const delayTime = 2000 + (charsCount / 1245.5) * 60000;
    //console.log(delayTime);
    await delay(delayTime);

    for (let i = 0; i < notifications.length; i++) {
      const notification = notifications[i];
      await delay(500);
      $(notification).removeClass("animate");
    }
  }

  animateNotifications();
});

function addNotification(title, body, type) {
  const notification = document.createElement("div");
  notification.classList.add("notification");
  notification.classList.add(type);

  const titleElement = document.createElement("h1");
  titleElement.innerText = title;
  notification.appendChild(titleElement);

  const bodyElement = document.createElement("p");
  bodyElement.innerText = body;
  notification.appendChild(bodyElement);

  //add to .notifications
  document.querySelector(".notifications").appendChild(notification);

  new Promise((res) => setTimeout(res, 500)).then(() => {
    $(notification).addClass("animate");

    notification.addEventListener(
      "click",
      function () {
        $(notification).removeClass("animate");
      },
      false
    );

    new Promise((res) => setTimeout(res, 5000)).then(() => {
      $(notification).removeClass("animate");
    });
  });

  return notification;
}

function arraysEqual(a, b) {
  //check if arrays are equal, but they do not have to be in the same order

  if (a === b) return true;
  if (a == null || b == null) return false;
  if (a.length !== b.length) return false;

  a.sort();
  b.sort();

  for (var i = 0; i < a.length; ++i) {
    if (a[i] !== b[i]) return false;
  }
  return true;
}
