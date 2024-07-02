---
layout: default
title: what's new
nav_exclude: false
---

# Release Notes - Versie 0.5.2 / Versie 0.5.3 - 02-07-2024
0.5.3 Breaking news, Zermos takes over the Netherlands

- infowijs
    - het maakt niet uit welke versie (of van welke school) je bent, als je toegang hebt tot de Hoy app of een variatie daarvan, dan kan je nu op Zermos koppelen*

- Account
    - Je kan nu inloggen met een school of een persoonlijk Microsoft account
    - als je in of uitlogt dan wordt de sidebar geupdate met de beschikbare functies (uitgelogd: inloggen knop, ingelogd: alle gekoppelde functies)
    
- school (mijn school)
    - als je email niet eindig op @ccg-leerling.nl dan wordt printen en informatiebord verborgen
    
- Somtoday
    - Je kunt nu je cijfers verversen door op de refresh knop te drukken 
    
- Zermelo 
    - Je kan nu je schooltijden aanpassen in de instellingen, deze worden dan gebruikt voor je rooster
    
- Koppelen
    - Zermelo vereist nu een schoolcode om te koppelen, dit zodat Zermos langzamerhand public kan gaan
    - Somtoday kan nu gekoppeld worden door leerlingen van elke school die geen externe provider gebruikt 
    - Als je een app koppelt dan wordt de sidebar geupdate met de nieuwe beschikbare functies
    
    

# Release Notes - Versie 0.5.1 - 28-06-2024
0.5.1 bugfixes

- Debug
    - Added an Eruda debug instance to Zermos, but only when specifically enabled by the user. This will help me debug issues that are hard to reproduce.

- Zermelo - Rooster
    - Als je een smartwatch gebruikt, dan wordt je nu automatisch naar je rooster, je kan geen andere pagina's openen.

- Somtoday - leermiddelen
    - Sommige konden de leermiddelen pagina niet openen, maar dat is nu opgelost.

- Somtoday - cijfers
    - Voor Zermos recap moest ik cijfers opvragen en moest ik dus de grond leggen voor cijfers uit andere jaarlagen. Ik wil geen beloftes maken, maar het zou kunnen dat ik binnenkort cijfers uit andere jaarlagen kan laten zien.

- Zermos
    - Zermos recap komt er aan, maar hou dat nog even tussen ons...
    - Op de eerste keer pagina is de leermiddelen knop ook uitgelegd en is er een spelfout verbeterd.
    - Een installeer Zermos knop is toegevoegd aan de account pagina.

# Release Notes - Versie 0.5 - 22-06-2024
0.5 Guess who's back, back again V2

- algemeen
    - Mocht er een error zijn tijdens het aanvragen van nieuwe pagina's dan wordt je URL aangepast naar de opgevraagde pagina zodat je, als je opnieuw probeert, je niet de vorige pagina weer ziet.
    
- Account - Debug
    - Mocht je cache niet goed werken, dan kan je nu je cache legen via de debug pagina.

- School - informatiebord
    - Het informatiebord wordt nu client-side gerenderd, waardoor de server minder belast wordt en de pagina sneller laadt. Ook wordt de data lokaal gecached, zodat de pagina niet elke keer opnieuw hoeft te laden.
    
- Somtoday - leermiddelen!?!?!?
    - Jaja, ik heb na diep graven een nieuwe, betrouwbare manier gevonden om de leermiddelen op te halen. Dit is nu uiteraard gelijk geïmplementeerd in de app.
    - Je kan nu ook je eigen leermiddelen toevoegen, handig voor de boeken die níet in Somtoday staan.

- Somtoday - afwezigheid
    - Je afwezigheid loopt nu niet achter, maar wordt elke keer opnieuw opgehaald. Dit zorgt ervoor dat je altijd de meest recente afwezigheid ziet.

- Koppelingen - Somtoday
    - De koppeling met SOM werkt weer <img src="https://cultofthepartyparrot.com/parrots/hd/parrot.gif" alt="PARTEYYYYY" height="20"/>
    


