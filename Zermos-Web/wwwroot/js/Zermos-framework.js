const Zermos = {
  CurrentVersion: "",
  mainBeforeLoad: function(callback) {},
  mainAfterLoad: function(callback) {},
  mainUnload: function(callback) {},
  checkForUpdates: function(callback) {},
  mainProcessAccessibility: function() {},
  installPrompt: null,
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
  "laadbericht aan het laden...", //dankje Micha
  "Ruikt het naar wortels? - Een sneeuwpop",
  "Een beetje zout toevoegen",
  "De kat uit de boom kijken",
  "Het is duidelijk GIF",
  "Don't worry, be happy.",
  "we zijn moeilijke dingen aan het berekenen",
  "kiek 'm goan",
  "BIEM!",
  "Dammen bouwen",
  "Kroketten bakken",
  "Kokosnoten zijn geen specerijen",
  "Even een Frikandel Speciaal halen",
  "Stamppot aan het prakken",
  "Wacht even, we zijn nog aan het polderen",
  "De dijken aan het verhogen",
  "Zoeken naar de juiste toon op de triangel",
  "Gezellig, hè?",
  "Ik heb pech, ik sta in de file",
  "Nog even en je kunt weer lekker niksen",
  "Fiets aan het parkeren",
  "Ja, dág!",
  "Haring happen",
  "Tulpen planten",
  "Frikandelbroodjes aan het opwarmen",
  "Naar Flappie zoeken",
  "Worstenbroodjes bakken",
  "De waterstand meten",
  "De dropvoorraad aanvullen",
  "De stroopwafel perfect op je kopje plaatsen",
  "Fietsbanden oppompen",
  "De HEMA-worst snijden",
  "Hagelslag strooien",
  "Bitterballen uit het vet halen",
  "De kaasschaaf slijpen",
  "Op zoek naar de goedkoopste benzine",
  "Het gras maaien",
  "De Albert Heijn Bonuskaart scannen",
  "In de aanbieding!",
  "De XXL-winkelwagen van de Jumbo manoeuvreren",
  "Slootje springen",
  "Gourmetten voorbereiden",
  "Sinterklaasgedicht schrijven",
  "De kliko's buiten zetten",
  "Hutspot opwarmen",
  "Spruitjes wassen",
  "De buienradar checken",
  "Zoeken naar die ene stroopwafel onderin de pak",
  "De kerstboom uit de schuur halen (ja, nu al)",
  "Mokken verzamelen bij de Gamma",
  "De IKEA-handleiding ontcijferen",
];

//==============================SCROLLING SYSTEM TOP==============================
function getElementY(query) {
  return (
      main.scrollTop + document.querySelector(query).getBoundingClientRect().top
  );
}

function doScrolling(element, duration) {
  var startingY = main.scrollTop;
  var elementY = getElementY(element);
  // If element is close to page's bottom then window will scroll only to some position above the element.
  var targetY =
      document.body.scrollHeight - elementY < main.innerHeight ?
          document.body.scrollHeight - main.innerHeight :
          elementY;
  var diff = targetY - startingY;

  // Easing function: easeInOutCubic
  // From: https://gist.github.com/gre/1650294
  var easing = function(t) {
    return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
  };
  var start;

  if (!diff) return;

  diff -= 96;

  //add a percentage of diff to the duration
  duration += Math.abs(diff / 10);

  // Bootstrap our animation - it will get called right before next frame shall be rendered.
  window.requestAnimationFrame(function step(timestamp) {
    if (!start) start = timestamp;
    // Elapsed miliseconds since start of scrolling.
    var time = timestamp - start;
    // Get percent of completion in range [0, 1].
    var percent = Math.min(time / duration, 1);
    // Apply the easing.
    // It can cause bad-looking slow frames in browser performance tool, so be careful.
    percent = easing(percent);

    main.scrollTo(0, startingY + diff * percent);

    // Proceed with animation as long as we wanted it to.
    if (time < duration) {
      window.requestAnimationFrame(step);
    }
  });
}

//==============================SCROLLING SYSTEM LEFT==============================
function getElementX(query) {
  return (
      main.scrollLeft + document.querySelector(query).getBoundingClientRect().left
  );
}

