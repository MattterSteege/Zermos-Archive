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

//==============================HISTORY SYSTEM==============================
const customHistory = {
  history: [],
  currentIndex: -1,

  // Function to navigate to a new page and update the history
  navigateTo(url) {
    // Add the current page to the history if it's not a forward navigation
    if (this.currentIndex < this.history.length - 1) {
      this.history = this.history.slice(0, this.currentIndex + 1);
    }
    this.history.push(url);
    this.currentIndex = this.history.length - 1;

    // // Replace the page content with the new URL (you need to implement this)
    // ReplacePage(url);

    // Update the browser's URL
    window.history.pushState({ url }, "", url);
    //console.log(this.history, this.currentIndex);
  },

  // Function to go back in history
  historyBack() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      const url = this.history[this.currentIndex];
      ReplacePage(url, true);
      window.history.pushState({ url, index: this.currentIndex }, "", url);
    }
  },

  // Function to go forward in history
  historyForward() {
    if (this.currentIndex < this.history.length - 1) {
      this.currentIndex++;
      const url = this.history[this.currentIndex];
      ReplacePage(url, true);
      window.history.pushState({ url, index: this.currentIndex }, "", url);
    }
  },
};

//If user presses back or forward button, navigate to the correct page
window.onpopstate = function (event) {
  if (event.state) {
    //check if the event.state.url is in the history and check if it is the previous or next page
    if (customHistory.history.includes(event.state.url)) {
      if (customHistory.currentIndex > event.state.index) {
        customHistory.historyBack();
      } else if (customHistory.currentIndex < event.state.index) {
        customHistory.historyForward();
      }
    } else {
      ReplacePage(event.state.url, true);
    }
  }
};
