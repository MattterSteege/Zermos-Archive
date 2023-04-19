//https://ccg.zportal.nl/api/v3/liveschedule?access_token=gvcnr9ck37l96uk70u4dcdbuq8&student=58373&week=202315
function getDagrooster(){
    let ajaxRequest = new XMLHttpRequest();
    ajaxRequest.onreadystatechange = function(){
        console.log("Ready state changed!");
        //more on this in a second
    }
    ajaxRequest.open("GET", "https://ccg.zportal.nl/api/v3/liveschedule?access_token=gvcnr9ck37l96uk70u4dcdbuq8&student=58373&week=202315", true);
    ajaxRequest.send();

    ajaxRequest.onreadystatechange = function(){
        if(ajaxRequest.readyState === 4){
            const data = JSON.parse(ajaxRequest.responseText);
            const container = document.getElementById("content");

            for (const key in data) {
                const value = data[key];
                const element = document.createElement("div");
                element.innerText = `${key}: ${value}`;
                container.appendChild(element);
            }
        }
    }
}