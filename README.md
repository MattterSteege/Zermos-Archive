<!-- Project header -->
<p align="center">
  <img src="https://github.com/MattterSteege/Zermos-Archive/raw/refs/heads/Zermos-Native-Android/app/src/main/res/mipmap-xxxhdpi/ic_launcher.png" alt="Zermos icon" width="96" height="96" />
  <br/>
  <h1>Zermos</h1>
  <i>Alle schoolinformatie in één oogopslag</i>
</p>

<!-- Badges -->
<p align="center">
  <a href="https://github.com/MattterSteege/Zermos-Archive">
    <img src="https://img.shields.io/github/stars/MattterSteege/Zermos-Archive?style=for-the-badge" alt="GitHub Stars" />
  </a>
  <a href="https://github.com/MattterSteege/Zermos-Archive/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/MattterSteege/Zermos-Archive?style=for-the-badge" alt="License: MIT" />
  </a>
  <a href="https://zermos.kronk.tech">
    <img src="https://img.shields.io/website?url=https%3A%2F%2Fzermos.kronk.tech&style=for-the-badge" alt="Website Status" />
  </a>
  <a href="https://github.com/MattterSteege/Zermos-Archive/blob/Docs/WhatsNew.md">
    <img src="https://img.shields.io/badge/Changelog-View-blue?style=for-the-badge" alt="Changelog" />
  </a>
</p>

---

# Welkom bij Zermos 🎉

Leuk dat je geïnteresseerd bent in **Zermos**!  
Zermos is een webapplicatie die je net dat beetje extra geeft. Het laat je **Somtoday**, **Zermelo** en **Infowijs** koppelen aan je account, zodat je in één oogopslag al je info bij elkaar hebt — superhandig.  

Deze pagina’s zijn bedoeld om je wegwijs te maken in Zermos.

---

## ✨ Wat Zermos kan (als je alles hebt gekoppeld):

- 📅 Je rooster bekijken én delen met vrienden  
- 📊 Je cijfers tot in detail inzien  
- 📝 Zelf huiswerk plannen en filteren  

