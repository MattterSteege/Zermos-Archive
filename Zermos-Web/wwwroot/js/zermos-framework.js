//on popstate, replace the page with what's after the hash
window.addEventListener('popstate', function(e) {
    //if the hash is empty, don't replace the page
    if (window.location.hash === "") {
        return;
    }
    ReplacePage(window.location.hash.replace("#", ""), true);
});

document.addEventListener("DOMContentLoaded", async function () {
    await initializeNotifications();
    console.log("[ GLOBAL.JS ] Notificaties geïnitialiseerd");
});

//==============================NOTIFICATION SYSTEM==============================

async function initializeNotifications() {
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

    const notifications = $(".notification");
    let charsCount = 0;
    //for each notification, add the .animate class, but only if the previous notification has been animated with an delay of 500ms, the first one also gets a delay of 500ms
    for (let i = notifications.length - 1; i >= 0; i--) {
        const notification = notifications[i];
        charsCount += notification.getElementsByTagName("p")[0].innerText.length;
        await delay(500);
        notification.classList.add("animate");

        //when clicked: $(notification).removeClass("animate");
        notification.addEventListener("click", async function () {
            notification.classList.remove("animate");
            await delay(500);
            notification.remove();
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
        notification.classList.remove("animate");
        notification.remove();
    }
}

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

//==============================LOADING SYSTEM==============================

var loadingTexts = [
    "Tijd en Ruimte omwisselen",
    "heftig rond draaien",
    "Een lepel buigen",
    "Denk niet aan paarse nijlpaarden",
    "Leuke dag?",
    "Lachen laadt Zermos sneller!",
    "En geniet van de liftmuziek",
    "Even geduld a.u.b. terwijl de kleine elven je pagina tekenen",
    "Wil je daar frietjes bij?",
    "De zwaartekrachtconstante in uw omgeving controleren",
    "Ga je gang - houd je adem in!",
    "Je staat tenminste niet in de wacht",
    "De server wordt aangedreven door een citroen",
    "We stellen je geduld op de proef",
    "Alsof je een andere keuze had",
    "Wachten totdat de satelliet in positie komt",
    "Zijn we er al?",
    "Tel maar tot 10",
    "Waarom zo serieus?",
    "Terugtellen vanaf oneindig",
    "Geen paniek",
    "Dus... Kom je hier vaak?",
    "Waarschuwing: steek jezelf niet in de hens",
    "We maken een koekje voor je",
    "Het rad van fortuin draaien",
    "Kans op succes berekenen",
    "Hoe noem je 8 Hobbits? Een Hobbyte",
    "Is dit Windows?",
    "Wacht alstublieft tot de luiaard begint te bewegen",
    "Ik zweer dat het bijna klaar is",
    "Laten we een minuutje mindfulness nemen",
    "Luisteren naar het geluid van één hand die klapt",
    "Zorgen dat alle puntjes hieronder staan",
    "De hamster laten draaien",
    "AI overtuigen om niet kwaad te worden",
    "Hoe ben je hier gekomen?",
    "Wacht, ruik je iets brandends?",
    "42",
    "Ik denk dat ik ben, daarom ben ik. Denk Ik",
    "het gebeurt",
    "Een paar zeeschildpadden vastbinden",
    "Wat is de vliegsnelheid van een onbeladen zwaluw?",
    "Ik wist niet dat verf zo snel droogde.",
    "De hond uitlaten",
    "Delen door nul",
    "Als dit over 5 minuten niet klaar is, wacht dan gewoon langer.",
    "We hebben een grotere boot nodig.",
    "Webontwikkelaars doen het met &lt;style&gt;",
    "Encryptie op militair niveau kraken",
    "Op zoek naar gevoel voor humor, wacht even",
    "BRB",
    "TODO: grappig laadbericht invoegen",
    "Laten we hopen dat het het wachten waard is",
    "Draai gerust rond in je stoel",
    "Wat de wat?",
    "Meer RAM downloaden",
    "Alt-F4 versnelt dingen",
    "Pixels duwen",
    "Lekker weertje hè",
    "Een muur bouwen",
    "Alles in dit universum is een aardappel of geen aardappel",
    "Geduld, het is moeilijk!",
    "Nieuwe manieren ontdekken om je te laten wachten",
    "TODO: Liftmuziek invoegen",
    "Het zware werk doen",
    "We werken heel hard ... Echt",
    "je bent nummer 2147483647 in de wachtrij",
    "Eenhoorns voeren",
    "Vind je mijn laadanimatie leuk? Ik heb hem zelf gemaakt",
    "Een mississippi, twee mississippi...",
    "Geen paniek... AHHHHH!",
    "Zorgen dat kabouters nog steeds kort zijn.",
    "IJs bakken...",
];

//==============================SCROLLING SYSTEM==============================
function getElementY(query) {
    return main.scrollTop + document.querySelector(query).getBoundingClientRect().top
}

function doScrolling(element, duration) {
    var startingY = main.scrollTop
    var elementY = getElementY(element)
    // If element is close to page's bottom then window will scroll only to some position above the element.
    var targetY = document.body.scrollHeight - elementY < main.innerHeight ? document.body.scrollHeight - main.innerHeight : elementY
    var diff = targetY - startingY

    // Easing function: easeInOutCubic
    // From: https://gist.github.com/gre/1650294
    var easing = function(t) {
        return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1
    }
    var start

    if (!diff) return

    diff -= 96;

    //add a percentage of diff to the duration
    duration += Math.abs(diff / 10);

    // Bootstrap our animation - it will get called right before next frame shall be rendered.
    window.requestAnimationFrame(function step(timestamp) {
        if (!start) start = timestamp
        // Elapsed miliseconds since start of scrolling.
        var time = timestamp - start
        // Get percent of completion in range [0, 1].
        var percent = Math.min(time / duration, 1)
        // Apply the easing.
        // It can cause bad-looking slow frames in browser performance tool, so be careful.
        percent = easing(percent)

        main.scrollTo(0, startingY + diff * percent)

        // Proceed with animation as long as we wanted it to.
        if (time < duration) {
            window.requestAnimationFrame(step)
        }
    })
}

//==============================BUTTON SYSTEM==============================
function createButtonForSidebar(icon, onclick) {
    const mainButton = document.createElement("li");
    mainButton.classList.add("menu-item");
    mainButton.classList.add("menu-item-custom");
    mainButton.id = "added-by-fetch";
    mainButton.addEventListener("click", onclick);
    
    const buttonIcon = document.createElement("div");
    buttonIcon.classList.add("fa-solid");
    buttonIcon.classList.add(icon);
    buttonIcon.classList.add("menu-icons");
    buttonIcon.classList.add("menu-icons-custom");
    buttonIcon.classList.add("fa-fw");
    mainButton.appendChild(buttonIcon);
    
    return mainButton;
}

var buttonCount = 0;
function createButtonForBottomRight(icon, onclick) {
    const mainButton = document.createElement("li");
    mainButton.classList.add("menu-item-Right");
    mainButton.classList.add("menu-item-Right-custom");
    mainButton.id = "added-by-fetch";
    mainButton.addEventListener("click", onclick);
    mainButton.style.bottom = `calc(var(--padding) + ${buttonCount * 48}px + var(--padding) * ${buttonCount})`;
    buttonCount++;
    
    const buttonIcon = document.createElement("div");
    buttonIcon.classList.add("fa-solid");
    buttonIcon.classList.add(icon);
    buttonIcon.classList.add("menu-icons");
    buttonIcon.classList.add("menu-icons-custom");
    buttonIcon.classList.add("fa-fw");
    mainButton.appendChild(buttonIcon);
    
    return mainButton;
}

function addButtonToPage(icon, onclick) {
    buttonCount = document.querySelectorAll('.menu-item-custom').length;
    document.querySelector(".bottom ul").appendChild(createButtonForSidebar(icon, onclick));
    document.querySelector(".bottom ul").appendChild(createButtonForBottomRight(icon, onclick));
}

//==============================UTILITY FUNCTIONS==============================
let isMobile = () => !!(navigator.userAgent.match(/Android/i) ||
    navigator.userAgent.match(/webOS/i) ||
    navigator.userAgent.match(/iPhone/i) ||
    navigator.userAgent.match(/iPad/i) ||
    navigator.userAgent.match(/iPod/i) ||
    navigator.userAgent.match(/BlackBerry/i) ||
    navigator.userAgent.match(/Windows Phone/i));