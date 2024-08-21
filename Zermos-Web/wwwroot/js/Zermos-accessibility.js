// Function to handle links with onclick attributes
function handleOnClickLinks() {
    const links = main.querySelectorAll("[onclick]");

    links.forEach((link, index) => {
        // Add role="button" and tabindex to simulate button behavior
        link.setAttribute("role", "button");
        link.setAttribute("tabindex", "0");
        link.setAttribute("aria-pressed", "false");

        // Add keyboard accessibility
        link.addEventListener("keypress", function(event) {
            if (event.key === "Enter" || event.key === " ") {
                link.click();
                event.preventDefault();
            }
        });

        // Optionally, add an aria-label based on the inner text or a default description
        if (!link.hasAttribute("aria-label")) {
            const linkText = link.innerText || "interactive element";
            link.setAttribute("aria-label", `Activate ${linkText}`);
        }
    });
}

// Function to handle standard links without onclick attributes
function handleStandardLinks() {
    const links = main.querySelectorAll("a:not([onclick])");

    links.forEach((link) => {
        if (!link.hasAttribute("aria-label")) {
            link.setAttribute("aria-label", `Link to ${link.getAttribute("href")}`);
        }
    });
}

//to avoid confilt with the sidebar
const buttonTabIndexOffset = 100;

// Function to handle tabindex for button roles
function handleButtonRoles() {
    let buttons = main.querySelectorAll("[role='button']");

    buttons.forEach((button, index) => {
        button.setAttribute("tabindex", `${index + 1 + buttonTabIndexOffset}`);
    });
}

// Skip images as per requirements
function handleImages() {
    const images = main.querySelectorAll("img");

    images.forEach((img) => {
        img.setAttribute("alt", "");
        img.setAttribute("aria-hidden", "true");
    });
}

Zermos.mainProcessAccessibility = function() {
    handleOnClickLinks();
    handleStandardLinks();
    handleButtonRoles();
    handleImages();
}