# Release Notes - Versie 0.4.8.1 - 14-06-2024
0.4.8.1 - lang geleden zeg...

- algemeen
    - Nog meer compression van bestanden, dus snellere laadtijden
    
- Somtoday - Cijfers
    - Cijfers die een 5.5 zijn worden nu ook groen weergegeven (dus ook als ze afgerond een 5.5 zijn (> 5.45))
    
- Zermelo - Rooster
    - Een klein foutje dat sommige lessen niet goed werden weergegeven is opgelost (oepsie)


# Release Notes - Versie 0.4.8 - 17-04-2024
0.4.8 - Weer wat kleine updates

- Algemeen
    - Tussen alle andere coole nieuwe features door, heb ik ook nog een heel nieuw systeem ingevoerd: Zermos-Minimzer; alle code die wordt meegestuurd als je een pagina opvraagt wordt met gemiddeld 40% verkleind. Dit zorget voor een snellere laadtijd van de pagina's.
    - kleine inconsistenties in de app zijn opgelost.
    
- Zermelo
    - Je rooster op je smartwatch kan nu, als je per ongeluk de functie heb aangezet, ook de functie weer uitzetten.
    - Het rooster systeem heeft een update gehad, waardoor het rooster sneller laad, minder hapert én niet meer af en toe de verkeerde week toonde.
    - De manier waarop de rooster-weken worden opgehaald is aangepast, en niet meer gerouleerd. Dit zou moeten zorgen voor een stabieler rooster.
    - De roosters zijn nu constant op elke manier waarop je ze kan bekijken (behalve smartwatch)
    
- Somtoday - cijfers
    - Meer consistentie tussen de cijfers (op sommige plekken waren decimalen met een punt, op andere met een komma)
    - Cijfers die bestaan uit meerdere deel-toetsen worden nu ook weergegeven in de app.
    
- Informatiebord
    - Als er geen tekst bij de foto hoort, dan is de foto volledig zichtbaar.
    
- Koppelingen
    - Er was een probleem waarbij op magische wijze apps ontkoppeld werden. Dit is nu opgelost.    

# Release Notes - Versie 0.4.7 - 26-03-2024
Na een heel fiasco met de database, gekloot met Somtoday koppelingen en nog meer grijze haren kan je nu weer een koppeling leggen met Somtoday, nice.

0.4.7 - Somtoday is een t$@#ng@&*er

- Somtoday - koppelingen
    - Je kan nu weer een koppeling maken met Somtoday, nadat de &*%#&*$ een nieuwe inlog methode hebben gemaakt waardoor de oude koppeling niet meer werkte.
    
- Account - Instellingen
    - Nieuwe instellingen toegevoegd! Als je net zo blind bent als Joep, kan je nu de lettergrootte aanpassen (100, 125, 150, 175 en 200% normale grootte).

# BELANGRIJK - 23-03-2024
Zoals je misschien wel gezien hebt is Zermos offline, dit komt (indirect) door Somtoday. Somtoday heeft een nieuwe inlog methode en degene die ik nu gebruik om de app te koppelen werkt dus niet meer. En van het een komt het andere, uiteindelijk is de database geklapt en ben ik wat data kwijt, maar gelukkig heb ik bijna alles terug weten te halen (Helaas is zelf ingepland huiswerk wel weg). Dit weekend ga ik alles weer op z'n plek zetten en zorgen dat alles werkt (en hopelijk de nieuwe koppelmethode van Somtoday, maar ik kan het niet garanderen).

# Release Notes - Versie 0.4.6.1 - 26-02-2024
0.4.6.1 - foutjes van 0.4.6 opruimen
- Zermelo
    - Klein foutje met styling opgelost
    
- Somtoday
    - Probleem met de cijfer percentages opgelost

# Release Notes - Versie 0.4.6 - 25-02-2024
0.4.6 - bugfixes

