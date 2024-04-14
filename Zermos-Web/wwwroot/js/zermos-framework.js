const Zermos = {
    CurrentVersion: "",
    mainBeforeLoad: function (callback) {},
    mainAfterLoad: function (callback) {},
    mainUnload: function (callback) {},
    checkForUpdates: function (callback) {},
};

//==============================VARIABLES==============================
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

//==============================SCROLLING SYSTEM TOP==============================
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

//==============================SCROLLING SYSTEM LEFT==============================
function getElementX(query) {
    return main.scrollLeft + document.querySelector(query).getBoundingClientRect().left
}

function doScrollingLeft(scrollelement, element, duration) {
    var startingX = scrollelement.scrollLeft
    var elementX = getElementX(element)
    // If element is close to page's bottom then window will scroll only to some position above the element.
    var targetX = document.body.scrollWidth - elementX < main.innerWidth ? document.body.scrollWidth - main.innerWidth : elementX
    var diff = targetX - startingX
    
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
        
        main.scrollTo(startingX + diff * percent, 0)
        
        // Proceed with animation as long as we wanted it to.
        if (time < duration) {
            window.requestAnimationFrame(step)
        }
    }
    )
}

//==============================BUTTON SYSTEM==============================
function createButtonForSidebar(icon, onclick, background) {
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
    buttonIcon.style.background = `var(--${background === Background.Primary ? 'inactive-color' : background})`;
    mainButton.appendChild(buttonIcon);
    
    if (background === Background.Accent)
        buttonIcon.style.color = "var(--white-color)";
    else
        buttonIcon.style.color = "var(--text-color)";

    return mainButton;
}

var buttonCountHorizontalTopRight = 1;
var buttonCountVerticalTopRight = 1;
var buttonCountHorizontalBottomRight = 0;
var buttonCountVerticalBottomRight = 0;
var bottomRightCornerOccupied = false;

// Enum for vertical alignment
const VerticalAlignment = {
    TB: "TopRightToBottom",
    TL: "TopRightToLeft",
    BL: "BottomRightToLeft",
    BT: "BottomRightToTop"
};

const Background = {
    Accent: "accent-color",
    Primary: "overlay-color",
}

function createButtonForBottomRight(icon, onclick, verticalAlignment, background) {
    const mainButton = document.createElement("li");
    mainButton.classList.add("menu-item-right");
    mainButton.style.background = `var(--${background})`;
    mainButton.id = "added-by-fetch";
    mainButton.addEventListener("click", onclick);

    switch (verticalAlignment) {
        case VerticalAlignment.TB: //done
            mainButton.style.top = `calc(var(--padding) + ${buttonCountVerticalTopRight * 48}px + var(--padding) * ${buttonCountVerticalTopRight})`;
            buttonCountVerticalTopRight++;
            break;
        case VerticalAlignment.TL: //done
            mainButton.style.top = `var(--padding)`;
            mainButton.style.right = `calc(var(--padding) + ${buttonCountHorizontalTopRight * 48}px + var(--padding) * ${buttonCountHorizontalTopRight})`;
            buttonCountHorizontalTopRight++;
            break;
        case VerticalAlignment.BL:
            mainButton.style.bottom = `var(--padding)`;
            mainButton.style.right = `calc(var(--padding) + ${buttonCountHorizontalBottomRight * 48}px + var(--padding) * ${buttonCountHorizontalBottomRight})`;
            buttonCountHorizontalBottomRight++;
            bottomRightCornerOccupied = true;
            break;
        default:
        case VerticalAlignment.BT:
            mainButton.style.bottom = `calc(var(--padding) + ${buttonCountVerticalBottomRight * 48}px + var(--padding) * ${buttonCountVerticalBottomRight})`;
            buttonCountVerticalBottomRight++;
            bottomRightCornerOccupied = true;
            break;
    }
    
    if (bottomRightCornerOccupied && (buttonCountVerticalBottomRight === 0 || buttonCountHorizontalBottomRight === 0))
        if (buttonCountVerticalBottomRight === 0)
            buttonCountVerticalBottomRight++;
        else
            buttonCountHorizontalBottomRight++;

    const buttonIcon = document.createElement("div");
    buttonIcon.classList.add("fa-solid");
    buttonIcon.classList.add(icon);
    buttonIcon.classList.add("menu-icons");
    buttonIcon.classList.add("menu-icons-custom");
    buttonIcon.classList.add("fa-fw");
    
    if (background === Background.Accent) 
        buttonIcon.style.color = "var(--white-color)";
    else
        buttonIcon.style.color = "var(--text-color)";
    
    mainButton.appendChild(buttonIcon);

    return mainButton;
}

