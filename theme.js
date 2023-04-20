function getDagrooster(){
    let ajaxRequest = new XMLHttpRequest();
    ajaxRequest.onreadystatechange = function(){
        console.log("Ready state changed!");
        //more on this in a second
    }
    ajaxRequest.open("GET", "https://ccg.zportal.nl/api/v3/liveschedule?access_token=gvcnr9ck37l96uk70u4dcdbuq8&student=58373&week=202316", true);
    ajaxRequest.send();

    ajaxRequest.onreadystatechange = function(){
        if(ajaxRequest.readyState === 4){
            //navigator.clipboard.writeText(ajaxRequest.responseText);
            const data = JSON.parse(ajaxRequest.responseText);

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
                        gridItem.textContent = appointmentDay[i].subjects.length > 0 ? appointmentDay[i].subjects[0] : "";

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