function doScrollingLeft(scrollelement, element, duration) {
  var startingX = scrollelement.scrollLeft;
  var elementX = getElementX(element);
  // If element is close to page's bottom then window will scroll only to some position above the element.
  var targetX =
      document.body.scrollWidth - elementX < main.innerWidth ?
          document.body.scrollWidth - main.innerWidth :
          elementX;
  var diff = targetX - startingX;

  // Easing function: easeInOutCubic
  // From: https://gist.github.com/gre/1650294
  var easing = function(t) {
    return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
  };
  var start;

  if (!diff) return;

  diff -= 96;

  //add a percentage of diff to the duration
  duration += Math.abs(diff / 10);

  // Bootstrap our animation - it will get called right before next frame shall be rendered.
  window.requestAnimationFrame(function step(timestamp) {
    if (!start) start = timestamp;
    // Elapsed miliseconds since start of scrolling.
    var time = timestamp - start;
    // Get percent of completion in range [0, 1].
    var percent = Math.min(time / duration, 1);
    // Apply the easing.
    // It can cause bad-looking slow frames in browser performance tool, so be careful.
    percent = easing(percent);

    main.scrollTo(startingX + diff * percent, 0);

    // Proceed with animation as long as we wanted it to.
    if (time < duration) {
      window.requestAnimationFrame(step);
    }
  });
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
  buttonIcon.style.background = `var(--${
      background === Background.Primary ? "inactive-color" : background
  })`;
  mainButton.appendChild(buttonIcon);

  if (background === Background.Accent)
    buttonIcon.style.color = "var(--white-color)";
  else buttonIcon.style.color = "var(--text-color)";

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
  BT: "BottomRightToTop",
};

const Background = {
  Accent: "accent-color",
  Primary: "overlay-color",
};

function createButtonForBottomRight(
    icon,
    onclick,
    verticalAlignment,
    background
) {
  const mainButton = document.createElement("li");
  mainButton.classList.add("menu-item-right");
  mainButton.style.background = `var(--${background})`;
  mainButton.id = "added-by-fetch";
  mainButton.addEventListener("click", onclick);

  switch (verticalAlignment) {
    case VerticalAlignment.TB: //done
      mainButton.style.top = `calc(var(--padding) + ${
          buttonCountVerticalTopRight * 48
      }px + var(--padding) * ${buttonCountVerticalTopRight})`;
      buttonCountVerticalTopRight++;
      break;
    case VerticalAlignment.TL: //done
      mainButton.style.top = `var(--padding)`;
      mainButton.style.right = `calc(var(--padding) + ${
          buttonCountHorizontalTopRight * 48
      }px + var(--padding) * ${buttonCountHorizontalTopRight})`;
      buttonCountHorizontalTopRight++;
      break;
    case VerticalAlignment.BL:
      mainButton.style.bottom = `var(--padding)`;
      mainButton.style.right = `calc(var(--padding) + ${
          buttonCountHorizontalBottomRight * 48
      }px + var(--padding) * ${buttonCountHorizontalBottomRight})`;
      buttonCountHorizontalBottomRight++;
      bottomRightCornerOccupied = true;
      break;
    default:
    case VerticalAlignment.BT:
      mainButton.style.bottom = `calc(var(--padding) + ${
          buttonCountVerticalBottomRight * 48
      }px + var(--padding) * ${buttonCountVerticalBottomRight})`;
      buttonCountVerticalBottomRight++;
      bottomRightCornerOccupied = true;
      break;
  }

  if (
      bottomRightCornerOccupied &&
      (buttonCountVerticalBottomRight === 0 ||
          buttonCountHorizontalBottomRight === 0)
  )
    if (buttonCountVerticalBottomRight === 0) buttonCountVerticalBottomRight++;
    else buttonCountHorizontalBottomRight++;

  const buttonIcon = document.createElement("div");
  buttonIcon.classList.add("fa-solid");
  buttonIcon.classList.add(icon);
  buttonIcon.classList.add("menu-icons");
  buttonIcon.classList.add("menu-icons-custom");
  buttonIcon.classList.add("fa-fw");

  if (background === Background.Accent)
    buttonIcon.style.color = "var(--white-color)";
  else buttonIcon.style.color = "var(--text-color)";

  mainButton.appendChild(buttonIcon);

  return mainButton;
}

