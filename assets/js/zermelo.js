function getDagrooster(){
    let ajaxRequest = new XMLHttpRequest();
    const access_token = localStorage.getItem("zermelo-access_token");
    const student = localStorage.getItem("zermelo-student_id");


    let currentDate = new Date();
    let startDate = new Date(currentDate.getFullYear(), 0, 1);
    let days = Math.floor((currentDate - startDate) / (24 * 60 * 60 * 1000));
    const weekNumber = Math.ceil(days / 7);

    ajaxRequest.open("GET", "https://ccg.zportal.nl/api/v3/liveschedule?access_token=" + access_token + "&student=" + student + "&week=2023" + weekNumber, true);
    ajaxRequest.send();

    ajaxRequest.onreadystatechange = function(){
        if(ajaxRequest.readyState === 4){
            //navigator.clipboard.writeText(ajaxRequest.responseText);
            const data = JSON.parse(ajaxRequest.responseText);
            console.log(data);

            const model = {
                response: {
                    status: data.response.status,
                    message: data.response.message,
                    details: data.response.details,
                    eventId: data.response.eventId,
                    startRow: data.response.startRow,
                    endRow: data.response.endRow,
                    totalRows: data.response.totalRows,
                    data: data.response.data.map(item => ({
                        laatsteWijziging: item.laatsteWijziging,
                        appointments: item.appointments.map(appointment => ({
                            status: appointment.status.map(status => ({
                                code: status.code,
                                nl: status.nl,
                                en: status.en
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
                            id: appointment.id
                        }))
                    }))
                }
            };

            let appointmentsByDay = {
                1: [], // Monday
                2: [], // Tuesday
                3: [], // Wednesday
                4: [], // Thursday
                5: []  // Friday
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
                        gridItem.textContent = (appointmentDay[i].subjects.length > 0 ? appointmentDay[i].subjects[0] : "") + (appointmentDay[i].locations[0] ? " - " + appointmentDay[i].locations[0] : "") + (appointmentDay[i].teachers[0] ? " - " + appointmentDay[i].teachers[0] : "");

                        if(appointmentDay[i].cancelled === true && gridItem.textContent !== ""){
                            gridItem.classList.add("cancelled");
                        }

                        //href to ./lesson?id=[id]
                        gridItem.addEventListener("click", function(){
                            window.location.href = "./lesson/?id=" + appointmentDay[i].id;
                        });

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
    }
}

getWeek = function() {
    var date = new Date(this.getTime());
    date.setHours(0, 0, 0, 0);
    // Thursday in current week decides the year.
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    // January 4 is always in week 1.
    var week1 = new Date(date.getFullYear(), 0, 4);
    // Adjust to Thursday in week 1 and count number of weeks from date to week1.
    return 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000
        - 3 + (week1.getDay() + 6) % 7) / 7);
}

// Returns the four-digit year corresponding to the ISO week of the date.
getWeekYear = function() {
    var date = new Date(this.getTime());
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    return date.getFullYear();
}

function getLessonFromParameter(){
    //add div with content: window.location.search.replace("?id=", "")
    document.getElementById("lesson").textContent = window.location.search.replace("?id=", "");
}