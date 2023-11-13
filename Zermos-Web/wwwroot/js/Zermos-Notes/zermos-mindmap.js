const divs = document.querySelectorAll('.draggable');
let highestZIndex = 1;

//list of coupled divs
var coupledDivs = [];
var firstDiv = null;
var secondDiv = null;

//setUp the canvas by setting each div's id and each coupler's id
divs.forEach((div, index) => {
    div.id = `div${index}`;
    var couplers = div.querySelectorAll('.coupler');
    couplers.forEach((coupler, index) => {
        //0 is top, 1 is bottom, 2 is left, 3 is right
        coupler.id = `coupler${index}`;
    });
});


divs.forEach((div) => {
    let offsetX, offsetY;

    const move = (e) => {
        // Calculate the new position for the current div
        let newLeft = Math.min(Math.max(0, e.clientX - offsetX), window.innerWidth - div.offsetWidth);
        let newTop = Math.min(Math.max(0, e.clientY - offsetY), window.innerHeight - div.offsetHeight);


        div.style.left = `${newLeft}px`;
        div.style.top = `${newTop}px`;
        div.style.zIndex = highestZIndex++; // Increment z-index

        var c=document.getElementById("linesCanvas");
        var ctx=c.getContext("2d");
        ctx.clearRect(0, 0, c.width, c.height);

        //recalculate all the lines
        coupledDivs.forEach((coupledDiv) => {
            var parentOfFirst = coupledDiv[0].parentElement;
            var parentOfSecond = coupledDiv[1].parentElement;

            // Assuming you have the parent elements in parentOfFirst and parentOfSecond
            var childrenOfFirst = Array.from(parentOfFirst.children);
            var childrenOfSecond = Array.from(parentOfSecond.children);

            var closestChildFromFirst = null;
            var closestChildFromSecond = null;
            var minDistance = Infinity;
            var smoothingDistance = 100; //px

            // Loop through the children of the first parent
            for (var i = 0; i < childrenOfFirst.length; i++) {
                var child1 = childrenOfFirst[i];

                // Loop through the children of the second parent
                for (var j = 0; j < childrenOfSecond.length; j++) {
                    var child2 = childrenOfSecond[j];

                    var coords1 = calculateCoords(child1);
                    var coords2 = calculateCoords(child2);

                    // Calculate the distance between the two children
                    var distance = Math.sqrt(Math.pow(coords1.x - coords2.x, 2) + Math.pow(coords1.y - coords2.y, 2));

                    // If the distance is less than the minimum distance, update the minimum distance and the closest children
                    // if (distance < minDistance) {
                    //     minDistance = distance;
                    //     closestChildFromFirst = child1;
                    //     closestChildFromSecond = child2;
                    // }

                    //if the distance between the two children is less than the distance between the current closest children - smoothingDistance
                    //then update the closest children
                    if (distance < minDistance) {
                        minDistance = distance;
                        closestChildFromFirst = child1;
                        closestChildFromSecond = child2;
                    }
                }
            }

            coupledDiv[0] = closestChildFromFirst;
            coupledDiv[1] = closestChildFromSecond;

            var firstCoords = calculateCoords(coupledDiv[0]);
            var secondCoords = calculateCoords(coupledDiv[1]);

            //draw the line
            DrawLine(firstCoords.x, secondCoords.x, firstCoords.y, secondCoords.y);
        });

    }

    div.addEventListener('mousedown', (e) => {
        // Bring the current div to the front
        div.style.zIndex = highestZIndex++;

        // Calculate the initial offset values for the current div
        offsetX = e.clientX - div.offsetLeft;
        offsetY = e.clientY - div.offsetTop;
        document.addEventListener('mousemove', move);
    });

    document.addEventListener('mouseup', (e) => {
        document.removeEventListener('mousemove', move);
    });

    const couplers = document.querySelectorAll('.coupler');
    couplers.forEach((coupler) => {
        coupler.onclick = (e) => {
            if (firstDiv == null) {
                firstDiv = coupler;

                coupler.style.backgroundColor = "red";
            } else {
                secondDiv = coupler;
                coupledDivs.push([firstDiv, secondDiv]);

                coupler.style.backgroundColor = "red";

                var firstCoords = calculateCoords(firstDiv);
                var secondCoords = calculateCoords(secondDiv);

                //draw the line
                DrawLine(firstCoords.x, secondCoords.x, firstCoords.y, secondCoords.y);

                firstDiv = null;
                secondDiv = null;
            }
        }
    });
});

