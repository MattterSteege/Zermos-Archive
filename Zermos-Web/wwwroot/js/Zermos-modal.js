class ZermosModal {
    constructor() {
        this.title = '';
        this.fields = [];
        this.elements = [];
        this.onSubmitCallback = null;
        this.onCloseCallback = null;
        this.closingDisabled = false;
        this.fadeDuration = 500;
        this.submitButtonLabel = 'Verstuur!';
    }

    //ADDERS
    
    //
    // @param {string} text - The text to be displayed
    //
    addText(text) {
        this.fields.push(["text",text]);
        return this;
    }

    //
    // @param {string} input - The placeholder text for the input
    //
    addInput(input) {
        this.fields.push(["text_input",input]);
        return this;
    }
    
    //
    // @param {string} input - The placeholder text for the input
    //
    addMultilineInput(input) {
        this.fields.push(["multiline_input", input]);
        return this;
    }
    
    //
    // @param {string} input - The placeholder text for the input
    // @param {array} options - The options for the select input
    //
    addSelectInput(input, options) {
        this.fields.push(["select_input", input, options]);
        return this;
    }

    //
    // @param {string} input - The placeholder text for the input
    // @param {Date} defaultDate - The default date for the input
    //
    addDateInput(input, defaultDate) {
        this.fields.push(["date_input", input, defaultDate]);
        return this;
    }

    //
    // @param {string} label - The label for the button
    // @param {function} callback - The callback for the button
    //
    addButton(label, callback) {
        this.fields.push(["button", label, callback]);
        return this;
    }
    
    //
    // @param {string} label - The label for the toggle
    // @param {boolean} toggled - Whether the toggle is toggled or not
    //
    addToggle(label, toggled) {
        this.fields.push(["toggle", label, toggled]);
        return this;
    }
    
    //
    // @param {array} toggles - The labels for the toggles
    // @param {array} toggled - Whether the toggles are toggled or not
    //
    addMultiToggles(toggles, toggled) {
        //make a list of toggles which collectively return an array of true/false values
        //this array is then returned to the callback
        this.fields.push(["multi_toggle", toggles, toggled]);
        return this;
    }
    
    //SETTERS
    
    //
    // @param {string} title - The title of the modal
    //
    setTitle(title) {
        this.title = title;
        return this;
    }
    
    //
    // @param {number} duration - The duration of the fade in/out animation
    //
    setFadeDuration(duration) {
        this.fadeDuration = duration;
        return this;
    }
    
    //
    // @param {string} label - The label for the submit button
    //
    setSubmitButtonLabel(label) {
        this.submitButtonLabel = label;
        return this;
    }
    
    //
    // This disables the closing of the modal when clicking on the background
    //
    disableClose() {
        this.closingDisabled = true;
        return this;
    }
    
    //
    // This hides the submit button
    //
    hideSubmitButton() {
        this.submitButtonLabel = null;
        return this;
    }

    //CALLBACKS
    
    //
    // The callback for when the submit button is clicked
    // @param {function} callback - The callback for the submit button
    //
    onSubmit(callback) {
        this.onSubmitCallback = callback;
        return this;
    }

    //
    // The callback for when the modal is closed
    // @param {function}    
    //
    onClose(callback) {
        this.onCloseCallback = callback;
        return this;
    }
    
    //HELPERS
    
    //
    // Returns the values of the input fields
    //
    getConstructorValues() {
        console.log(this);
        return this;
    }

    //
    // Opens the modal
    //
    open() {
        //remove any existing modal
        unloadModal();
        
        
        // Create the modal HTML, add event listeners, and display it
        const modal = createModal.call(this);

        function createModal() {
            const modal = createDivWithClass('modal');
            modal.id = 'modal';

            const modalContent = createDivWithClass('modal-content');
            var title = document.createElement('h1');
            title.textContent = this.title;
            modalContent.appendChild(title);

            const fields = createInputFields(this.fields);
            this.elements = fields.children;
            
            const submitButton = createButton('submitBtn', this.submitButtonLabel);

            modal.appendChild(modalContent);
            modal.appendChild(fields);
            if (submitButton !== null)
                modal.appendChild(submitButton);

            const modalBackground = createDivWithClass('modal-background');
            modalBackground.id = 'modal';
            modalBackground.style.opacity = 0;
            modalBackground.appendChild(modal);

            return modalBackground;
        }

        function createDivWithClass(className) {
            const div = document.createElement('div');
            div.classList.add(className);
            return div;
        }

        function createInputFields(fields) {
            const inputContainer = createDivWithClass('input-container');
            fields.forEach((input, index) => {
                let inputField;
                
                if (input[0] === "text_input") 
                    inputField = createInput(`input${index}`, input[1]);
                else if (input[0] === "select_input")
                    inputField = createSelectInput(`input${index}`, input[2]);
                else if (input[0] === "multiline_input")
                    inputField = createMultilineInput(`input${index}`, input[1]);
                else if (input[0] === "date_input")
                    inputField = createDateInput(`input${index}`, input[1], input[2]);
                else if (input[0] === "button")
                    inputField = createButton(`input${index}`, input[1], input[2]);
                else if (input[0] === "text")
                    inputField = createText(input[1]);
                else if (input[0] === "toggle")
                    inputField = createToggle(`input${index}`, input[1], input[2]);
                else if (input[0] === "multi_toggle")
                    inputField = createMultiToggle(`input${index}`, input[1], input[2]);

                inputContainer.appendChild(inputField);
            });
            return inputContainer;
        }

        function createInput(id, placeholder) {
            const input = document.createElement('input');
            input.id = id;
            input.type = "text";
            input.placeholder = placeholder;
            input.dataset.type = "single_text_input";
            return input;
        }

        function createSelectInput(id, options) {
            const input = document.createElement('select');
            input.id = id;
            options.forEach((option) => {
                const optionElement = document.createElement('option');
                optionElement.value = option;
                optionElement.textContent = option;
                input.appendChild(optionElement);
            });
            input.dataset.type = "select";
            return input;
        }
        
        function createMultilineInput(id, placeholder) {
            const input = document.createElement('textarea');
            input.id = id;
            input.placeholder = placeholder;
            input.dataset.type = "multiline_text_input";
            return input;
        }
        
        function createDateInput(id, placeholder, defaultDate) {
            const input = document.createElement('input');
            input.id = id;
            input.type = "date";
            input.placeholder = placeholder;
            if (defaultDate) {
                input.value = defaultDate.toLocaleString('sv-SE').split(' ')[0];
            }
            input.dataset.type = "date";
            return input;
        }
        
        function createButton(id, label, callback) {
            if (!label) return null;
            
            const button = document.createElement('button');
            button.id = id;
            button.textContent = label;
            button.addEventListener('click', callback);
            button.dataset.type = "button";
            return button;
        }
        
        function createToggle(id, label, toggled) {
            const toggleParent = document.createElement('div');
            toggleParent.classList.add('toggle-parent');
            
            const toggle = document.createElement('input');
            toggle.id = id;
            toggle.type = "checkbox";
            toggle.textContent = label;
            toggle.checked = toggled;
            toggle.addEventListener('click', () => {
                toggle.classList.toggle('active');
            });
            
            const toggleLabel = document.createElement('label');
            toggleLabel.htmlFor = id;
            toggleLabel.textContent = label;
            
            toggleParent.appendChild(toggle);
            toggleParent.appendChild(toggleLabel);
                        
            toggleParent.dataset.type = "single_toggle";
            return toggleParent;
        }
        
        function createMultiToggle(id, labels, toggled) {
            const toggle = document.createElement('div');
            toggle.id = id;
            toggle.classList.add('multi-toggle');
            labels.forEach((label, index) => {
                toggle.appendChild(createToggle(`${id}${index}`, label, toggled[index]));
            });
            toggle.dataset.type = "multi_toggle";
            return toggle;
        }
        
        function createText(text) {
            const p = document.createElement('p');
            p.innerHTML = text;
            p.dataset.type = "text";
            return p;
        }

        function fade(element, opacity, duration) {
            setTimeout(() => {
                element.style.transition = `opacity ${duration}ms`;
                element.style.opacity = opacity;
            }, 10);
        }
        
        document.body.appendChild(modal);
        fade(modal, 1, this.fadeDuration);

        const submitButton = modal.querySelector('#submitBtn');
        

        if (submitButton) {
            submitButton.addEventListener('click', () => {
                if (this.onSubmitCallback) {
                    
                    const inputValues = Array.from(this.elements).map((inputElement) => {
                        if (inputElement.dataset.type === "text") return null;
                        if (inputElement.dataset.type === "button") return null;
                        
                        
                        //special case for toggles
                        if (inputElement.dataset.type === "single_toggle") {
                            return inputElement.children[0].checked;
                        } else if (inputElement.dataset.type === "multi_toggle") {
                            return Array.from(inputElement.children).map(child => child.children[0].checked);
                        } 
                        
                        //everything else
                        else {
                            return inputElement.value;
                        }
                    });
                    this.onSubmitCallback(...inputValues.filter(value => value !== null));
                }
                unloadModal();
            });
        }
        
        document.querySelector('#modal').addEventListener('click', (e) => {
            if (this.closingDisabled) return;
            if (this.onCloseCallback) this.onCloseCallback();
            
            
            //if the one clicked is not the background (modal), return
            if (e.target.id !== 'modal') return;
            
            var modal = document.querySelector('#modal');
            fade(modal, 0, this.fadeDuration);
            setTimeout(() => {
                unloadModal()
            }, this.fadeDuration);
        });
    }
}

function unloadModal() {
    if (document.querySelector('#modal'))
        document.body.removeChild(document.querySelector('#modal'));
}