function addButtonToPage(
    icon,
    onclick,
    verticalAlignment = VerticalAlignment.BT,
    background = Background.Accent
) {
  var buttons = document.querySelectorAll(".menu-item-right");
  var button = [];
  if (buttons.length === 0) {
    buttonCountHorizontalTopRight = 1;
    buttonCountVerticalTopRight = 1;
    buttonCountHorizontalBottomRight = 0;
    buttonCountVerticalBottomRight = 0;
    bottomRightCornerOccupied = false;
  }

  button.push(createButtonForSidebar(icon, onclick, background));

  if (
      verticalAlignment === VerticalAlignment.BL ||
      verticalAlignment === VerticalAlignment.BT
  )
    document.querySelector(".bottom ul").appendChild(button[0]);
  else document.querySelector(".bottom ul .spacer").after(button[0]);

  button.push(
      createButtonForBottomRight(icon, onclick, verticalAlignment, background)
  );

  document.querySelector(".bottom ul").appendChild(button[1]);

  return button;
}

//==============================UTILITY FUNCTIONS==============================
let isMobile = () =>
    !!(
        navigator.userAgent.match(/Android/i) ||
        navigator.userAgent.match(/webOS/i) ||
        navigator.userAgent.match(/iPhone/i) ||
        navigator.userAgent.match(/iPad/i) ||
        navigator.userAgent.match(/iPod/i) ||
        navigator.userAgent.match(/BlackBerry/i) ||
        navigator.userAgent.match(/Windows Phone/i) ||
        navigator.userAgent.match(/zermos_mobile_app/i)
    );

let isNativeApp = () => !!navigator.userAgent.match(/zermos_mobile_app/i);

let isSmartWatch = () =>
    //check if screen is smaller than 600px
    window.matchMedia("(max-width: 600px)").matches;

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
  if (typeof obj !== "undefined") {
    callback(obj);
  } else {
    setTimeout(function() {
      waitForObject(obj, callback, interval);
    }, interval);
  }
}

// Define a function to append a script to either the head or body
function appendScript(element, scriptToWorkWith, isSrc) {
  const script = document.createElement("script");
  if (isSrc) {
    script.src = scriptToWorkWith.src;
    if (script.integrity)
      script.integrity = scriptToWorkWith.integrity;
    if (script.crossOrigin)
      script.crossOrigin = scriptToWorkWith.crossOrigin;
    if (script.referrerPolicy)
      script.referrerPolicy = scriptToWorkWith.referrerPolicy;
    if (script.type)
      script.type = scriptToWorkWith.type;
  } else {
    script.innerHTML = scriptToWorkWith.innerHTML;

    if (scriptToWorkWith.type) 
      script.type = scriptToWorkWith.type;
  }

  if (scriptToWorkWith.defer) {
    script.defer = true;
  }

  script.id = "added-by-fetch";
  element.appendChild(script);
  
  if (isSrc) {
    return script;
  }
}

// Define a function to append a stylesheet link to the head
function appendStylesheet(data) {
  const link = document.createElement("style");
  link.id = "added-by-fetch";
  link.innerHTML = data;
  document.head.appendChild(link);
  return link;
}

function stripTag(tag, data) {
  const div = document.createElement('div');
  div.innerHTML = data;
  const scripts = div.getElementsByTagName(tag);
  let i = scripts.length;
  while (i--) {
    scripts[i].parentNode.removeChild(scripts[i]);
  }
  return div.innerHTML;
}
//==============================EVENT SYSTEM==============================
// Make sure only one event listener is added for each type to the #main element
let eventListeners = [];

function addEventListenerToMain(type, listener, options) {
  // Check if an event listener of the same type already exists for #main
  const existingListener = eventListeners.find(
      (eventListener) =>
          eventListener.type === type && eventListener.listener === listener
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
      (eventListener) =>
          !(eventListener.type === type && eventListener.listener === listener)
  );

  // Remove the listener from #main
  main.removeEventListener(type, listener);
}