function addButtonToPage(icon, onclick, verticalAlignment = VerticalAlignment.BT, background = Background.Accent) {
    var buttons = document.querySelectorAll(".menu-item-right");
    if (buttons.length === 0) {
        buttonCountHorizontalTopRight = 1;
        buttonCountVerticalTopRight = 1;
        buttonCountHorizontalBottomRight = 0;
        buttonCountVerticalBottomRight = 0;
        bottomRightCornerOccupied = false;
    }
    if (verticalAlignment === VerticalAlignment.BL || verticalAlignment === VerticalAlignment.BT)
        document.querySelector(".bottom ul").appendChild(createButtonForSidebar(icon, onclick, background));
    else
        document.querySelector(".bottom ul .spacer").after(createButtonForSidebar(icon, onclick, background));
    document.querySelector(".bottom ul").appendChild(createButtonForBottomRight(icon, onclick, verticalAlignment, background));
}

//==============================UTILITY FUNCTIONS==============================
let isMobile = () => !!(navigator.userAgent.match(/Android/i) ||
    navigator.userAgent.match(/webOS/i) ||
    navigator.userAgent.match(/iPhone/i) ||
    navigator.userAgent.match(/iPad/i) ||
    navigator.userAgent.match(/iPod/i) ||
    navigator.userAgent.match(/BlackBerry/i) ||
    navigator.userAgent.match(/Windows Phone/i));

function copyToClipboard(text) {
    //first with the new api
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    } else {
        //fallback to the old api
        var textArea = document.createElement("textarea");
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            document.execCommand("copy");
        } catch (err) {
            console.error("Fallback: Oops, unable to copy", err);
        }

        document.body.removeChild(textArea);
    }
}

function waitForObject(obj, callback, interval = 100) {
    if (typeof obj !== 'undefined') {
        callback(obj);
    } else {
        setTimeout(function () {
            waitForObject(obj, callback, interval);
        }, interval);
    }
}
//==============================EVENT SYSTEM==============================
// Make sure only one event listener is added for each type to the #main element
let eventListeners = [];

function addEventListenerToMain(type, listener, options) {
    // Check if an event listener of the same type already exists for #main
    const existingListener = eventListeners.find(
        (eventListener) => eventListener.type === type && eventListener.listener === listener
    );

    // If not, add the new listener to #main and store it in the eventListeners array
    if (!existingListener) {
        eventListeners.push({
            type: type,
            listener: listener,
        });
        main.addEventListener(type, listener, options);
    }
}

const getEventListenersFromMain = () => eventListeners;

function removeEventListenerFromMain(type, listener) {
    // Remove the listener from the eventListeners array
    eventListeners = eventListeners.filter(
        (eventListener) => !(eventListener.type === type && eventListener.listener === listener)
    );

    // Remove the listener from #main
    main.removeEventListener(type, listener);
}

window.addEventListener("main:before-unload", () =>  Zermos.mainBeforeLoad());
window.addEventListener("main:load", () => Zermos.mainOnLoad());
window.addEventListener("main:before-unload", () => Zermos.mainBeforeUnload());

//==============================SHARING SYSTEM==============================
async function ZermosShareImage(title, text, blob) {
    const data = {
        files: [
            new File([blob], 'file.png', {
                type: blob.type,
            }),
        ],
        title: title,
        text: text,
    };
    try {
        if (!(navigator.canShare(data))) {
            return {name: "NotSupportedError", message: "Your browser does not support this feature"};
        }
        await navigator.share(data);
        return true;
    } catch (err) {
        return {name: err.name, message: err.message};
    }
}

