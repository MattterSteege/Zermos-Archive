---
layout: default
title: what's new
nav_exclude: false
---

# Release Notes - Versie 0.2.0 - 14-08-2023

0.2 - The small update...

* notitie, vanaf nu zijn er 2 versies van Zermos web: 'Zermos web' een de 'PWA', ze verschillen niet veel van elkaar, dus wat voor het een geldt, geldt ook voor het ander. Als je vragen hebt over beide, kijk dan bij de docs.

- Algemeen
  - We hebben een P, we hebben een W, we hebben een A, we hebben nu een PWA! Zermos web is nu ook te downloaden als een website, dit is nog experimenteel en kan dus nog bugs bevatten (en is voor nu alleen beschikbaar op PC), maar wel ongelovelijk handig. Zermos is tijdens het gebruik van de PWA (niet de website versie) in staat om pagina's op te slaan op je telefoon en die aan je te laten zien voor als je geen internet hebt. Zo kan je dus ook offline je rooster, cijfers en huiswerk bekijken, het is alleen niet het nieuwste van het nieuwste.
  - URL-balk kleur is nu weer 'responsive' naar je geselecteerde thema.
  - Experimentele functie zijn verwijderd uit de publieke versie van de website
  - Alleen [leerlingnummer]@ccg-leerling.nl is nu nog toegestaan als email adres voor het inloggen op Zermos.
  - 'zachtjes tikt de regen op mijn zolderraam, ik heb geen zin om naar school te gaan', nou, dat is toevallig. Zermos heeft laat nu de regen rondom school zien, handig zeg!

- Infowijs
  - Caching is nu ook doorgevoerd in de 'infowijs' pagina's, dat bezorgt jouw weer een snellere laadtijd!
  - Verschillende UI verbeteringen

- Zermelo
  - 'Since the dawn of time, humanity has struggled with time. So they created timezones, 24 of them. Some are +1.00, some are -9.00, but they all have one thing in common: they are a pain in the...', nou, mijn mening over de manier waarop ik het rooster uit Zermelo moet verwerken is dus ook duidelijk.
  - er is nu op pc een balk waarin je een week terug en vooruit kan gaan, dus je kan met een beetje klikken je rooster van 5 maanden terug bekijken.
  - Woosh, woosh, woosh, swipe op mobiel met gemak door je rooster heen.

- Koppelingen
  - De login UI op de Somtoday pagina is nu wél aanwezig
  - Het laad-icoontje op de Infowijs QR pagina staat nu niet achter de tekst
  - Je kan nu ook je email gebruiken om infowijs te koppelen!

- Somtoday - cijfers
  - Er is een nieuwe statistiek toegevoegd: 'cijfer verandering'. je krijgt nu te zien hoe je nieuwe cijfer je gemiddelde beïnvloedt (Bijv. CKV: +0.95% of Duits: -382.52%).

# Release Notes - Versie 0.1.3 - 06-08-2023

- Algemeen
    - De oude 'Docs' pagina (https://zermos.kronk.tech/Docs) is verangen door een nieuwe pagina (https://zermos-docs.kronk.tech) met een betere UI en meer informatie
    - Meerdere kleine UI verbeteringen
    - De icoontjes hebben een nieuw likje verf gekregen
    - Het mail login systeem werkt weer, dus inloggen is weer helemaal chill ;)

- Somtoday - Huiswerk
    - De 'huiswerk toevoegen' knop is nu zichtbaar op de navigatiebalk
    - Huiswerk wordt nu voor 1 uur opgeslagen in de database, zodat het laden sneller gaat en Somtoay niet overbelast wordt

- Somtoday - Cijfers
    - Cijfers worden nu voor 15 minuten opgeslagen in de database, zodat het laden sneller gaat en Somtoay niet overbelast wordt

- Infowijs - School nieuws
    - Er is nu een 'lees meer' knop aanwezig, zodat alles wat overzichtelijker is terwijl je nog steeds alles kan lezen
    - Mogelijke bijlagen worden nu onder aan het bericht weergegeven, i.p.v. halverwege
    - Er is nu een notificatie bubble rechts bovenaan het nieuws bericht, net zoals bij de cijfers