window.addEventListener("main:before-unload", () => Zermos.mainBeforeLoad());
window.addEventListener("main:load", () => Zermos.mainOnLoad());
window.addEventListener("main:before-unload", () => Zermos.mainBeforeUnload());

//==============================UPDATE SYSTEM==============================
Zermos.checkForUpdates = () => {
  var version_user = document.cookie
      .split("; ")
      .find((row) => row.startsWith("version_used"))
      .split("=")[1];
  if (version_user !== Zermos.CurrentVersion && version_user !== "") {
    //show modal
    new ZermosModal()
        .addHeading({text: "Zermos is geupdate!"})
        .addText({text: "Zermos is weer een versietje ouder 🥳. Er zijn natuurlijk weer nieuwe functies toegevoegd en bugs gefixt. Veel plezier met de nieuwe versie!"})
        .addText({text: "Je gebruikt nu versie " + Zermos.CurrentVersion + ", de vorige keer dat je Zermos bezocht was dat versie " + version_user})
        .addButton({text: "Check wat er nieuw is", onClick: (_) => {
          window.open("https://zermos-docs.kronk.tech/WhatsNew.html", "_blank");
        }})        
        .addButton({text: "Meer informatie in de Discord server", onClick: (_) => {
          window.open("https://discord.gg/krP9se64TD", "_blank");
        }})
        .addButton({text: "let's go, ik ben er klaar voor!", onClick: (ctx) => {
          var url =
              "/Account/UpdateSetting?key=version_used&value=" +
              Zermos.CurrentVersion;
          fetch(url, {
            method: "POST",
          });

          //set cookie version_used
          document.cookie =
              "version_used=" +
              Zermos.CurrentVersion +
              "; expires=Fri, 31 Dec 9999 23:59:59 GMT";
          
            ctx.close();
        }})
        .onCloseCallback(() => {
          var url =
              "/Account/UpdateSetting?key=version_used&value=" +
              Zermos.CurrentVersion;
          fetch(url, {
            method: "POST",
          });

          //set cookie version_used
          document.cookie =
              "version_used=" +
              Zermos.CurrentVersion +
              "; expires=Fri, 31 Dec 9999 23:59:59 GMT";

          ctx.close();
        })
        .open();

    localStorage.clear();
  } else if (version_user === "") {
    var url =
        "/Account/UpdateSetting?key=version_used&value=" + Zermos.CurrentVersion;
    fetch(url, {
      method: "POST",
    });

    //set cookie version_used
    document.cookie =
        "version_used=" +
        Zermos.CurrentVersion +
        "; expires=Fri, 31 Dec 9999 23:59:59 GMT";
  }
};

Zermos.mainUnload.bind(() => {
  Console.log("unloading");
});

//==============================TITLE TEXT==============================
document.addEventListener("DOMContentLoaded", function() {
  const elements = document.querySelectorAll(".svgText");
  // console.log(elements);

  elements.forEach(function(element) {
    element.addEventListener("mouseover", function() {
      //if screen is bigger than 1200px
        if (window.matchMedia("(min-width: 1200px)").matches) {
          element.classList.add("animating");
        }
    });

    element.addEventListener("transitionend", function() {
      element.classList.remove("animating");
    });
  });
  
  const top = document.querySelector(".top");
  top.addEventListener("click", function() {
    elements.forEach(function(element) {
        element.classList.add("animating");
      });
    
    setTimeout(() => {
        elements.forEach(function(element) {
            element.classList.remove("animating");
        });
      }, 300);
    });
});
//==============================LOGGING OVERWRITE==============================
// Overwrite console.log to log to the console and to an array
var consoleArray = [];

consoleLog = console.log;
console.log = function(...args) {
    consoleArray.push(["log", null, ...args]);
    consoleLog(`%c[Log]%c`, "font-weight:700;color:royalblue;", "", ...args);
};

consoleError = console.error;
console.error = function(...args) {
    consoleArray.push(["error", new Error().stack, ...args]);
    consoleError(`%c[Error]%c`, "font-weight:700;color:red;", "", ...args);
};

