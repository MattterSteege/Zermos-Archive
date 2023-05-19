console.log("Antonius.js loaded successfully");

function getInfoboord() {

    const request = new XMLHttpRequest();
    request.open('GET', 'https://cors-proxy.mjtsgamer.workers.dev/?url=https://www.carmelcollegegouda.nl/vestigingen/antoniuscollege-gouda/infoscherm', true);

    request.onreadystatechange = function() {
        if (this.readyState === 4) {
            //console.log(this.responseText);

            const parser = new DOMParser()
            const doc = parser.parseFromString(this.responseText, 'text/html')

            // Use DOM methods to select the desired elements
            const elements = doc.querySelectorAll('.swiper-slide')

            let newsItems = []

            for (const element of elements) {
                //console.log(element)

                const title = element.querySelector('h1.text-black')?.textContent ?? ""
                const subTitle = element.querySelector('h2.text-black')?.textContent ?? ""
                const image = element.querySelector('img').src.replace(document.location.origin, /*proxy*/ "https://www.carmelcollegegouda.nl")

                const content = []
                for (const contentElement of element.querySelectorAll('.content')) {
                    content.push(contentElement.textContent)
                }

                const model = {
                    title: title,
                    subTitle: subTitle,
                    image: image,
                    content: content
                }

                newsItems.push(model)
            }

            //console.log(newsItems)

            const newsItemsHolder = document.getElementById("news-items-holder")

            for (const newsItem of newsItems) {

                const newsItemElement = document.createElement("div")
                newsItemElement.className = "child"

                if (newsItem.title !== "" && newsItem.title !== undefined) {

                    const newsItemTitle = document.createElement("h1")
                    newsItemTitle.className = "heading"
                    newsItemTitle.textContent = newsItem.title

                    newsItemElement.appendChild(newsItemTitle)
                }

                if (newsItem.content.length > 0) {
                    const newsItemText = document.createElement("p")
                    newsItemText.className = "text"
                    newsItemText.textContent = newsItem.content.join("<br><br>")

                    newsItemElement.appendChild(newsItemText)
                }

                if (newsItem.image !== "" && newsItem.image !== undefined) {
                    const newsItemImageOverlay = document.createElement("div")
                    newsItemImageOverlay.className = "image-overlay"

                    const newsItemImage = document.createElement("img")
                    newsItemImage.src = newsItem.image

                    const newsItemImageOverlayOverlay = document.createElement("div")
                    newsItemImageOverlayOverlay.className = "overlay"

                    newsItemImageOverlay.appendChild(newsItemImage)
                    newsItemImageOverlay.appendChild(newsItemImageOverlayOverlay)

                    newsItemElement.appendChild(newsItemImageOverlay)
                }

                newsItemsHolder.appendChild(newsItemElement)
            }
        }
    }

    request.send();

}