- Somtoday - cijfers
    - cijfers delen errort nu niet meer.
    - Als je je specifieke cijfers bekijkt, dan zijn de cijfers die meetellen voor je examen nu zichtbaar gemaakt met een examen-hoedje.
    - SE cijfers zijn nu ook zichtbaar.
    - Stats zien er nu beter uit.
    
- algemeen
    - Zermos heeft nu een nieuw klein broertje: Zermos Watch. deze website is gemaakt voor kleine schermen zoals Galaxy Watches en Apple Watches. (je moet op het internet kunnen om deze te gebruiken)

# Release Notes - Versie 0.4.5 - 20-01-2024
0.4.5 - Alles is voor Bassie

Deze update is puur bugfixes enz. de code die op de achtergrond werkt is nu veel beter voor mij. Jullie zien niets anders dan een paar ms snellere laadtijden.

- Algemeen
    - elke keer als je de pagina refreshed of opnieuw laadt, worden je cookies geüpdate met de waarde van de database, om zo snel mogelijk alle data te hebben gesynchroniseerd.

- Zermelo - Rooster
    - De API service is nu veel beter geschreven en kan miner snel falen.
    - Zermelo laat nu nieuwe kleuren zien: rood (en ondoorzichtig) voor als 2 lessen tegelijk zijn en geel als je de keuze hebt om naar een les te gaan.
    
- Infowijs
    - Net zoals bij Zermelo is de API service opnieuw geschreven.
    
- Account
    - Als je nog nooit afwezig bent geweest (wow, hoe doe je dat?) dan laadt je account nu wel in, dit is een test die ik over het hoofd hebt gezien.
    
- Debug
    - de pagina is iets netter, en uitgebrijder. Maar boeie, je komt hier toch nooit.
    
- Somtoday - cijfers
    - Een probleem met de beschrijving van specifieke cijfers is nui opgelost.

# Release Notes - Versie 0.4.4.1 - 06-01-2024
0.4.4.1 - Hotfix for Somtoday

- Je kan nu wel inloggen als je een SOMtoday wachtwoord hebt met speciale tekens
- Een fout met week huiswerk is opgelost.

# Release Notes - Versie 0.4.4 - 28-12-2023
0.4.4 - Het oude jaar uit met een klapper

- Eerste keer
    - De eerste keer dat je een Zermos account aan maakt krijg je nu een kleine uitleg pagina te zien, zodat je weet wat je kan verwachten.

- authentificatie en autorisatie
    - het aanmaken van een account is nu opnieuw van de grond af aan opgebouwd en werkt nu wél zoals ik het wil.
    - het kon gebeuren dat je in 'account limbo' kwam, waarbij je geen account had, maar wel ingelogd was. Dit is nu opgelost, je wordt nu automatisch uitgelogd als je in de databse hebt.
    
- Hoofdmenu
    - Het hoofdmenu bestaat niet meer, het was echt te veel werk voor zo weinig functionaliteit, dus houdoe.
    
- Zermelo - Rooster
    - De paklijst in nu te vinden bij je rooster door op het 'lijst' icoontje te drukken.
    - Het swipen is nu veeeeel smoother
    
- Somtoday - cijferstatestieken
    - een volgorde probleem is nu opgelsot, nice.

- Infowijs - Schoolkalender
    - Het caching systeem dat ik heb gemaakt voor het Informatiebord is nu ook toegepast op de schoolkalender, dit scheelt erg wat laadtijd.

- School - Infomatiebord
    - het caching systeem is nu opnieuw opgebouwd, en werkt globaal en niet per gebruiker. Dit scheelt een hoop data en tijd.

# Release Notes - Versie 0.4.3 - 01-12-2023
Dit is een kleine update, maar wel een handige, ik wou het gewoon eventje snel uitbrengen!
0.4.3 - Sharing is caring

