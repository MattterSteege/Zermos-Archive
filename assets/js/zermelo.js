console.log("zermelo.js loaded successfully");


/*dagrooster  \/   */
function getDagrooster() {
  //console.log("Getting dagrooster...");
  let ajaxRequest = new XMLHttpRequest();
  const access_token = localStorage.getItem("zermelo-access_token");
  const student = localStorage.getItem("zermelo-student_id");

  if (access_token === null || student === null) {
    //console.log("No access token or student id found, redirecting to login page...");
    window.location.href = "/Zermos/zermelo/inloggen/";
  }

  let currentDate = new Date();
  let startDate = new Date(currentDate.getFullYear(), 0, 1);
  let days = Math.floor((currentDate - startDate) / (24 * 60 * 60 * 1000));
  const weekNumber = 16; //Math.ceil(days / 7);
  console.warn("FIXED WEEK NUMBER TO 16 (DUE TO VACATION)\n\nzermelo.js:L19: const weekNumber = 16; //Math.ceil(days / 7);");

  ajaxRequest.open(
      "GET",
      "https://ccg.zportal.nl/api/v3/liveschedule?access_token=" +
      access_token +
      "&student=" +
      student +
      "&week=2023" +
      weekNumber,
      true
  );
  ajaxRequest.send();

  ajaxRequest.onreadystatechange = function() {
    if (ajaxRequest.readyState === 4) {
      //navigator.clipboard.writeText(ajaxRequest.responseText);
      const data = JSON.parse(ajaxRequest.responseText);
      //console.log(data);

      const model = {
        response: {
          status: data.response.status,
          message: data.response.message,
          details: data.response.details,
          eventId: data.response.eventId,
          startRow: data.response.startRow,
          endRow: data.response.endRow,
          totalRows: data.response.totalRows,
          data: data.response.data.map((item) => ({
            laatsteWijziging: item.laatsteWijziging,
            appointments: item.appointments.map((appointment) => ({
              status: appointment.status.map((status) => ({
                code: status.code,
                nl: status.nl,
                en: status.en,
              })),
              actions: appointment.actions,
              start: appointment.start,
              end: appointment.end,
              cancelled: appointment.cancelled,
              appointmentType: appointment.appointmentType,
              online: appointment.online,
              optional: appointment.optional,
              appointmentInstance: appointment.appointmentInstance,
              startTimeSlotName: appointment.startTimeSlotName,
              endTimeSlotName: appointment.endTimeSlotName,
              subjects: appointment.subjects,
              groups: appointment.groups,
              locations: appointment.locations,
              teachers: appointment.teachers,
              onlineTeachers: appointment.onlineTeachers,
              onlineLocationUrl: appointment.onlineLocationUrl,
              capacity: appointment.capacity,
              expectedStudentCount: appointment.expectedStudentCount,
              expectedStudentCountOnline: appointment.expectedStudentCountOnline,
              changeDescription: appointment.changeDescription,
              schedulerRemark: appointment.schedulerRemark,
              content: appointment.content,
              id: appointment.id,
            })),
          })),
        },
      };

      //console.log(model);

      let appointmentsByDay = {
        1: [], // Monday
        2: [], // Tuesday
        3: [], // Wednesday
        4: [], // Thursday
        5: [], // Friday
      };

      for (let i = 0; i < model.response.data[0].appointments.length; i++) {
        let appointment = model.response.data[0].appointments[i];
        const date = new Date(appointment.start * 1000);
        // Get the day of the week (0 = Sunday, 1 = Monday, etc.) and add the appointment to the corresponding array in appointmentsByDay
        const dayOfWeek = date.getUTCDay();
        appointmentsByDay[dayOfWeek].push(appointment);
      }

      // Loop through each appointment day and create a new grid column for each one
      for (let i = 1; i <= 5; i++) {
        const appointmentDay = appointmentsByDay[i];
        const gridColumn = document.createElement("div");
        gridColumn.classList.add("grid-column");

        let lessonNumber = 1;

        // Loop through the appointments in the current day and create a new grid item for each one
        for (let i = 0; i < appointmentDay.length; i++) {
          if (appointmentDay[i].startTimeSlotName === lessonNumber.toString()) {
            const gridItem = document.createElement("div");
            gridItem.classList.add("grid-item");

            if (window.innerWidth < 753) {
              gridItem.innerHTML =
                  (appointmentDay[i].subjects.length > 0 ?
                      appointmentDay[i].subjects[0] :
                      "");

              /*add <br>*/
                gridItem.innerHTML += "<br>";


              gridItem.innerHTML +=
                  (appointmentDay[i].locations[0] ?
                      appointmentDay[i].locations[0] :
                      "");
            }
            else {
              gridItem.textContent =
                  (appointmentDay[i].subjects.length > 0 ?
                      appointmentDay[i].subjects[0] :
                      "") +
                  (appointmentDay[i].locations[0] ?
                      " - " + appointmentDay[i].locations[0] :
                      "") +
                  (appointmentDay[i].teachers[0] ?
                      " - " + appointmentDay[i].teachers.join(", ") :
                      "");

              //log subject[0] when the mouse hovers over the grid item
              gridItem.addEventListener("mouseover", function () {
                if (appointmentDay[i].subjects[0] !== undefined) {
                  getLessonFromParameter(appointmentDay[i]);
                } else {

                }
              });
            }

            if (appointmentDay[i].cancelled === true && gridItem.textContent !== "") {
              gridItem.classList.add("cancelled");
            } else if (gridItem.textContent !== "") {
              gridItem.href = "";
            }

            gridColumn.appendChild(gridItem);
          } else {
            i--;
          }

          lessonNumber++;
        }

        // Add empty grid items for any remaining slots
        for (let i = 0; i < 9 - appointmentDay.length; i++) {
          const gridItem = document.createElement("div");
          gridItem.classList.add("grid-item");

          gridColumn.appendChild(gridItem);
        }

        //add the column to grid-container
        document.getElementById("grid-container").appendChild(gridColumn);
      }
    }
  };
}