- Koppelingen - Zermelo
    - Er is nu een optie om qr-code login te gebruiken, dan hoef ik je wachtwoord niet te weten ;) Je vind je qr-code in Zermelo op https://ccg.zportal.nl/main/ > koppelingen > koppel externe applicatie
    - Er is nu een optie om de koppelcode van zermelo te gebruiken, ook nu hoef ik dus je wachtwoord niet te weten. Je vind je koppelcode in Zermelo op https://ccg.zportal.nl/main/ > koppelingen > koppel externe applicatie
    - De UI voor het koppelen van Zermelo is nu een stukkie mooier
    - Wachtwoord koppelen is volledig verwijderd, op de oude pagina staan nu de qr-code en koppelcode opties

- Koppelingen - Infowijs
    - Er is nu een laad iccoontje aanwezig, zodat je weet dat er iets gebeurd

- Under the hood
    - Het mail systeem is verbeterd, het is nu een stukkie sneller en werkt op meer devices

# Release Notes - Versie 0.1.2 - 18-07-2023 🎉

- Algemeen
    - Nice email UI
    - Je krijgt nu een mail binnen als je Zermos wilt gebruiken, dus Zermos is up and running :)

- Somtoday - huiswerk
    - De UI is nu beter, het bevat nu een duidelijker overzicht van alle huiswerk types (groen = zelf toegevoegd, oranje = toets, rood = grote toets, blauw = bijlage(n) aanwezig)

# Release Notes - Versie 0.1.1 - 15-07-2023 🚀

- Algemeen
    - Visuele veranderingen zijn doorgevoerd
    - Automatisch css en js refreshen is toegevoegd, dit houdt de UI een beetje op orde
    - Errors worden nu in een klein berichtje getoond
    - 'Hulp nodig?' knop wordt verborgen als het scherm the klein is (qua hoogte)
    - De 'nieuwste versie' popup staat nu wél over alle andere UI

- Account
    - Jouw informatie (inlog tokens, email, etc.) is niet meer zichtbaar en wordt niet doorgegeven aan jou, dit zorgt ervoor dat jouw informatie veiliger is.

- Zermelo - Rooster
    - Rooster ondersteunt nu ook activiteiten en examens
    - Rooster wordt nu gerenderd gebaseerd op de start en eind tijd, niet op lesuur (lesuur 1, 2, 3, etc)
    - Rooster UI is aangepast, het type les wordt nu met een kleur aangegeven (les = blauw, activiteit = groen, examen = oranje, uitval = rood)
    - Rooster is nu sneller (omdat het niet meer door 5*9 lessen hoeft te loopen, maar door de algemene +/- 30 lessen)

- Infowijs - Kalender
    - Kalender wordt nu elke 12 uur cyclus (0:00 - 11:59 / 12:00 - 23:59) opgeslagen en vervolgens wordt die versie gebruikt om de kalender te tonen, dit zorgt ervoor dat de laadtijd van de kalender een stuk korter is.

- School - Informatieboord
    - Informatieboord wordt nu elke 12 uur cyclus opgeslagen en vervolgens wordt die versie gebruikt om het informatieboord te tonen, dit zorgt ervoor dat de laadtijd van het informatieboord een stuk korter is.
    - Klikken op berichten voor extra details is verwijderd, de details worden nu direct getoond, dit zorgt ervoor dat de laadtijd van het informatieboord een stukje korter is.

- Somtoday - Cijfers
    - URL's zijn nu logischer om mee te werken
    - De 'last-seen' feature geeft nu geen error meer, dus je kan je cijfers weer bekijken...

# Release Notes - Versie 0.1 - 10-07-2023 🎈

- Toegevoegde cijferberekeningen (wat moet ik halen, wat ga ik staan) aan somtoday cijfers.
- Routes bijgewerkt om met een hoofdletter te beginnen.
- Probleem met inloggen opgelost met retour-URL.
- SchoolKalender sneller gemaakt.
- Iconen van rooster en schoolkalender bijgewerkt.
- Probleem met somtoday-refresh_token opgelost.
- Aangepaste huiswerk-functionaliteit toegevoegd.
- Nieuwe versie-melding toegevoegd.
- Scrollen in de zijbalk en mobiele lay-out gerepareerd.
- Nieuwe cijfer-melding weergegeven op het cijfer zelf.
- Mobiele zijbalk verplaatst naar links, omdat dit gemakkelijker bereikbaar is.
- Gedetailleerde berichtpagina van de schoolinformatie verwijderd, omdat deze volledig nutteloos was 😛
- 'Wat is er nieuw'-pagina toegevoegd, dat is deze pagina dus