- Somtoday - Cijfers
    - Je kan nu je cijfers delen met je vrienden (en opa en oma natuurlijk) Dit doe je heel makkelijk. Ga naar je cijfers, klik op het deel icoontje en kies uit een paar opties (welke vakken, tot waaneer mogen ze het bekijken en of ze alleen het gemiddelde zien of ook de cijfers zelf)

# Release Notes - Versie 0.4.2 - 28-11-2023
0.4.2 - Zermelo strikes back

- Zermelo - Rooster
    - kleine visuele aanpassingen
    
- Account
    - Al je gedeelde links naar data zijn nu te zien op je account pagina.
    - Afwezigheid is in een popup te zien, geen nieuwe pagina die ingeladen hoeft te worden dus!
    
- Somtoday - cijfers
    - Er zijn nu geen cijfers meer met een +NaN% erachter, maar gewoon een 100%. Dit had te maken met de weging van het eerste cijfer van dat vak (dat was een 0).
    - Statestieken zijn een beetje gerefresh, minder bugs, meer betrouwbaarheid.

- Somtoday - Huiswerk
    - De knop 'bekijk huiswerk' werkt nu weer, ik had niet door dat ik hem kapoet had gemaakt :(
    
- Infowijs - schoolkalender
    - De schoolkalender scrollt nu weer automatisch naar de juiste maand, dit was kapot gegaan door een update van de site die ik niet had doorgevoerd, heel sad.
    
- Hoofdmenu
    - Ik heb de aftelklok voor de volgende les (hopelijk) gefixt, hij telt nu niet alleen maar naar je eerste les, maar de eerstvolgende les.

# Release Notes - Versie 0.4.1 - 21-11-2023
0.4.1 - Zermelo baby

- debug
    - Er is een nieuwe preview toegevoegd 'Zermelo sharing', hiermee kan j eje rooster delen, ik wil het systeem van dingen delen met vrienden via een link doorvoeren in meer dingen zoals cijfers. Maar omdat je voor nu de gedeelde links niet kan verwijderen blijft het eventjes in Beta

- notities
    - Ik heb het notities systeem weer verwijderd, het kostte teveel werk en ik was er niet blij mee, balen, maar het was toch pas in de beta versie.
    - Ook de Beta Microsoft koppeling is verwijderd, omdat het niet werkte. punt.
    
- Zermelo - Rooster
    - Ik heb de werking van het rooster verbeterd, het is nu sneller en beter (goh).
    - Het rooster werkt nu op 1 pagina, je krijgt dus niet elke keer een nieuw laadscherm.
    - De les details, werkt nu op de vertrouwde po-pup code, nice.
        - Ook is de datum nu makkelijk verwerkt met tijden en al.
    - Er zijn nu makkelijke knoppen om naar de volgende en vorige dag te gaan. op zowel mobiel als desktop.
    - Screenshots sturen van je rooster om te vergelijken? Nah, stuur je rooster door via een link die je kan delen! Dat is de Zermos manier.
    - De pagina waarop je Zermelo kan koppelen is nu opnieuw getypt, omdat Zermelo een nieuwe API heeft en je nu via de mobiele app kan koppelen.

# Release Notes - Versie 0.4 - 14-11-2023

0.4 - MICROSOFT BETA & nog iets meer content

- debug
    - Er is een nieuwe pagina waarop je verschillende dingen kunt refreshen, voor als Zermos niet optimaal werkt, maar dit hoef je eigenlijk nooit te gebruiken. Je komt hier door naar 'Account' te gaan en dan 3x op 'Profiel' te klikken

- Account
    - Hoevaak je afwezig bent geweest wordt nu elke dag ververst (mits je op de account pagina komt), eerst was dat alleen als je op de afwezigheid pagina kwam.

- algemeen
    - grote veranderingen in de code die mijn leven makkelijker maken :)
    - Verschillende UI verbeteringen
    - als je dingen moet invullen, dan krijg je nu netjes een popup met een tekstveld en uitleg, dit is een heel nieuw (uiteraad zelfgemaakt) systeem
    - Alle knoppen die buiten het navigatie menu vallen (en de accent kleur hebben) volgen nu allemaal dezelfde code en werken dus allemaal hetzelfde, NETJUHS.
    - Als je een systeem niet gekoppeld hebt, dan krijg je nu een veel netter pagina te zien met een uitleg en een link naar de originele pagina.
    - Een kleine cleanup voor de UI, niets bijzonders
    - Om te voorkomen dat in het notitie systeem een te grote hoeveelheid aan request wordt gestuurd zijn er rate limits toegevoegd, dit betekend dat je niet meer dan X request per X seconden kan sturen. Je zult hier geen last van hebben, het betekend dat je maximaal 3 keer per 10 seconden kan opslaan, dat is meer dan genoeg.
 
