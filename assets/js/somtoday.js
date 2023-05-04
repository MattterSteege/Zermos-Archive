console.log("SOMtoday.js loaded successfully");

function authenticateUser(username, password) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:44333/SOMtoday/authenticate?username=${username}&password=${password}`;
    xhr.open("GET", url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function() {
        if (this.readyState === 4) {
            const response = JSON.parse(this.responseText);
            const model = {
                access_token: response.access_token,
                refresh_token: response.refresh_token,
                somtoday_api_url: response.somtoday_api_url,
                somtoday_oop_url: response.somtoday_oop_url,
                scope: response.scope,
                somtoday_organisatie_afkorting: response.somtoday_organisatie_afkorting,
                id_token: response.id_token,
                token_type: response.token_type,
                expires_in: response.expires_in,
            }

            localStorage.setItem("somtoday-access_token", model.access_token);
            localStorage.setItem("somtoday-refresh_token", model.refresh_token);

            getStudent();

            window.location.href = "/Zermos/somtoday/";
        }
    }

    xhr.send();
}

function CheckIfSOmTodayTokenIsExpired() {
    //decode the token with base64
    const token = localStorage.getItem("somtoday-access_token");
    const decodedToken = atob(token.split('.')[1]);
    const parsedToken = JSON.parse(decodedToken);
    const expirationDate = parsedToken.exp;
    //if the token is expired, refresh it
    if (expirationDate < Date.now()) {
        const refreshToken = localStorage.getItem("somtoday-refresh_token");
        const xhr = new XMLHttpRequest();
        const url = `https://localhost:44333/SOMtoday/refresh?refreshToken=${refreshToken}`;
        xhr.open("GET", url, true);
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onreadystatechange = function() {
            if (this.readyState === 4) {
                const response = JSON.parse(this.responseText);
                const model = {
                    access_token: response.access_token,
                    refresh_token: response.refresh_token,
                    somtoday_api_url: response.somtoday_api_url,
                    somtoday_oop_url: response.somtoday_oop_url,
                    scope: response.scope,
                    somtoday_organisatie_afkorting: response.somtoday_organisatie_afkorting,
                    id_token: response.id_token,
                    token_type: response.token_type,
                    expires_in: response.expires_in,
                }

                localStorage.setItem("somtoday-access_token", model.access_token);
                localStorage.setItem("somtoday-refresh_token", model.refresh_token);
            }

            getStudent();
        }
    }
}