getWeek = function() {
  let date = new Date();
  date.setHours(0, 0, 0, 0);
  date.setDate(date.getDate() + 3 - ((date.getDay() + 6) % 7));
  const week1 = new Date(date.getFullYear(), 0, 4);

  return (1 + Math.round(((date.getTime() - week1.getTime()) / 86400000 - 3 + ((week1.getDay() + 6) % 7)) / 7));
};

// Returns the four-digit year corresponding to the ISO week of the date.
getWeekYear = function() {
  const date = new Date();
  date.setDate(date.getDate() + 3 - ((date.getDay() + 6) % 7));
  return date.getFullYear();
};

function decodeUrl(url) {
  return decodeURIComponent(url.replace(/\+/g, " "));
}




/*Lesson information \/   */

function getLessonFromParameter(model) {
  document.getElementById("lesson-title").innerHTML = model.subjects[0];

  const lessonDescription = document.getElementById("lesson-description");
    lessonDescription.innerHTML = "";

  const a = document.createElement("li");
  a.classList.add("list");
  a.innerHTML = unixToDatetime(model.start).toLocaleString('nl-NL', {
    hour: '2-digit',
    minute: '2-digit'
  }) + " - " + unixToDatetime(model.end).toLocaleString('nl-NL', {
    hour: '2-digit',
    minute: '2-digit'
  });

  lessonDescription.appendChild(a);

  const b = document.createElement("li");
  b.classList.add("list");
  b.innerHTML = model.locations[0];
  lessonDescription.appendChild(b);

  const c = document.createElement("li");
  c.classList.add("list");
  c.innerHTML =  model.teachers.join(", ");
  lessonDescription.appendChild(c);
}

function unixToDatetime(unix) {
  return new Date(unix * 1000);
}