- Somtoday - custom huiswerk
    - het aanmaken van huiswerk gaat nu via de custom popup's, dit scheelt weer het laden van een nieuwe pagina en scheelt mij ongeveer 200% code
    
- Somtoday - huiswerk
    - Het filteren van huiswerk gaat nu ook via de custom popup's, stukken netter dit :)
    
- Hoofdmenu
    - De afteltijd gaat nu wel goed en loopt geen +1 of +2 uur achter
    
- Microsoft - Onedrive
    -Dit zit nu nog achter de 'previews', maar het begint te wekren.

# Release Notes - Versie 0.3.2 - 08-10-2023

0.3.2 - Een beetje dit, een beetje dat, maar nu nog net iets meer
- algemeen
    - Als je je thema veranderd, krijgt de balk aan de bovenkant nu ook de juiste kleur.
    - meedere kleine bugfixes die ik niet meer weet.

- hoofdmenu
    - De tijd tot de volgende les is nu weer synchroom met de Nederlandse tijdzone
    
- somtoday - huiswerk
    - Als je huiswerk sorteert, dan worden de lege dagen nu ook weggehaald.

- Zermelo - rooster
    - Het rooster dat in de wintertijd ligt loop nu geen uur meer voor.

# Release Notes - Versie 0.3.1 - 04-10-2023

0.3.1 - Een beetje dit, een beetje dat, maar nu nog net iets meer

- Hoofdmenu
    - Uitgevallen lessen staan nu niet meer in de paklijst
    - 'Volgende les' laat uitgevallen lessen nu ook neit zien

- account - instellingen
    - de 'handig' instelling laat nu ook zien welke je hebt gekozen
    
- Infowijs - Schoolkalender
    - Datum's zijn weer gefixt (hoop ik, het is echt heel vervelend om mee te werken)

# Release Notes - Versie 0.3 - 04-10-2023

0.3 - Een beetje dit, een beetje dat

- algemeen
    - De 'volgende' en 'vorige' knoppen bovenaan de (pc) pagina werken nu met de custom 'history api'
    - Alle pagina's volgen nu een en dezelfde regels voor de layout
    - Alle nieuwe gebruikers (/de mensen die de notificatie nog niet ontvangen hebben) ontvangen nu een melding dat ze de docs moeten bekijken wat Zermos allemaal kan.
    - De 'notificatie' UI is nu iets makkelijker te bekijken
    - Laptop gebruikers zien nu een leesbare tekst
    - het opsporen van bugs is nu iets makkelijker geworden door backend logging (boeit je niks, maar ik ben er blij mee)
    - Het zelfgemaakt 'history' systeem werkt nu ook voor computer gebruikers (volgende en terug knoppen, mogelijk knoppen op je muis) en voor mobiele gebruikers (terug knop op je telefoon)
    - Je instellingen worden nu op een nieuwe manier opgeslagen, dus het kan zijn dat je instellingen nu weg zijn, sorry daarvoor. Maar nu kan ik 100x makkelijker instellingen toevoegen.