function DrawLine(x1, x2, y1, y2) {
    var c=document.getElementById("linesCanvas");
    var ctx=c.getContext("2d");

    ctx.beginPath();
    ctx.lineWidth="4";
    ctx.strokeStyle="red";
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
}

function calculateCoords(div) {
    var coords = {
        x: (div.getBoundingClientRect().x + 5) / window.innerWidth * 1920,
        y: (div.getBoundingClientRect().y + 9) / window.innerHeight * 1080
    }
    return coords;
}

function ConnectAllCouplers() {
    var couplers = document.querySelectorAll('.coupler');
    couplers.forEach((coupler) => {

        couplers.forEach((coupler2) => {
            if (coupler !== coupler2) {
                coupledDivs.push([coupler, coupler2]);

                coupler.style.backgroundColor = "red";
                coupler2.style.backgroundColor = "red";

                var firstCoords = calculateCoords(coupler);
                var secondCoords = calculateCoords(coupler2);

                //draw the line
                DrawLine(firstCoords.x, secondCoords.x, firstCoords.y, secondCoords.y);
            }
        });
    });
}

function Clear() {
    var c=document.getElementById("linesCanvas");
    var ctx=c.getContext("2d");
    ctx.clearRect(0, 0, c.width, c.height);

    coupledDivs = [];
    firstDiv = null;
    secondDiv = null;

    var couplers = document.querySelectorAll('.coupler');
    couplers.forEach((coupler) => {
        //remove the red background from the style attribute
        coupler.style.backgroundColor = "";
    });
}

function Save() {
    const savedData = {
        divs: [],
        coupledDivs: [],
    };

    // Save the positions of all divs
    divs.forEach((div) => {
        savedData.divs.push({
            id: div.id,
            left: div.style.left,
            top: div.style.top,
        });
    });

    // Save the connections between couplers
    coupledDivs.forEach((coupledDiv) => {
        //save as #[parentDiv.Id] #[coupledDiv.Id]
        var one = "#" + coupledDiv[0].parentNode.id + " #" + coupledDiv[0].id;
        var two = "#" + coupledDiv[1].parentNode.id + " #" + coupledDiv[1].id;
        savedData.coupledDivs.push([one, two]);
    });

    const jsonString = JSON.stringify(savedData);
    // You can now store the `jsonString` or use it as needed (e.g., save it to a file or send it to a server).
    console.log(jsonString);
    //localstorage
    localStorage.setItem("savedData", jsonString);
}


function Load() {
    Clear();

    //const jsonData = prompt("Please enter the JSON string", "");
    const jsonData = localStorage.getItem("savedData"); // or any other source of JSON string
    if (jsonData) {
        try {
            const parsedData = JSON.parse(jsonData);

            // Restore div positions
            parsedData.divs.forEach((divData) => {
                const div = document.getElementById(divData.id);
                div.style.left = divData.left;
                div.style.top = divData.top;
            });

            // Restore coupler connections
            parsedData.coupledDivs.forEach((coupledDivIds) => {
                const firstDiv = document.querySelector("" + coupledDivIds[0].toString())
                const secondDiv = document.querySelector("" + coupledDivIds[1].toString())
                coupledDivs.push([firstDiv, secondDiv]);

                firstDiv.style.backgroundColor = "red";
                secondDiv.style.backgroundColor = "red";

                const firstCoords = calculateCoords(firstDiv);
                const secondCoords = calculateCoords(secondDiv);

                // Redraw the line
                DrawLine(firstCoords.x, secondCoords.x, firstCoords.y, secondCoords.y);
            });
        } catch (error) {
            console.error("Error loading data:", error);
            alert("Error loading data. Please check the JSON format.");
        }
    }
}