➡️ **Check het hier:** [https://zermos.kronk.tech](https://zermos.kronk.tech)

---

## 📖 Het verhaal achter Zermos

Ik heb Zermos over een paar jaar tijd gebouwd, met veel plezier. Op een gegeven moment hing ik zelfs posters door de school om meer leerlingen te overtuigen het te gebruiken. Dat leverde uiteindelijk maar zo’n ±100 gebruikers op, maar geloof me: het voelde ultiem gaaf om te zien dat het werkte.

Nu ben ik klaar met de middelbare school en vind ik dat dit project **voor iedereen toegankelijk** moet zijn. Er zit ongelofelijk veel werk in de koppelingen, en ik heb heel wat uitgezocht over de systemen van Somtoday en Infowijs. Daarom deel ik nu alles met jullie: leer ervan, bouw erop verder, doe ermee wat je wil — **(wel graag conform licentie 😉)**.

---

## ⚠️ Belangrijke kanttekeningen

> **Let op:** Zermos, Somtoday, Zermelo en Infowijs hebben **geen** officiële samenwerking of contract. Buiten Zermelo werd het project eigenlijk niet “gewenst” 😬.

- Dit is een **archief** van de volledige code, niet per se bedoeld om zelf 1-op-1 te hosten. Het mag wel, maar houd er rekening mee dat alles is gebouwd op mijn eigen infrastructuur.  
- Ja, ik heb fouten gemaakt. Heel veel. Vooral hardcoded databasewachtwoorden (die nu allemaal gepatcht zijn). Dus nee, je gaat geen bruikbare wachtwoorden vinden. Alles wat je ziet is oud en onbruikbaar.  

---

## 🛠️ Gebruikte technologieën & evolutie van Zermos

Het hele project is echt een groeiverhaal, met behoorlijk wat omzwervingen in technologie en aanpak — ik neem je even mee:

1. **Unity project (vanuit niks)**  
   De allereerste versie van Zermos was een experiment in Unity, puur vanilla. Geen frameworks, niks fancy, gewoon leren door doen. Dit was vooral een proof of concept, niks voor productie.

<details>
<summary>Filmpje met hoe dat eruit zag</summary>

https://github.com/user-attachments/assets/e9f8bda6-aa8e-48d2-94f5-2d00616127bd

</details>

2. **Raw HTML/CSS/JS website**  
   Daarna ben ik helemaal teruggegaan naar basics: een pure statische website met vanilla HTML, CSS en JavaScript. Lekker direct, geen gedoe met backend, maar wel beperkt in wat je kon doen met de koppelingen.

<details>
<summary>Wat was dit intens lelijk 😬</summary>

![Screenshot 1](https://github.com/user-attachments/assets/754fe313-10cd-4d9b-a99b-51010e346ee4)
![Screenshot 2](https://github.com/user-attachments/assets/ac579d97-3d12-4efe-a670-2dca34415eb0)
![Screenshot 3](https://github.com/user-attachments/assets/805f0251-ac37-43a5-bb12-0adb5f45c3eb)

</details>

3. **ASP.NET Core website (huidige versie)**  
   Toen vond ik het tijd voor iets stevigers: een ASP.NET Core backend, waarmee ik alle koppelingen serieus kon aanpakken en dynamische functionaliteit kon bouwen. Dit is nu het kloppende hart van Zermos.

   Dit gaf me ook de optie om een database voor gebruikers aan te maken, dus dan konden mensen op hun computer, tablet, ipad, laptop, telefoon, en ja ook smartwatch hun rooster, cijfers en alles bekijken!

4. **Windows, Android en PWS apps**  
   Om het af te maken zijn er ook native apps gekomen:  
   - Allen zijn in feite een webview van de website.  
   - Met extra kleine features, zoals deeplinking, speciaal voor de Somtoday-koppeling. (android en windows)  
   - Hiermee kon ik gebruikers een app-gevoel geven zonder alles helemaal opnieuw te bouwen.

---

> Zermos is dus niet één rechtlijnig project, maar een verzameling experimenten, verbeteringen en platformen die samen het huidige product vormen. Het was soms frustrerend en chaotisch, maar ook super leerzaam.

---


## 🖼️ Screenshots

<details>
<summary>Klik om screenshots te tonen</summary>

![Screenshot 4](https://github.com/user-attachments/assets/6f042921-c219-4b68-86d4-87fb9312e287)  
![Screenshot 5](https://github.com/user-attachments/assets/79ea16e3-76e0-44f2-ae09-ac769987c358)  
![Screenshot 6](https://github.com/user-attachments/assets/086069fb-1a62-4717-9e1f-cae126befb9b)  
![Screenshot 7](https://github.com/user-attachments/assets/042220c7-9ba3-41d3-9e6f-f1e55f1b3a02)  
![Screenshot 8](https://github.com/user-attachments/assets/912c0fec-c5fd-4728-917d-446e191d3f24)  

> Niet alle screenshots zijn beschikbaar — ik heb geen toegang meer tot Somtoday, dus de cijferpagina’s ontbreken.
</details>

---

## 🎨 Promo posters

<details>
<summary>Klik om posters te bekijken</summary>

![Poster 1](https://github.com/user-attachments/assets/1bb18476-3cb8-45df-bb94-01783969f7b3)  
![Poster 2](https://github.com/user-attachments/assets/6bedbe35-76ba-42d4-9b45-dc103a31a0bd)

</details>

---

## 🌐 Hosting & documentatie

Voor nu blijf ik Zermos gewoon hosten, inclusief documentatie:  
[https://zermos-docs.kronk.tech](https://zermos-docs.kronk.tech)

---

## 📜 Changelog

De volledige changelog vind je hier:  
[**📄 Bekijk de changelog**](https://github.com/MattterSteege/Zermos-Archive/blob/Docs/WhatsNew.md)

---

## 🛠️ Disclaimers

- Ja, er zat een **backdoor** in — maar alleen in de *debug build* en alleen als het ingelogde e-mailadres hetzelfde was als het mijne. Daarmee kon ik inloggen op accounts van anderen, puur om te helpen tijdens de moeizame uitrol.  
- **Licentie:** MIT. Je mag het gebruiken, verkopen, aanpassen, maar **niet claimen als eigen code**. Ik heb hier jaren aan gewerkt, door 5 à 6 totaal verschillende versies heen. Het minste wat ik verdien, is dat je me credit geeft.

---