- Account
    - Er is nu een uitlog knop... Dat duurde even...
    - De 'opgeslagen informatie' zit nu achter een knop, dan is je scherm niet gelijk vol :)
    - Da's pech, instellingen weg. De instellingen zijn naar een eigen pagina verplaats, je bereikt het via 'Account' -> 'Instellingen' Ook krijgt het nu zijn eigen bullet point in de release notes.
    - CSS cleanup, hoe de pagina er uitziet is het zelfde, maar de code is nu iets netter.
 
- Instellingen
    - De Instellingen pagina heeft zijn eigen 'settings framework' gekregen zodat de pagina niet herladen hoefte worden als je iets veranderd.
    - Links handige niet getreurd, je kan nu de UI omdraaien zodat je de knoppen aan de linkerkant hebt.
    
- A wild Hoofdmenu appeard 😲
    - Op het nieuwe hoofdmenu kan je de volgende dingen:
        - Bekijk je volgende les (wie de docent is, waar het is, welk vak en hoelang nog tot de les begint)
        - Bekijk je laatst binnen gekomen cijfer
        - Je tas makkelijker in pakken met de handige afvinklijst
        - het laatste schoolnieuws
        - 'DOOR DE WIND, DOOR DE REGEN 🎵' Je ziet nu het komende weer boven de school.
        - Bekijk het huiswerk voor morgen (want dat van vandaag heb je al gemaakt, toch?)

- Koppelingen
    - De UI van de koppelschermen is sterk verbeterd en is nu overal hetzelfde.
    - Zermelo koppeling met je inloggegevens is nu weer beschikbaar.
        
- Infowijs - Schoolkalender
    - Bepaalde data waren met 1 dag opgeschoven, dat is nu opgelost.
    
- Somtoday - Huiswerk
    - code waarmee ik de huiswerk pagina getest heb is nu verwijderd
    - Ook op mobiel kan je nu huiswerk sorteren, nice!
    
- School - Schoolklimaat
    - Backend veranderingen zijn doorgevoerd, je ziet nu alle meters, ook als ze niet online zijn. Je kan ook zoeken op naam (?locatie=[bijv. a2]) en online status (/schoolklimaat?isOnline=[0 = offline, 1 = online, 2 = alles])

# Release Notes - Versie 0.2.4 - 15-09-2023

0.2.4 - UI fixes voor de vorige updates, geen nieuwe functionaliteiten

- Account
    - De item selector op je laptop is nu weg als je op mobiel bent (je scherm minder dan 1000px breed is)
    
- Somtoday - Huiswerk
    - Deze pagina laadt nu wel weer in (foutje met custom huiswerk)

