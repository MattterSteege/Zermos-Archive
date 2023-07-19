console.log('global.js loaded');
document.addEventListener('DOMContentLoaded', function () {
    const elements = document.querySelectorAll('.svgText');
    // console.log(elements);

    elements.forEach(function (element) {
        element.addEventListener('mouseover', function () {
            element.classList.add('animating');
        });

        element.addEventListener('transitionend', function () {
            element.classList.remove('animating');
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

    const delay = ms => new Promise(res => setTimeout(res, ms));

//for each notification, add the .animate class, but only if the previous notification has been animated with an delay of 500ms, the first one also gets a delay of 500ms

    async function animateNotifications() {
        const notifications = $(".notification");

        notifications.removeClass("animate");

        for (let i = notifications.length - 1; i >= 0; i--) {
            const notification = notifications[i];
            await delay(500);
            $(notification).addClass("animate");

            //when clicked: $(notification).removeClass("animate");
            notification.addEventListener('click', function () {
                $(notification).removeClass("animate");
            });
        }

        await delay(5000);

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

    new Promise(res => setTimeout(res, 500)).then(() => {
        $(notification).addClass("animate");

        notification.addEventListener('click', function () {
                $(notification).removeClass("animate");
        }, false);

        new Promise(res => setTimeout(res, 5000)).then(() => {
            $(notification).removeClass("animate");
        });
    });

    return notification;
}