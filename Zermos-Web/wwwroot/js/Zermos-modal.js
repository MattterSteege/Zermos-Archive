class ZermosModal {
    constructor() {
        this.title = '';
        this.fields = [];
        this.onSubmitCallback = null;
        this.onCloseCallback = null;
        this.closingDisabled = false;
        this.fadeDuration = 500;
        this.submitButtonLabel = 'Verstuur!';
    }

    //ADDERS
    addText(text) {
        this.fields.push(["text",text]);
        return this;
    }

    addInput(input) {
        this.fields.push(["text_input",input]);
        return this;
    }
    
    addMultilineInput(input) {
        this.fields.push(["multiline_input", input]);
        return this;
    }
    
    addSelectInput(input, options) {
        this.fields.push(["select_input", input, options]);
        return this;
    }

    addDateInput(input, defaultDate) {
        this.fields.push(["date_input", input, defaultDate]);
        return this;
    }

    addButton(label, callback) {
        this.fields.push(["button", label, callback]);
        return this;
    }
    
    addToggle(label, toggled) {
        this.fields.push(["toggle", label, toggled]);
        return this;
    }
    
    addMultiToggles(toggles, toggled) {
        //make a list of toggles which collectively return an array of true/false values
        //this array is then returned to the callback
        this.fields.push(["multi_toggle", toggles, toggled]);
        return this;
    }
    
    //SETTERS
    setTitle(title) {
        this.title = title;
        return this;
    }
    
    setFadeDuration(duration) {
        this.fadeDuration = duration;
        return this;
    }
    
    setSubmitButtonLabel(label) {
        this.submitButtonLabel = label;
        return this;
    }
    
    disableClose() {
        this.closingDisabled = true;
        return this;
    }
    
    hideSubmitButton() {
        this.submitButtonLabel = null;
        return this;
    }

    //CALLBACKS
    onSubmit(callback) {
        this.onSubmitCallback = callback;
        return this;
    }

    onClose(callback) {
        this.onCloseCallback = callback;
        return this;
    }

    open() {
        //remove any existing modal
        const existingModal = document.querySelector('#modal');
        if (existingModal) {
            document.body.removeChild(existingModal);
        }
        
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
                    inputField = createInput(`input${index}`, 'text', input[1]);
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

        function createInput(id, type, placeholder) {
            const input = document.createElement('input');
            input.id = id;
            input.type = type;
            input.placeholder = placeholder;
            return input;
        }

        function createSelectInput(id, options) {
            const select = document.createElement('select');
            select.id = id;
            options.forEach((option) => {
                const optionElement = document.createElement('option');
                optionElement.value = option;
                optionElement.textContent = option;
                select.appendChild(optionElement);
            });
            return select;
        }
        
        function createMultilineInput(id, placeholder) {
            const input = document.createElement('textarea');
            input.id = id;
            input.placeholder = placeholder;
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
            return input;
        }
        
        function createButton(id, label, callback) {
            if (!label) return null;
            
            const button = document.createElement('button');
            button.id = id;
            button.textContent = label;
            button.addEventListener('click', callback);
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
                        
            return toggleParent;
        }
        
        function createMultiToggle(id, labels, toggled) {
            const toggle = document.createElement('div');
            toggle.id = id;
            toggle.classList.add('multi-toggle');
            labels.forEach((label, index) => {
                toggle.appendChild(createToggle(`${id}${index}`, label, toggled[index]));
            });
            return toggle;
        }
        
        function createText(text) {
            const p = document.createElement('p');
            p.innerHTML = text;
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
                    const inputValues = this.fields.map((_, i) => {
                        const inputElement = modal.querySelector(`#input${i}`);
                        if ((!inputElement || !inputElement.value) && !inputElement?.children[0]?.children[0]?.value) {
                            return null;
                        }

                        return inputElement.value === undefined ? Array.from(inputElement.children).map(child => child.children[0].checked) : inputElement.value;
                    });
                    this.onSubmitCallback(...inputValues.filter(value => value !== null));
                }
                document.body.removeChild(modal);
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
                document.body.removeChild(modal);
            }, this.fadeDuration);
        });

        //if the user presses enter go to the next input field, if there is one otherwise submit
        document.querySelector('#modal').addEventListener('keydown', (e) => {
            if (e.key === "Enter") {
                const nextInput = e.target.nextElementSibling;
                if (nextInput) {
                    nextInput.focus();
                } else {
                    submitButton.click();
                }
            }
        });
    }
}