- Algemeen
    - De css (opmaak) van de website is nu beter georganiseerd, geoptimaliseerd en opgeschoond. Ook werkt de website nu beter op mobiel. (Dit is 90% van deze hele update en je ziet er heel weinig van :( )
    
- Zermelo - Rooster
    - Door de css aanpassingen is de rooster pagina nu ook beter op mobiel, blur werkt nu ook op safari en de hoogte het rooster (plus de les elementen) is nu dynamisch en stop niet meer na een hoogte van 600px.

# Release Notes - Versie 0.2.3 - 10-09-2023

0.2.3 - uhmmmm

- algemeen
    - er wordt nu gewacht tot de css ingeladen is voordat de pagina wordt getoond, dit voorkomt dat de pagina eerst de ruwe tekst toont en daarna pas de opmaak
    - De knop naar het hoofdmenu is tijdelijk verwijderd, ik ben totaal niet blij met hoe de pagina er uit ziet en werkt, dus daar ga ik nog aan werken. Voor nu blijft hij nog wel beschikbaar via https://zermos.kronk.tech/Hoofdmenu, maar niet via de knop
    - Je kan nu aangeven welke pagina je wilt zien als je Zermos bezoekt (https://zermos.kronk.tech/), als je Zermos bezoekt op bijv. https://zermos.kronk.tech/rooster, dan wordt je automatisch nogsteeds doorgestuurd naar het rooster. Stel het snel in op de account pagina
    
- Somtoday - Afwezigheid
    - Je afwezigheid's data wordt nu getoond in het nederlands, dus niet meer august, maar augustus
    
- Somtoday - Huiswerk
    - Je wordt nu automatisch doorgestuurd naar de juiste dag, dan hoef je niet zoveel te scrollen
    - Als je via het rooster naar je huiswerk wordt gestuurd (door huiswerk op een les), dan gaat het huiswerk dat er bij hoort gloeien
    - Je kan nu je huiswerk sorteren op vak, en type (huiswerk, toets en grote toest). Als je bijvoorbeeld alle je wiskunde huiswerk verbergd, dan komt er een klein meldinkje te zien zodat je weet dat er meer huiswerk is, maar dat het verborgen is.
    - 'dag' en 'week' huiswerk wordt nu ook getoont, dit herken je aan de licht-groene rechter rand.

- Zermelo 
    - Als je op een vak klikt, dan krijg je nu een popup met de vak, docent(en), lokaal(en), begintijd, eindtijd, groep(en) en de statusen van de les
    - Als je op de knop 'bekijk huiswerk' klikt, dan krijg je het huiswerk  dat op die les gepland staat te zien, anders krijg je een melding dat er geen huiswerk is

- School - Schoolklimaat (!?)
    - Ik ben bezig met een nieuwe functie: 'Schoolklimaat'. Dit is een pagina waarop je de temperaturen etc. van alle lokalen in een keer kan zien. Je kan een bekijken wat ik al heb op https://zermos.kronk.tech/schoolklimaat (P.S. het is niet veel)
    
- Backend
    - De oude authenticatie manier is nu volledig verwijderd, je kan nu alleen nog maar inloggen met je Microsoft account

# Release Notes - Versie 0.2.2 - 29-08-2023

0.2.2 meer dingen

- Somtoday - cijfers
    - De overgang naar specifieke cijfer data is nu ook met de nieuwe laadmanier, die was ik vorige update vergeten over te zetten
    - De grafieken laden weer goed in
    - De grafieken laten nu ook goed de cijfers van 'examens' (CKV, Maat, etc.) zien en daardoor krijg je nu ook gemiddeldes van die cijfers i.p.v. NaN (Not a Number)
    
- Infowijs - Schoolnieuws
    - Mogelijk bijgevoegde foto's worden nu ook weergegeven
    
- Somtoday (/ account)
    - Stond de brug weer open? Natuurlijk. Je mislukte smoesjes staan nu ook op Zermos, ga naar je account en klik op 'afwezigheid' om ze te bekijken.
    
- Somtoday - cijfers
    - De cijfers worden nu op een andere manier opgevraagd op Zermos, hierdoor laden de cijfers iets langzamer in, maar dit is veiliger voor jouw data. Ook is het makkelijker om een cijfer te vinden aangezien het heel handig is gesorteerd op vak en cijfer id.
<br>`/Somtoday/Cijfers` > alle vakken
<br>`/Somtoday/Cijfers/Ckv` > alle cijfers van CKV
<br>`/Somtoday/Cijfers/Ckv/Statestieken` > alle cijfers van CKV met statistieken
<br>`/Somtoday/Cijfers/Ckv/12345678901234` > het cijfer van CKV met (Somtoday) id 12345678901234

- Somtoday - Huiswerk
    - Een visueel probleem is opgelost, de huiswerk items zijn nu weer even breed als de rest van de pagina
    
- Somtoday - custom huiswerk
    - Je custom huiswerk is nu ook verwijderbaar, klik 2 keer op het prullenbakje om het huiswerk te verwijderen
    
- School - informatiebord
    - het informatiebord is nu ook te zien zonder dat je ingelogd bent
    
- Koppelingen
    - Alle koppeling methodes zijn nu ook overgeschakeld naar de nieuwe UI

# Release Notes - Versie 0.2.1 - 20-08-2023

0.2.1 UX update

In deze update heb ik me vooral gericht op de manier waarop je omgaat met de website. Hierdoor heb ik verschillende dingen aangepast, zoals:

- Algemeen
    - De term 'routing' (die ik hieronder zal verduidelijken) verwijst naar het proces van het navigeren tussen verschillende pagina's op een website. Een website bestaat uit verschillende URL's, zoals bijvoorbeeld https://zermos.kronk.tech/Zermelo/Rooster en https://zermos.kronk.tech/Somtoday/Cijfers. In het begin verliep dit proces soepel, maar er was een terugkerend probleem: telkens wanneer je naar een andere pagina ging, werden alle extra scripts, afbeeldingen, iconen, en andere elementen opnieuw gedownload, hoewel dit eigenlijk niet noodzakelijk was.<br><br>Om dit aan te pakken, heb ik een oplossing geïmplementeerd. Wanneer je bijvoorbeeld de URL 'https://zermos.kronk.tech/Zermelo/Rooster' bezoekt, word je nu doorgestuurd naar 'https://zermos.kronk.tech?url=/Zermelo/Rooster'. Bij deze aangepaste URL wordt eerst de basisset van de website gedownload, inclusief de zijbalk, iconen en andere componenten. Hierdoor ontstaat een leeg gebied in het midden van de pagina. Vervolgens wordt de specifieke opgevraagde pagina (die wordt aangegeven in de '?url=/Zermelo/Rooster' parameter) gedownload en in dit lege gebied geplaatst. Dit hele proces gebeurt achter de schermen en wordt netjes gepresenteerd met een laadscherm, waardoor het lijkt alsof er geen verandering heeft plaatsgevonden.<br><br>Deze aanpak heeft ook een extra voordeel: de website hoeft veel minder gegevens te downloaden, omdat je op je telefoon feitelijk op dezelfde pagina blijft en de ervaring dus 'normaal' aanvoelt. In het verleden werd bij elke paginawissel ongeveer 811 KB (ongeveer 81 e-mails) aan gegevens gedownload. Dit gebeurde telkens bij elke laadscherm en elke andere pagina. Nu wordt er slechts 1 keer ongeveer 811 KB gedownload bij het opstarten, gevolgd door slechts 2 KB per pagina. Dit is een aanzienlijke verbetering. Bovendien worden de grootste bestanden gecachet (opgeslagen in het geheugen van je mobiel/PC), waardoor deze niet opnieuw gedownload hoeven te worden. Dit resulteert in slechts ongeveer 20 KB aan gegevens die worden gedownload wanneer je Zermos opstart.
    - Het laadscherm is ook veel soepeler :)
    - Het probleem met de 404 pagina is verholpen (de 404 pagina stond niet op de goede plek, de opgevraagde pagina bestond dus niet en werd je doorgestuurd naar de 404 pagina, enz. enz.).
    - Leuke laadscherm berichten toegevoegd, want school wifi is traaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa....
    - Er is een Update(); method toegevoegd, maar dit is niet echt belangrijk om te weten.
    
- Zermelo - Rooster
    - Er is nu een klein bolletje dat aangeeft of je ver genoeg hebt geswiped om naar de volgende/vorige week te gaan.
    - Een veelvoorkomende error is opgelost, dat is altijd fijn ;)
    - De week verandere via 'swipen' werkt nu weer zoals het hoort.
    
- Hoofdmenu
    - de weer 'card' geeft nu een soepele lijn in plaats van een balk van onder.
    
- Infowijs/Account
    - de rechter zijbalk met 'belangrijke links' is nu weer werkend, en laat de kleuren weer zien.

# Release Notes - Versie 0.2.0 - 14-08-2023

0.2 - The small update...

* Notitie, vanaf nu zijn er 2 versies van Zermos web: 'Zermos web' een de 'PWA', ze verschillen niet veel van elkaar, dus wat voor het een geldt, geldt ook voor het ander. Als je vragen hebt over beide, kijk dan bij de docs.

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