/*Inloggen met zermelo   */
function authenticateZermelo_step_1(username, password) {
  const data = "username=" + username + "&password=" + password + "&client_id=OAuthPage&redirect_uri=/main/&scope=&state=" + generateString(6) + "&response_type=code&tenant=ccg";

  let xhr = new XMLHttpRequest();
  //xhr.withCredentials = true;

  xhr.addEventListener("readystatechange", function() {
    if (this.readyState === 4) {
      const regex = /[a-zA-Z0-9]{20}/;
      const accessToken = this.responseText.match(regex)[0];
      //console.log("first > " + accessToken);
      authenticateZermelo_step_2(accessToken);
    }
  });

  xhr.open("POST", "https://ccg.zportal.nl/api/v3/oauth");
  xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
  xhr.send(data);
}

function generateString(number) {
  let text = "";
  const possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

  for (let i = 0; i < number; i++)
    text += possible.charAt(Math.floor(Math.random() * possible.length));

  return text;
}

function authenticateZermelo_step_2(code) {
  const data = "code=" + code + "&client_id=ZermeloPortal&client_secret=42&grant_type=authorization_code&rememberMe=true";
  //POST: https://ccg.zportal.nl/api/v3/oauth/token

  let xhr = new XMLHttpRequest();
  //xhr.withCredentials = true;

  xhr.addEventListener("readystatechange", function() {
    if (this.readyState === 4) {
      //console.log("real > " + this.responseText);

      const zermeloAuthModel = JSON.parse(this.responseText);
      const model = {
        access_token: zermeloAuthModel.access_token,
        token_type: zermeloAuthModel.token_type,
        expires_in: zermeloAuthModel.expires_in
      };
      //console.log(model);
      //console.log(model.access_token);
      localStorage.setItem("zermelo-access_token", model.access_token);
      localStorage.setItem("zermelo-kwt_allowed", "true");

      getZermeloUser();
    }

  });

  xhr.open("POST", "https://ccg.zportal.nl/api/v3/oauth/token");
  xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
  xhr.send(data);
}

function authenticateZermelo_connect_code(code){
    code = code.replace(/\s/g, '');
    if(code.length === 12){

      let xhr = new XMLHttpRequest();

      xhr.addEventListener("readystatechange", function() {
        if (this.readyState === 4) {
          //console.log("real > " + this.responseText);

            const zermeloAuthModel = JSON.parse(this.responseText);
            const model = {
                access_token: zermeloAuthModel.access_token,
                token_type: zermeloAuthModel.token_type,
                expires_in: zermeloAuthModel.expires_in
            };
            //console.log(model);
            //console.log(model.access_token);
            localStorage.setItem("zermelo-access_token", model.access_token);
            localStorage.setItem("zermelo-kwt_allowed", "false");

              getZermeloUser();
        }

      });

      xhr.open("POST", "https://ccg.zportal.nl/api/v3/oauth/token?grant_type=authorization_code&code=" + code);
      xhr.send();
    }
}









function getZermeloUser() {
  let xhr = new XMLHttpRequest();

  xhr.addEventListener("readystatechange", function () {

    //console.log(this.responseText);

    if (this.readyState === 4) {

      const data = JSON.parse(this.responseText);
      const model = {
        response: {
          status: data.response.status,
          message: data.response.message,
          details: data.response.details,
          eventId: data.response.eventId,
          startRow: data.response.startRow,
          endRow: data.response.endRow,
          totalRows: data.response.totalRows,
          data: data.response.data.map((item) => ({
            code: item.code,
            firstName: item.firstName,
            prefix: item.prefix,
            lastName: item.lastName,
            gender: item.gender,
            email: item.email,
            street: item.street,
            city: item.city,
            dateOfBirth: item.dateOfBirth,
            schoolInSchoolYears: item.schoolInSchoolYears,
            houseNumber: item.houseNumber,
            postalCode: item.postalCode,
          })),
        },
      };

      //console.log(model);

      localStorage.setItem("zermelo-student_id", model.response.data[0].code);

      window.location.href = "/Zermos/zermelo/rooster/";
    }
  });

  xhr.open("GET", "https://ccg.zportal.nl/api/v3/users/~me?access_token=" + localStorage.getItem("zermelo-access_token"), true);
  xhr.send();
}