function getGrades() {
//XMLHttpRequest https://localhost:44333/SOMtoday/grades?token=${token}&studentId=${studentId}&begintNaOfOp=${begintNaOfOp}
    CheckIfSOmTodayTokenIsExpired();

    const token = localStorage.getItem("somtoday-access_token");
    const studentId = localStorage.getItem("somtoday-student_id");
    const begintNaOfOp = "2022-01-01";

    if (token === null) {
        //console.log("No access token or student id found, redirecting to login page...");
        window.location.href = "/Zermos/somtoday/inloggen/";
    }

    var xhr = new XMLHttpRequest();
    xhr.open("GET", `https://localhost:44333/SOMtoday/grades?token=${token}&studentId=${studentId}&begintNaOfOp=${begintNaOfOp}`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function() {
        if (this.readyState === 4) {
            const response = JSON.parse(this.responseText);
            const model = response.items.map(item => ({
                type: item.type,
                links: item.links.map(link => ({
                    id: link.id,
                    rel: link.rel,
                    type: link.type,
                    href: link.href
                })),
                permissions: item.permissions.map(permission => ({
                    full: permission.full,
                    type: permission.type,
                    operations: permission.operations,
                    instances: permission.instances
                })),
                herkansingstype: item.herkansingstype,
                resultaat: item.resultaat,
                geldendResultaat: item.geldendResultaat,
                datumInvoer: item.datumInvoer,
                teltNietmee: item.teltNietmee,
                toetsNietGemaakt: item.toetsNietGemaakt,
                leerjaar: item.leerjaar,
                periode: item.periode,
                weging: item.weging,
                examenWeging: item.examenWeging,
                isExamendossierResultaat: item.isExamendossierResultaat,
                isVoortgangsdossierResultaat: item.isVoortgangsdossierResultaat,
                vak: {
                    links: item.vak.links.map(link => ({
                        id: link.id,
                        rel: link.rel,
                        type: link.type,
                        href: link.href
                    })),
                    permissions: item.vak.permissions.map(permission => ({
                        full: permission.full,
                        type: permission.type,
                        operations: permission.operations,
                        instances: permission.instances
                    })),
                    additionalObjects: item.vak.additionalObjects,
                    afkorting: item.vak.afkorting,
                    naam: item.vak.naam
                },
                vrijstelling: item.vrijstelling,
                omschrijving: item.omschrijving,
            }));

            // count all grades with the same vak.naam and calculate the average, then display it in the table like this:
            // <div className="grade-child">
            //     <div className="left">
            //         <h2 className="subject">Wiskunde</h2>
            //     </div>
            //     <div className="weight">
            //         5x
            //     </div>
            //     <div className="right passed">
            //         6.7
            //     </div>
            // </div>
            // parent id = grade-container

            model.sort((a, b) => a.vak.naam.localeCompare(b.vak.naam));

            //create a list of all grades with the same vak.naam
            let ListOfGradesWithSameVakNaam = [];
            let currentVak = "";
            let currentVakWeight = 0;
            let currentVakGradeTotal = 0;
            let currentVakId = 0;

            model.forEach(grade => {
                if (grade.type === "DeeltoetsKolom"){
                    return;
                }

                if (grade.vak.naam === currentVak) {
                    currentVak = grade.vak.naam;
                    currentVakWeight += Number(grade.weging) === 0 ? Number(grade.examenWeging) : Number(grade.weging);
                    currentVakGradeTotal += (Number(grade.geldendResultaat.replace(",", "")) / 10) * (Number(grade.weging) === 0 ? Number(grade.examenWeging) : Number(grade.weging));
                    currentVakId = Number(grade.vak.links[0].id);
                } else {
                    if (currentVak !== "") {
                        ListOfGradesWithSameVakNaam.push({
                            vak: currentVak,
                            weight: currentVakWeight,
                            grade: Math.round((currentVakGradeTotal / currentVakWeight) * 10) / 10,
                            id: Number(currentVakId)
                        });
                    }

                    currentVak = grade.vak.naam;
                    currentVakWeight = Number(grade.weging) === 0 ? Number(grade.examenWeging) : Number(grade.weging);
                    currentVakGradeTotal = (Number(grade.geldendResultaat.replace(",", "")) / 10) * (Number(grade.weging) === 0 ? Number(grade.examenWeging) : Number(grade.weging));
                    currentVakId = Number(grade.vak.links[0].id);
                }
            });

            //add the last vak to the list
            ListOfGradesWithSameVakNaam.push({
                vak: currentVak,
                weight: currentVakWeight,
                grade: Math.round((currentVakGradeTotal / currentVakWeight) * 10) / 10,
                id: Number(currentVakId)
            });


            //console.log(ListOfGradesWithSameVakNaam);



            //loop through the list and log the values
            //create the html elements
            // <div className="grade-child">
            //     <div className="left">
            //         <h2 className="subject">Wiskunde</h2>
            //     </div>
            //     <div className="weight">
            //         5x
            //     </div>
            //     <div className="right passed">
            //         6.7
            //     </div>
            // </div>
            // parent id = grade-container

            ListOfGradesWithSameVakNaam.forEach(grade => {
                //console.log(grade);
                const gradeContainer = document.getElementById("grade-container");

                const gradeChild = document.createElement("div");
                gradeChild.className = "grade-child";

                const left = document.createElement("div");
                left.className = "left";

                const subject = document.createElement("h2");
                subject.className = "subject";
                subject.innerText = grade.vak;
                left.appendChild(subject);

                const weight = document.createElement("div");
                weight.className = "weight";
                weight.innerText = `${grade.weight}x`;

                const right = document.createElement("div");
                right.className = "right";
                right.innerText = grade.grade;
                if (grade.grade >= 5.5 || isNaN(grade.grade)) {
                    right.classList.add("passed");
                }
                else {
                    right.classList.add("failed");
                }

                gradeChild.appendChild(left);
                gradeChild.appendChild(weight);
                gradeChild.appendChild(right);

                gradeContainer.appendChild(gradeChild);
                /*on click gradeChild*/
                gradeChild.addEventListener("click", () => {
                    window.location.href = "/Zermos/somtoday/cijfers/vak/?" + grade.id;
                });
            });
        }
    }

    xhr.send();
}


function getGradesById(vakId) {
    CheckIfSOmTodayTokenIsExpired();
    const token = localStorage.getItem("somtoday-access_token");
    const studentId = localStorage.getItem("somtoday-student_id");
    const begintNaOfOp = "2022-01-01";

    if (token === null) {
        //console.log("No access token or student id found, redirecting to login page...");
        window.location.href = "/Zermos/somtoday/inloggen/";
    }

    var xhr = new XMLHttpRequest();
    xhr.open("GET", `https://localhost:44333/SOMtoday/gradesById?token=${token}&studentId=${studentId}&begintNaOfOp=${begintNaOfOp}&vakId=${vakId}`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (this.readyState === 4) {
            const response = JSON.parse(this.responseText);
            const model = response.items.map(item => ({
                type: item.type,
                links: item.links.map(link => ({
                    id: link.id,
                    rel: link.rel,
                    type: link.type,
                    href: link.href
                })),
                permissions: item.permissions.map(permission => ({
                    full: permission.full,
                    type: permission.type,
                    operations: permission.operations,
                    instances: permission.instances
                })),
                herkansingstype: item.herkansingstype,
                resultaat: item.resultaat,
                geldendResultaat: item.geldendResultaat,
                datumInvoer: item.datumInvoer,
                teltNietmee: item.teltNietmee,
                toetsNietGemaakt: item.toetsNietGemaakt,
                leerjaar: item.leerjaar,
                periode: item.periode,
                weging: item.weging,
                examenWeging: item.examenWeging,
                isExamendossierResultaat: item.isExamendossierResultaat,
                isVoortgangsdossierResultaat: item.isVoortgangsdossierResultaat,
                vak: {
                    links: item.vak.links.map(link => ({
                        id: link.id,
                        rel: link.rel,
                        type: link.type,
                        href: link.href
                    })),
                    permissions: item.vak.permissions.map(permission => ({
                        full: permission.full,
                        type: permission.type,
                        operations: permission.operations,
                        instances: permission.instances
                    })),
                    additionalObjects: item.vak.additionalObjects,
                    afkorting: item.vak.afkorting,
                    naam: item.vak.naam
                },
                vrijstelling: item.vrijstelling,
                omschrijving: item.omschrijving,
            }));


            model.forEach(grade => {
                //console.log(grade);
                const gradeContainer = document.getElementById("grade-container");

                const gradeChild = document.createElement("div");
                gradeChild.className = "grade-child";

                const left = document.createElement("div");
                left.className = "left";

                const subject = document.createElement("p");
                subject.className = "description";
                subject.innerText = grade.omschrijving;
                left.appendChild(subject);

                const weight = document.createElement("div");
                weight.className = "weight";
                weight.innerText = `${Number(grade.weging) === 0 ? Number(grade.examenWeging) : Number(grade.weging)}x`;

                const right = document.createElement("div");
                right.className = "right";
                right.innerText = grade.geldendResultaat;
                if (grade.geldendResultaat >= 5.5 || isNaN(grade.geldendResultaat)) {
                    right.classList.add("passed");
                }
                else {
                    right.classList.add("failed");
                }

                gradeChild.appendChild(left);
                gradeChild.appendChild(weight);
                gradeChild.appendChild(right);

                gradeContainer.appendChild(gradeChild);
            });
        }
    }

    xhr.send();
}

function getStudent() {
//XMLHttpRequest https://localhost:44333/SOMtoday/student?token=${token}
    var xhr = new XMLHttpRequest();
    xhr.open("GET", `https://localhost:44333/SOMtoday/student?token=${localStorage.getItem("somtoday-access_token")}`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function() {
        if (this.readyState === 4) {
            const response = JSON.parse(this.responseText);

            const model = response.items.map(item => ({
                links: item.links,
                permissions: item.permissions,
                UUID: item.UUID,
                leerlingnummer: item.leerlingnummer,
                roepnaam: item.roepnaam,
                voorvoegsel: item.voorvoegsel,
                achternaam: item.achternaam,
                email: item.email,
                geboortedatum: item.geboortedatum,
                geslacht: item.geslacht,
                additionalObjects: item.additionalObjects
            }));

            localStorage.setItem("somtoday-student_id", model[0].links[0].id);
        }
    }

    xhr.send();
}

function getHomework(){
    CheckIfSOmTodayTokenIsExpired();
    //GET: https://localhost:44333/SOMtoday/huiswerk?begintNaOfOp=2022-01-10&token=
    CheckIfSOmTodayTokenIsExpired();
    //const begintNaOfOp = current date yyyy-mm-dd minus 14 days
    const begintNaOfOp = new Date();
    begintNaOfOp.setDate(begintNaOfOp.getDate() - 14);
    const begintNaOfOpString = `${begintNaOfOp.getFullYear()}-${begintNaOfOp.getMonth() + 1}-${begintNaOfOp.getDate()}`;

    var xhr = new XMLHttpRequest();
    xhr.open("GET", `https://localhost:44333/SOMtoday/huiswerk?begintNaOfOp=${begintNaOfOpString}&token=${localStorage.getItem("somtoday-access_token")}`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function() {
        if (this.readyState === 4){
            //console.log(this.responseText);
            const response = JSON.parse(this.responseText);
            //console.log(response);
            const model = response.items.map(item => ({
                aangemaaktOpDatumTijd: item.aangemaaktOpDatumTijd,
                additionalObjects: {
                    huiswerkgemaakt: item.additionalObjects.huiswerkgemaakt,
                    //leerlingen: item.additionalObjects.leerlingen,
                    swigemaaktVinkjes: item.additionalObjects.swigemaaktVinkjes
                },
                datumTijd: item.datumTijd,
                lesgroep: {
                    naam: item.lesgroep.naam,
                    schooljaar: {
                        naam: item.lesgroep.schooljaar.naam,
                        totDatum: item.lesgroep.schooljaar.totDatum,
                        vanafDatum: item.lesgroep.schooljaar.vanafDatum
                    },
                    vak: {
                        afkorting: item.lesgroep.vak.afkorting,
                        naam: item.lesgroep.vak.naam
                    },
                    vestiging: {
                        naam: item.lesgroep.vestiging.naam
                    },
                },
                sortering: item.sortering,
                studiewijzerItem: {
                    bijlagen: item.studiewijzerItem?.bijlagen ?? [],
                    huiswerkType: item.studiewijzerItem?.huiswerkType ?? "",
                    omschrijving: item.studiewijzerItem?.omschrijving ?? "",
                    onderwerp: item.studiewijzerItem?.onderwerp ?? "",
                    tonen: item.studiewijzerItem?.tonen ?? true
                }
            }));

            //return;


            model.sort((a, b) => {
                return new Date(a.datumTijd) - new Date(b.datumTijd);
            });


            let currentDate = "";
            let currentHomeworkContainer = null;
            model.forEach(item => {
                const date = item.datumTijd.split("T")[0];
                if (date !== currentDate) {
                    const homeworkContainer = document.getElementById("homework-container");
                    const homeworkDateDivider = document.createElement("div");
                    homeworkDateDivider.className = "homework-date-divider";

                    const dateObject = new Date(date);
                    const day = dateObject.toLocaleString("nl-NL", {weekday: "long"});
                    const dayNumber = dateObject.toLocaleString("nl-NL", {day: "numeric"});
                    const month = dateObject.toLocaleString("nl-NL", {month: "long"});
                    homeworkDateDivider.innerText = `${day} ${dayNumber} ${month}`;

                    const homeworkContainerForCurrentHomework = document.createElement("div");
                    homeworkContainerForCurrentHomework.className = "row";

                    homeworkContainer.appendChild(homeworkDateDivider);
                    homeworkContainer.appendChild(homeworkContainerForCurrentHomework);
                    currentDate = date;
                    currentHomeworkContainer = homeworkContainerForCurrentHomework;
                }

                /*
                      <div class="homework-child">
                        <div class="left">
                          <h1 class="subject">[vak.naam]</h1>
                          <p class="description">omschrijving</p>
                        </div>
                        <div class="right passed">

                        </div>
                      </div>
                */

                const homeworkChild = document.createElement("div");
                homeworkChild.className = "homework-child";

                const left = document.createElement("div");
                left.className = "left";

                const subject = document.createElement("h1");
                subject.className = "subject";
                subject.innerText = item.lesgroep.vak.naam;

                const description = document.createElement("p");
                description.className = "description";
                description.innerText = removeHTMLTags(item.studiewijzerItem.omschrijving === "" ? item.studiewijzerItem.onderwerp : item.studiewijzerItem.omschrijving);

                left.appendChild(subject);
                left.appendChild(description);



                const right = document.createElement("div");
                right.className = "right";

                homeworkChild.appendChild(left);
                homeworkChild.appendChild(right);

                currentHomeworkContainer.appendChild(homeworkChild);
            });

        }
    }

    xhr.send();
}

function removeHTMLTags(text) {
    return text.replace(/<[^>]*>/g, '');
}