consoleWarn = console.warn;
console.warn = function(...args) {
    consoleArray.push(["warn", new Error().stack, ...args]);
    consoleWarn(`%c[Warn]%c`, "font-weight:700;color:gold;", "", ...args);
};

consoleInfo = console.info;
console.info = function(...args) {
    consoleArray.push(["info", null, ...args]);
    consoleInfo(`%c[Info]%c`, "font-weight:700;color:blue;", "", ...args);
};

consoleDebug = console.debug;
console.debug = function(...args) {
    consoleArray.push(["debug", new Error().stack, ...args]);
    consoleDebug(`%c[Debug]%c`, "font-weight:700;color:orange;", "", ...args);
};

window.onerror = function(message, source, lineno, colno, error) {
    consoleArray.push(["error", new Error().stack, message]);
    consoleError(
        `%c[Error]%c`,
        "font-weight:700;color:red;",
        "",
        `${message} in ${source}:${lineno}:${colno}`
    );
    
    return true;
};

//==============================NETWORKING OVERWRITE==============================
// Overwrite fetch to log to the console
var networkArray = [];

const originalFetch = window.fetch;
window.fetch = async function(...args) {
  const [resource, config] = args;
  const method = config?.method || 'GET';

  const startTime = performance.now(); // Start time

  try {
    const response = await originalFetch(...args);
    const status = response.status;
    const url = response.url;
    const endTime = performance.now(); // End time
    const fetchTime = endTime - startTime; // Calculate fetch time
    let errorMessage = null;

    if (status >= 400) {
      errorMessage = await response.text();
    }

    networkArray.push([method, url, status, fetchTime, errorMessage]);

    return response;
  } catch (error) {
    const endTime = performance.now(); // End time
    const fetchTime = endTime - startTime; // Calculate fetch time

    networkArray.push([method, resource, 0, fetchTime, error.message]);
    throw error;
  }
};

