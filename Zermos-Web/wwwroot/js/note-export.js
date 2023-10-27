function generate(json) {
    
    console.log("[ ZERMOS ]  Begonnen met het genereren van een document.");
    
    const doc = new docx.Document({
        features: {
            updateFields: true,
        },
        sections: [],
    });

    let heading = new docx.Paragraph({
        text: json.titel,
        heading: docx.HeadingLevel.HEADING_1,
    });

    let desc = new docx.Paragraph({
        text: json.omschrijving,
    });
    
    let lastEdit = new docx.Paragraph({
        text: "Laatst gewijzigd: " + json.lastEdit + "\n\r\n\r\n\r",
    });
    
    let toc =  new docx.TableOfContents("Summary", {
        hyperlink: true,
        headingStyleRange: "1-5",
    });

    doc.addSection({
        children: [heading, desc, lastEdit, toc],
    });
    
    for (let i = 0; i < json.paragraphs.length; i++) {
        let heading = new docx.Paragraph({
            text: json.paragraphs[i].naam,
            heading: docx.HeadingLevel.HEADING_1,
        });
        
        let Paragraph = new docx.Paragraph({
            text: json.paragraphs[i].inhoud,
        });
        
        doc.addSection({
            children: [heading, Paragraph],
        });
    }




    docx.Packer.toBlob(doc).then(blob => {
        saveAs(blob, json.titel + ".docx");
        console.log("[ ZERMOS ]  Document gegenereerd en gedownload.");
        new ZermosModal()
            .addTitle("Document gegenereerd")
            .addDescription("Het document is gegenereerd en gedownload. Om het document optimaal te kunnen gebruiken, open het document in Microsoft Word. en klik op 'Bewerken inschakelen', dan worden de inhoudsopgave en de links geactiveerd.")
            .setSubmitButtonLabel("Sluiten")
            .open();
    });
}