//==============================UPDATE SYSTEM==============================
Zermos.checkForUpdates = () => {
    var version_user = document.cookie.split('; ').find(row => row.startsWith('version_used')).split('=')[1];
    if (version_user !== Zermos.CurrentVersion && version_user !== "") {
        //show modal
        new ZermosModal()
            .setTitle("Zermos is geupdate!")
            .addText("Zermos is weer een versietje ouder 🥳. Er zijn natuurlijk weer nieuwe functies toegevoegd en bugs gefixt. Veel plezier met de nieuwe versie!")
            .addText("Je gebruikt nu versie " + Zermos.CurrentVersion + ", de vorige keer dat je Zermos bezocht was dat versie " + version_user)
            .addButton("Check wat er nieuw is", function() {
                window.open("https://zermos-docs.kronk.tech/WhatsNew.html", "_blank");
            })
            .setSubmitButtonLabel("Let's go, ik ben er klaar voor!")
            .disableClose()
            .onSubmit(function() {
                var url = "/Account/UpdateSetting?key=version_used&value=" + Zermos.CurrentVersion;
                fetch(url, {
                    method: 'POST'
                });
                
                //set cookie version_used
                document.cookie = "version_used=" + Zermos.CurrentVersion + "; expires=Fri, 31 Dec 9999 23:59:59 GMT";
            })
            .open();
    }
    else if (version_user === ""){
        var url = "/Account/UpdateSetting?key=version_used&value=" + Zermos.CurrentVersion;
        fetch(url, {
            method: 'POST'
        });
        
        //set cookie version_used
        document.cookie = "version_used=" + Zermos.CurrentVersion + "; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    }
};

Zermos.mainUnload.bind(() => {
    console.log("unloading");
});

//==============================TITLE TEXT==============================
document.addEventListener('DOMContentLoaded', function() {
    const elements = document.querySelectorAll('.svgText');
    // console.log(elements);

    elements.forEach(function(element) {
        element.addEventListener('mouseover', function() {
            element.classList.add('animating');
        });

        element.addEventListener('transitionend', function() {
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
});
//==============================LOGGING OVERWRITE==============================
//overwrite console.log to log to the console and to an array
var consoleLog = console.log;
var consoleError = console.error;
var consoleWarn = console.warn;
var consoleInfo = console.info;
var consoleDebug = console.debug;
var consoleTrace = console.trace;

var consoleArray = [];

console.log = function() {
    consoleLog.apply(console, arguments);
    consoleArray.push([arguments, "log"]);
}

console.error = function() {
    consoleError.apply(console, arguments);
    consoleArray.push([arguments, "error"]);
}

console.warn = function() {
    consoleWarn.apply(console, arguments);
    consoleArray.push([arguments, "warn"]);
}

console.info = function() {
    consoleInfo.apply(console, arguments);
    consoleArray.push([arguments, "info"]);
}

console.debug = function() {
    consoleDebug.apply(console, arguments);
    consoleArray.push([arguments, "debug"]);
}

console.trace = function() {
    consoleTrace.apply(console, arguments);
    consoleArray.push([arguments, "trace"]);
}

//==============================PREVIEW SYSTEM==============================
//cookie preview=microsoft-somtoday-zermelo > TogglePreview("microsoft") > preview=somtoday-zermelo
function TogglePreview(preview) {
    var cookie = document.cookie;
    var previewCookie = cookie.split(";").find((c) => c.trim().startsWith("preview="));
    if (previewCookie === undefined) {
        // Set cookie with expiration date 10 years from now
        var expiresDate = new Date();
        expiresDate.setFullYear(expiresDate.getFullYear() + 10);
        document.cookie = "preview=" + preview + "; expires=" + expiresDate.toUTCString();
    } else {
        var previews = previewCookie.split("=")[1].split("-");
        if (previews.includes(preview)) {
            previews.splice(previews.indexOf(preview), 1);
        } else {
            previews.push(preview);
        }

        // Set cookie with expiration date 10 years from now
        var expiresDate = new Date();
        expiresDate.setFullYear(expiresDate.getFullYear() + 10);
        document.cookie = "preview=" + previews.join("-") + "; expires=" + expiresDate.toUTCString();
    }

    var element = document.querySelector("[data-preview-id='" + preview + "']");
    if (element !== null) {
        if (HasPreview(preview)) {
            element.classList.add("active");
        } else {
            element.classList.remove("active");
        }
    }
}

function HasPreview(preview) {
    var cookie = document.cookie;
    var previewCookie = cookie.split(";").find((c) => c.trim().startsWith("preview="));
    if (previewCookie === undefined) {
        return false;
    } else {
        return previewCookie.split("=")[1].split("-").includes(preview);
    }
}