//==============================PREVIEW SYSTEM==============================
//cookie preview=microsoft-somtoday-zermelo > TogglePreview("microsoft") > preview=somtoday-zermelo
function TogglePreview(preview) {
  var cookie = document.cookie;
  var previewCookie = cookie
      .split(";")
      .find((c) => c.trim().startsWith("preview="));
  if (previewCookie === undefined) {
    // Set cookie with expiration date 10 years from now
    var expiresDate = new Date();
    expiresDate.setFullYear(expiresDate.getFullYear() + 10);
    document.cookie =
        "preview=" + preview + "; expires=" + expiresDate.toUTCString();
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
    document.cookie =
        "preview=" +
        previews.join("-") +
        "; expires=" +
        expiresDate.toUTCString();
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

function setPreview(preview, value) {
  var isActivated = HasPreview(preview);
    if (isActivated !== value) {
        TogglePreview(preview);
    }
}

function HasPreview(preview) {
  var cookie = document.cookie;
  var previewCookie = cookie
      .split(";")
      .find((c) => c.trim().startsWith("preview="));
  if (previewCookie === undefined) {
    return false;
  } else {
    return previewCookie.split("=")[1].split("-").includes(preview);
  }
}

//==============================================================
var sidebarLoadSpeed = 250;

function newSidebar(forceShow = false) {
  fetch("/data/sidebar?no-framework&forceShow=" + forceShow)
      .then(response => response.text())
      .then(data => {
        var sidebar = document.querySelector("#sidebar")

        //if the data is the same, don't reload the sidebar
        if (sidebar.innerHTML === data) return;

        var children = Array.from(sidebar.children);
        ease(1, 0, sidebarLoadSpeed * (0.8), (value) => {
          children.forEach((child) => {
            child.style.opacity = value;
          });
        });
        setTimeout(() => {
          sidebar.innerHTML = data;
          var children = Array.from(sidebar.children);
          children.forEach((child) => {
            child.style.opacity = 0;
          });
          ease(0, 1, sidebarLoadSpeed * (0.8), (value) => {
            children.forEach((child) => {
              child.style.opacity = value;
            });
          });
        }, sidebarLoadSpeed);
      });
}

function ease(start, end, time, callback) {
  start = Number(start);
  end = Number(end);
  const startTime = Date.now();
  const duration = time;

  function update() {
    const currentTime = Date.now();
    const elapsed = currentTime - startTime;

    if (elapsed >= duration) {
      clearInterval(interval);
      callback(end);
    } else {
      const progress = elapsed / duration;
      const easedValue = start + (end - start) * (progress * (2 - progress));
      callback(easedValue);
    }
  }

  const interval = setInterval(update, 10); // Update approximately every 10ms
}

//==============================OFFLINE DETECTION==============================
const status = document.querySelector(".status");
let isUserOnline = window.navigator.onLine;
window.addEventListener("load", () => {
  const handleNetworkChange = () => {
    if (navigator.onLine) {
      document.body.classList.remove("offline");
    } else {
      document.body.classList.add("offline");
    }
    isUserOnline = navigator.onLine;
    console.log(`User connection status changed is is now ${isUserOnline ? "online" : "offline"}`);
  };

  window.addEventListener("online", handleNetworkChange);
  window.addEventListener("offline", handleNetworkChange);
});

//==============================CACHING SYSTEM==============================
function cachePage(data, url) {
  if (!HasPreview("enable_cache")) return;
  
  var lowerUrl = url?.toLowerCase();
  if (lowerUrl.includes("login") || lowerUrl.includes("logout") || lowerUrl.includes("callback")) {
    return;
  }

  //save the current page data in localstorage
  localStorage.setItem(url, data);
}

function getCachePage(url) {
  new Promise((resolve) => {
    document.getElementsByClassName("loader-text")[0].innerHTML = loadingTexts[Math.floor(Math.random() * loadingTexts.length)];
    document.querySelectorAll(".loading-dots").forEach(function(dot) {
      dot.style.background = "";
      dot.style.animation = "";
      dot.style.transform = "";
    });

    content.style.opacity = "1";
    // Assuming a 250ms fade-in animation
    setTimeout(resolve, 250);
  })
  .then(() => {
    //get the current page data from localstorage
    var data = localStorage.getItem(url);

    if (data) {
      doPageReplace(data, url);
      return;
    }

    document.getElementsByClassName("loader-text")[0].innerHTML = "De pagina kon niet worden opgevraagd.";
    document.querySelectorAll(".loading-dots").forEach(function (dot) {
      dot.style.background = "var(--deny-color)";
      dot.style.animation = "unset";
      dot.style.transform = "scale(1)";
    });
    history.replaceState(null, " Zermos", url);
    
    setTimeout(() => {
      new Promise((resolve) => {
        content.style.opacity = "0";
        // Assuming a 250ms fade-out animation
        setTimeout(resolve, 250);
      });
      }, 250);
  });
}

//cache css
function cacheCSS(url, data) {
  if (!HasPreview("enable_cache")) return;
  return new Promise((resolve, reject) => {
    if (url.includes("fontawesome") || url.includes("zermos")) {
      resolve(data);
      return;
    }

    //save the current page data in localstorage
    localStorage.setItem(url, data);
    resolve(data);
  });
}

function getCacheCSS(url) {
  //get the current page data from localstorage
  var data = localStorage.getItem(url);

  if (data) {
    appendStylesheet(data);
  }
}

//==============================PAGE INTEGRETY==============================
function isPageStructureComplete() {
  const requiredElements = [
    'html', 'head', 'body', 'main#main',
    'div#sidebar', 'div#content', 'div.top-bar'
  ];

  for (let selector of requiredElements) {
    if (!document.querySelector(selector)) {
      console.warn(`Missing element: ${selector}`);
      return false;
    }
  }

  const requiredScripts = [
    '/js/Zermos-framework.min.js',
    '/js/Zermos-accessibility.min.js',
    '/js/Zermos-modal.min.js'
  ];

  for (let src of requiredScripts) {
    if (!document.querySelector(`script[src^="${src}"]`)) {
      console.warn(`Missing script: ${src}`);
      return false;
    }
  }

  const requiredStylesheets = [
    '/css/style.css',
    '/css/modal.css'
  ];

  for (let href of requiredStylesheets) {
    if (!document.querySelector(`link[rel="stylesheet"][href^="${href}"]`)) {
      console.warn(`Missing stylesheet: ${href}`);
      return false;
    }
  }

  return true;
}

