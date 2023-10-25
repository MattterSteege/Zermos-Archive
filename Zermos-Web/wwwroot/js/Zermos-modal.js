class ZermosModal {
    constructor() {
        this.title = '';
        this.description = '';
        this.inputs = [];
        this.onSubmitCallback = null;
        this.onCloseCallback = null;
        this.fadeDuration = 500;
        this.submitButtonLabel = 'Verstuur!';
    }

    addTitle(title) {
        this.title = title;
        return this;
    }

    addDescription(description) {
        this.description = description;
        return this;
    }

    addInput(input) {
        this.inputs.push(["text_input",input]);
        return this;
    }
    
    addMultilineInput(input) {
        this.inputs.push(["multiline_input", input]);
        return this;
    }
    
    addSelectInput(input, options) {
        this.inputs.push(["select_input", input, options]);
        return this;
    }
    
    addDateInput(input) {
        this.inputs.push(["date_input", input]);
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

    onSubmit(callback) {
        this.onSubmitCallback = callback;
        return this;
    }

    onClose(callback) {
        this.onCloseCallback = callback;
        return this;
    }

    open() {
        // Create the modal HTML, add event listeners, and display it
        const modal = createModal.call(this);

        function createModal() {
            const modal = createDivWithClass('modal');

            const modalContent = createDivWithClass('modal-content');
            modalContent.innerHTML = `
                <h2>${this.title}</h2>
                <p>${this.description}</p>
              `;

            const inputs = createInputFields(this.inputs);

            const submitButton = createButton('submitBtn', this.submitButtonLabel);

            modal.appendChild(modalContent);
            modal.appendChild(inputs);
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

        function createInputFields(inputs) {
            const inputContainer = createDivWithClass('input-container');
            inputs.forEach((input, index) => {
                let inputField;
                
                if (input[0] === "text_input") 
                    inputField = createInput(`input${index}`, 'text', input[1]);
                else if (input[0] === "select_input")
                    inputField = createSelectInput(`input${index}`, input[2]);
                else if (input[0] === "multiline_input")
                    inputField = createMultilineInput(`input${index}`, input[1]);
                else if (input[0] === "date_input")
                    inputField = createDateInput(`input${index}`, input[1]);

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
        
        function createDateInput(id, placeholder) {
            const input = document.createElement('input');
            input.id = id;
            input.type = "date";
            input.placeholder = placeholder;
            return input;
        }
        
        function createButton(id, label) {
            const button = document.createElement('button');
            button.id = id;
            button.textContent = label;
            return button;
        }

        function fade(element, opacity, duration) {
            var ease = function (t) {return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1};
            var start = performance.now();
            var from = element.style.opacity || 0;
            requestAnimationFrame(function tick(timestamp) {
                    var progress = timestamp - start;
                    element.style.opacity = +from + (opacity - from) * ease(progress / duration);
                    if (progress < duration) {
                        requestAnimationFrame(tick);
                    }
                }
            );
        }
        
        document.body.appendChild(modal);
        fade(modal, 1, this.fadeDuration);

        const submitButton = modal.querySelector('#submitBtn');

        submitButton.addEventListener('click', () => {
            if (this.onSubmitCallback) {
                const inputValues = this.inputs.map((_, i) => {
                    const inputElement = modal.querySelector(`#input${i}`);
                    if (!inputElement || !inputElement.value) {
                        return null;
                    }
                    return inputElement.value;
                });
                this.onSubmitCallback(...inputValues);
            }
            document.body.removeChild(modal);
        });
        
        document.querySelector('#modal').addEventListener('click', (e) => {
            if (this.onCloseCallback) {
                this.onCloseCallback();
            }
            
            //if the one clicked is not the background (modal), return
            if (e.target.id !== 'modal') return;
            
            var modal = document.querySelector('#modal');
            fade(modal, 0, this.fadeDuration);
            setTimeout(() => {
                document.body.removeChild(modal);
            }, this.fadeDuration);
        });
    }
}