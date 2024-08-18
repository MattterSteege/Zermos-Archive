class ZermosModal {
    constructor(disableClosing = false, components = [], conditions = {}) {
        this.disableClosing = disableClosing;
        this.components = components;
        this.conditions = conditions;
        this.close();
    }

    addComponent(component) {
        if (!component.id) component.id = btoa(crypto.getRandomValues(new Uint8Array(8))).slice(0,11);
        this.components.push(component);
        return this;
    }

    setCondition(key, value) {
        this.conditions[key] = value;
        this.updateRenderedModal();
        return this;
    }

    
    //Returns the values of all the components in the modal an example would be (in json, but it's an object):
    //{ correct: true, values: [{type: "input", id: "input1", value: "value1", correct: true}, ... ] }
    getComponentsValue() {
        var values = [];
        var correctForm = true;
        this.components.forEach((elem) => {
            if (elem.type === "submenu"){
                var subValues = elem.subModal.getComponentsValue();
                subValues.correct == false ? correctForm = false : null;
                values.push(...subValues.values)
            }
            if ((elem.required && (elem.userSetValue === "" || elem.userSetValue === null || elem.userSetValue === undefined)) && this.isInputType(elem.type)){
                correctForm = false;
                values.push({type: elem.type, id: elem.id, value: null, correct: false});
            }
            else if (this.isInputType(elem.type))
            {
                var value = {type: elem.type, id: elem.id, value: null, correct: true};
                if (elem.userSetValue === "true" || elem.userSetValue === "false")
                    value.value = !!elem.userSetValue;
                else if (elem.userSetValue === undefined)
                    value.value = null;
                else
                    value.value = elem.userSetValue;

                values.push(value)
            }
        });

        return {correct: correctForm, values: values};
    }

    isInputType(type){
        return type.toLowerCase().includes("input");
    }

    updateRenderedModal() {
        document.querySelectorAll('div[showCondition]').forEach(element => {
            if (this.evaluateExpression(element.getAttribute('showCondition')))
                element.parentElement.classList.remove("hidden");
            else
                element.parentElement.classList.add("hidden");
        });
    }

    addHeading({text, subheading = "", level = 1, id = undefined}) {
        return this.addComponent({ type: 'heading', text, subheading, level, id: id});
    }

    addToggle({label, initialState = false, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'toggleInput', label, state: initialState, onChange, id: id});
    }

    addMultiToggles({labels, initialStates = [],  onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'multiToggleInput', labels, states: initialStates, onChange, id: id});
    }

    addButton({text, onClick = () => {}, id = undefined}) { //TODO
        return this.addComponent({ type: 'button', text, onClick, id: id});
    }

    addDoubleButtons({text, secondText, onClick = () => {}, secondOnClick = () => {}, id = undefined}) {
        return this.addComponent({ type: 'doubleButton', text, onClick, secondText, secondOnClick, id: id});
    }

    addTripleButtons({text, secondText, thirdText, onClick = () => {}, secondOnClick = () => {}, thirdOnClick = () => {}, id = undefined}) {
        return this.addComponent({ type: 'tripleButton', text, onClick, secondText, secondOnClick, thirdText, thirdOnClick, id: id});
    }

    addSubmenu({showCondition, subModal, id = undefined}) {
        return this.addComponent({ type: 'submenu', showCondition, subModal, id: id});
    }

    addText({text, asHtml = false, id = undefined}) {
        return this.addComponent({ type: 'text', text, asHtml: asHtml, id: id});
    }

    addLabel({text, id = undefined}) {
        return this.addComponent({ type: 'label', text, id: id});
    }

    addUrl({url, showFull = false, copyButton = true, id = undefined}) {
        return this.addComponent({ type: 'url', url, showFull, copyButton, id: id});
    }

    ///INPUT ELEMENTS
    addDatePicker({required = false, initialDate = undefined, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'datePickerInput', required, initialDate, onChange, id: id});
    }

    addDropdown({options, required = false, multiSelect = false, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'dropdownInput', options, required, multiSelect, onChange, id: id});
    }

    addSeparator({text = '', id = undefined}) {
        return this.addComponent({ type: 'separator', text, id: id});
    }

    addImage({src, alt = '', id = undefined}) {
        return this.addComponent({ type: 'image', src, alt, id: id});
    }

    addSpacer({height = "20px", id = undefined}) {
        return this.addComponent({ type: 'spacer', height, id: id});
    }

    addNumberInput({required = false, initialValue = 0, decimals = 0, min = Number.MIN_VALUE, max = Number.MAX_VALUE, step = 1, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'numberInput', required, initialValue, decimals, min, max, step, onChange, id: id});
    }

    addTextInput({required = false, initialValue = '', maxLength = null, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'textInput', required, initialValue, maxLength, onChange, id: id});
    }

    addPasswordInput({required = false, maxLength = null, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'passwordInput', required, maxLength, onChange, id: id});
    }

    addTextArea({required = false, initialValue = '', maxLength = null, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'textAreaInput', required, initialValue, maxLength, onChange, id: id});
    }

    addCheckbox({initialState = false, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'toggleInput', state: initialState, asCheckbox: true, onChange, id: id});
    }
    
    addMultiCheckbox(labels, initialStates = [], onChange = () => {}, id = undefined) {
        return this.addComponent({ type: 'multiToggleInput', labels, states: initialStates, asCheckbox: true, onChange, id: id});
    }
    
    addSlider({min = 0, max = 100, step = 1, initialValue = 50, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'sliderInput', min, max, step, initialValue, onChange, id: id});
    }

    addColorPicker({required, initialColor = '#000000', onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'colorPickerInput', required, initialColor, onChange, id: id});
    }

    addRating({required, maxRating = 5, initialRating = 0, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'ratingInput', required, maxRating, initialRating, onChange, id: id});
    }
    
    addHTML({html, id = undefined}) {
        return this.addComponent({ type: 'html', html, id: id});
    }
    
    addCodeBlock({code, id = undefined}) {
        return this.addComponent({ type: 'code', code, id: id});
    }

    addList({items, listType = "ul", id = undefined}) {
        return this.addComponent({ type: 'list', items, listType, id: id});
    }

    render() {
        const modalElement = document.createElement('div');
        modalElement.className = 'zermos-modal';
        this.components.forEach(component => {
            modalElement.appendChild(this.renderComponent(component));
        });
        return modalElement;
    }

    renderComponent(component) {
        const componentElement = document.createElement('div');
        componentElement.classList.add(component.type);
        componentElement.id = component.id;

        const renderMethods = {
            heading: this.renderHeading,
            button: this.renderButton,
            doubleButton: this.renderDoubleButton,
            tripleButton: this.renderTripleButton,
            submenu: this.renderSubmenu,
            text: this.renderText,
            label: this.renderLabel,
            toggleInput: this.renderToggle,
            multiToggleInput: this.renderMultiToggle,
            url: this.renderUrl,
            spacer: this.renderSpacer,
            separator: this.renderSeparator,
            image: this.renderImage,
            datePickerInput: this.renderDatePicker,
            dropdownInput: this.renderDropdown,
            numberInput: this.renderNumberInput,
            textInput: this.renderTextInput,
            passwordInput: this.renderPasswordInput,
            textAreaInput: this.renderTextArea,
            sliderInput: this.renderSlider,
            colorPickerInput: this.renderColorPicker,
            ratingInput: this.renderRating,
            html: this.renderHTML,
            code: this.renderCodeBlock,
            list: this.renderList
        };

        const renderMethod = renderMethods[component.type] || this.renderDefault;
        return renderMethod.call(this, component, componentElement);
    }

    renderHeading(component, componentElement) {
        const headingElement = document.createElement(`h${component.level}`);
        headingElement.innerText = component.text;
        const textElement = document.createElement('p');
        textElement.innerText = component.subheading;
        componentElement.append(headingElement, textElement);
        return componentElement;
    }

    renderButton(component, componentElement) {
        const buttonElement = document.createElement("div");
        buttonElement.innerText = component.text;
        buttonElement.onclick = () => component.onClick(this);
        componentElement.appendChild(buttonElement);
        return componentElement;
    }

    renderDoubleButton(component, componentElement) {
        const button1= document.createElement("div");
        button1.innerText = component.text;
        button1.onclick = () => component.onClick(this);

        const button2 = document.createElement("div");
        button2.innerText = component.secondText;
        button2.onclick = () => component.secondOnClick(this);

        componentElement.append(button1, button2);
        return componentElement;
    }

    renderTripleButton(component, componentElement) {
        const button1 = document.createElement("div");
        button1.innerText = component.text;
        button1.onclick = () => component.onClick(this);

        const button2 = document.createElement("div");
        button2.innerText = component.secondText;
        button2.onclick = () => component.secondOnClick(this);

        const button3 = document.createElement("div");
        button3.innerText = component.thirdText;
        button3.onclick = () => component.thirdOnClick(this);

        componentElement.append(button1, button2, button3);
        return componentElement;
    }

    renderSubmenu(component, componentElement) {
        const subMenuElement = component.subModal.render();
        subMenuElement.setAttribute("showCondition", component.showCondition.replace("\"", "'"));
        componentElement.appendChild(subMenuElement);

        if (this.evaluateExpression(component.showCondition)) 
            componentElement.classList.remove("hidden");
        else 
            componentElement.classList.add("hidden");
        
        return componentElement;
    }

    renderText(component, componentElement) {
        const textElement = document.createElement('p');
        component.asHtml ? textElement.innerHTML = component.text : textElement.innerText = component.text;
        componentElement.appendChild(textElement);
        return componentElement;
    }

    renderLabel(component, componentElement) {
        const labelElement = document.createElement('label');
        labelElement.innerText = component.text;
        componentElement.appendChild(labelElement);
        return componentElement;
    }

    renderToggle(component, componentElement) {
        component.userSetValue = component.state.toString();
        
        const toggleElement = document.createElement('div');
        toggleElement.classList.add("toggle-switch");
        component.asCheckbox ? toggleElement.classList.add("as-checkbox") : null;
        if (component.state) toggleElement.classList.add("active");
        toggleElement.innerHTML = "<div class=\"switch\"></div>";

        toggleElement.addEventListener('click', () => {
            toggleElement.classList.toggle('active');
            const isActive = toggleElement.classList.contains('active');
            if (component.onChange) {
                component.onChange(this, isActive);
                component.userSetValue = isActive;
            }
        });

        const toggleLabelElement = document.createElement('div');
        toggleLabelElement.classList.add("toggle-label");
        toggleLabelElement.innerText = component.label;

        componentElement.appendChild(toggleElement);
        componentElement.appendChild(toggleLabelElement)
        return componentElement;
    }

    renderMultiToggle(component, componentElement) {
        //make a 2 column grid and add a toggle for each label (renderToggle)
        const toggleContainer = document.createElement('div');
        toggleContainer.classList.add('multi-toggle-container');

        component.userSetValue = component.states.map(state => state.toString());
        
        component.labels.forEach((label, index) => {
            const toggle = this.renderToggle({ type: 'toggleInput', label, state: component.states[index], asCheckbox: component.asCheckbox, onChange: (ctx, value) => {
                component.states[index] = value;
                if (component.onChange) {
                    component.onChange(this, component.states);
                    component.userSetValue = component.states;
                }
            }}, document.createElement('div'));
            toggleContainer.appendChild(toggle);
        });
        
        componentElement.appendChild(toggleContainer);
        
        return componentElement;
    }

    renderUrl(component, componentElement) {
        const urlSection = document.createElement('div');
        urlSection.className = 'url-section';

        const urlField = document.createElement('div');
        urlField.className = 'url-field';
        urlField.textContent = component.showFull ? component.url : (new URL(component.url).pathname + new URL(component.url).search);
        urlField.contentEditable = !component.copyButton;

        if (component.copyButton) {
            const copyButton = document.createElement('div');
            copyButton.className = 'copy button';
            copyButton.textContent = 'Kopieer';

            copyButton.addEventListener('click', () => {
                navigator.clipboard.writeText(component.url);
            });

            urlSection.appendChild(copyButton);
        }
        urlSection.appendChild(urlField);

        componentElement.appendChild(urlSection);
        return componentElement;
    }

    renderSpacer(component, componentElement) {
        componentElement.style.height = component.height;
        return componentElement;
    }

    renderSeparator(component, componentElement) {
        const separatorTextElement = document.createElement('p');
        const separatorLineElement = document.createElement('div');
        separatorTextElement.innerText = component.text;
        componentElement.appendChild(separatorTextElement);
        componentElement.appendChild(separatorLineElement);
        return componentElement;
    }

    renderImage(component, componentElement) {
        const imageElement = document.createElement('img');
        imageElement.src = component.src;
        imageElement.alt = component.alt;
        componentElement.appendChild(imageElement);
        return componentElement;
    }

    renderDatePicker(component, componentElement) {
        const datepicker = document.createElement('div');
        datepicker.className = 'datepicker-parent';

        const selectedDateElem = document.createElement('div');
        selectedDateElem.className = 'selected-date';
        selectedDateElem.textContent = component.initialDate?.toLocaleDateString("nl-NL", {weekday: 'long',year: 'numeric',month: 'long',day: 'numeric'}) ?? 'Kies een datum';
        if (component.initialDate) component.userSetValue = component.initialDate;
        datepicker.appendChild(selectedDateElem);

        const calendarElem = document.createElement('div');
        calendarElem.className = 'calendar';

        const calendarHeader = document.createElement('div');
        calendarHeader.className = 'calendar-header';
        calendarElem.appendChild(calendarHeader);

        const prevMonthButton = document.createElement('div');
        prevMonthButton.className = 'prev-month';
        prevMonthButton.textContent = '<';
        calendarHeader.appendChild(prevMonthButton);

        const monthYearElem = document.createElement('span');
        monthYearElem.className = 'month-year';
        calendarHeader.appendChild(monthYearElem);

        const nextMonthButton = document.createElement('div');
        nextMonthButton.className = 'next-month';
        nextMonthButton.textContent = '>';
        calendarHeader.appendChild(nextMonthButton);

        const calendarBody = document.createElement('div');
        calendarBody.className = 'calendar-body';
        calendarElem.appendChild(calendarBody);

        componentElement.appendChild(datepicker);
        componentElement.appendChild(calendarElem);

        let selectedDate = component.initialDate;
        let currentMonth = selectedDate.getMonth();
        let currentYear = selectedDate.getFullYear();

        const renderCalendar = (month, year) => {
            calendarBody.innerHTML = '';
            const firstDay = new Date(year, month).getDay();
            const daysInMonth = new Date(year, month + 1, 0).getDate();

            monthYearElem.textContent = new Date(year, month + 1, 0).toLocaleString('default', { year: 'numeric', month: 'long' });

            for (let i = 0; i < firstDay - 1; i++) {
                calendarBody.appendChild(document.createElement('div'));
            }

            for (let day = 1; day <= daysInMonth; day++) {
                const dayElem = document.createElement('div');
                dayElem.textContent = day;
                dayElem.classList.add('day');
                if (day === selectedDate.getDate() && month === selectedDate.getMonth() && year === selectedDate.getFullYear()) {
                    dayElem.classList.add('selected');
                }
                dayElem.addEventListener('click', () => {
                    selectedDate = new Date(year, month, day);
                    selectedDateElem.textContent = selectedDate.toLocaleDateString("nl-NL", {weekday: 'long',year: 'numeric',month: 'long',day: 'numeric'});
                    renderCalendar(currentMonth, currentYear);
                    if (component.onChange) {
                        component.onChange(this, selectedDate);
                        component.userSetValue = selectedDate;
                    }
                });
                calendarBody.appendChild(dayElem);
            }
        };

        datepicker.addEventListener('click', () => {
            datepicker.classList.toggle("selected");
            calendarElem.classList.toggle("show");
        });

        prevMonthButton.addEventListener('click', () => {
            if (currentMonth === 0) {
                currentMonth = 11;
                currentYear--;
            } else {
                currentMonth--;
            }
            renderCalendar(currentMonth, currentYear);
        });

        nextMonthButton.addEventListener('click', () => {
            if (currentMonth === 11) {
                currentMonth = 0;
                currentYear++;
            } else {
                currentMonth++;
            }
            renderCalendar(currentMonth, currentYear);
        });

        renderCalendar(currentMonth, currentYear);

        return componentElement;
    }

    renderDropdown(component, componentElement) {

        const dropdownElement = document.createElement('div');
        dropdownElement.classList.add('dropdown');

        const dropdownButton = document.createElement('div');
        dropdownButton.classList.add('dropdown-button');
        dropdownButton.innerText = 'Select...';

        const dropdownMenu = document.createElement('div');
        dropdownMenu.classList.add('dropdown-menu');

        component.options.forEach(option => {
            const optionElement = document.createElement('div');
            optionElement.classList.add('dropdown-option');
            optionElement.innerText = option.label;
            optionElement.dataset.value = option.value;
            optionElement.dataset.label = option.label;

            optionElement.addEventListener('click', () => {
                if (component.multiSelect) {
                    optionElement.classList.toggle('selected');
                } else {
                    Array.from(dropdownMenu.children).forEach(child => child.classList.remove('selected'));
                    optionElement.classList.add('selected');
                    dropdownButton.click();
                }
                const selectedOptions = Array.from(dropdownMenu.children)
                    .filter(child => child.classList.contains('selected'))
                    .map(child => child.dataset);
                
                const selectedOptionsLabels = Array.from(dropdownMenu.children)
                    .filter(child => child.classList.contains('selected'))
                    .map(child => child.dataset.label);

                dropdownButton.innerText = selectedOptionsLabels.length ? selectedOptionsLabels.join(", ") : "Select...";

                if (component.onChange) {
                    component.onChange(this, component.multiSelect ? selectedOptions : selectedOptions[0]);
                    component.userSetValue = component.multiSelect ? selectedOptions : selectedOptions[0];
                }
            });

            dropdownMenu.appendChild(optionElement);
        });

        dropdownButton.addEventListener('click', () => {
            dropdownButton.classList.toggle("selected");
            dropdownMenu.classList.toggle("show");
        });

        dropdownElement.appendChild(dropdownButton);
        dropdownElement.appendChild(dropdownMenu);

        componentElement.appendChild(dropdownElement);
        return componentElement;
    }

    renderNumberInput(component, componentElement) {
        const numberInput = document.createElement('div');
        numberInput.className = 'number-input';

        const decrementButton = document.createElement('div');
        decrementButton.className = 'decrement-button';
        decrementButton.textContent = '-';

        const numberDisplay = document.createElement('span');
        numberDisplay.contentEditable = 'true';
        numberDisplay.className = 'display';

        const incrementButton = document.createElement('div');
        incrementButton.className = 'increment-button';
        incrementButton.textContent = '+';

        numberInput.append(decrementButton, numberDisplay, incrementButton);

        let value = component.initialValue;

        const updateDisplay = () => {
            numberDisplay.textContent = value.toFixed(component.decimals);
            if (component.onChange) {
                component.onChange(this, value);
                component.userSetValue = value;
            }
        };

        const increment = () => {
            if (value + component.step <= component.max) {
                value += component.step;
                updateDisplay();
            }
        };

        const decrement = () => {
            if (value - component.step >= component.min) {
                value -= component.step;
                updateDisplay();
            }
        };

        const validateAndSetInput = (inputValue) => {
            let parsedValue = parseFloat(inputValue.replace(',', '.'));
            if (!isNaN(parsedValue) && parsedValue >= component.min && parsedValue <= component.max) {
                value = parsedValue;
                updateDisplay();
            }
        };

        incrementButton.addEventListener("click", increment);
        decrementButton.addEventListener("click", decrement);

        numberDisplay.addEventListener("input", (e) => validateAndSetInput(e.target.textContent));
        numberDisplay.addEventListener("keypress", (e) => {
            if (!/[0-9,.]/.test(String.fromCharCode(e.which))) {
                e.preventDefault();
            }
        });

        updateDisplay();
        componentElement.appendChild(numberInput);
        return componentElement;
    }

    renderTextInput(component, componentElement) {
        const textElementInput = document.createElement('div');
        textElementInput.className = 'text-element-input';

        const textDisplay = document.createElement('div');
        textDisplay.contentEditable = 'true';
        textDisplay.className = 'display';
        textDisplay.textContent = component.initialValue;

        textElementInput.append(textDisplay);

        const handleInput = (e) => {
            if (component.maxLength && e.target.innerText.length > component.maxLength) {
                e.target.innerText = e.target.innerText.slice(0, component.maxLength);
            }
            if (component.onChange) {
                component.onChange(this, e.target.innerText);
                component.userSetValue = e.target.innerText;
            }
        };

        textDisplay.addEventListener("input", handleInput);

        componentElement.appendChild(textElementInput);
        return componentElement;
    }

    renderPasswordInput(component, componentElement) {
        const passwordInput = document.createElement('div');
        passwordInput.className = 'password-element-input';

        const textDisplay = document.createElement('div');
        textDisplay.contentEditable = 'true';
        textDisplay.className = 'display';

        const eyeIcon = document.createElement('span');
        eyeIcon.innerText = "👀";
        eyeIcon.className = 'eye-icon';

        passwordInput.append(textDisplay, eyeIcon);

        let password = '';
        let showPassword = false;

        const updateDisplay = () => {
            textDisplay.innerText = showPassword ? password : '•'.repeat(password.length);
            if (component.onChange) {
                component.onChange(this, password);
                component.userSetValue = password;
            }
        };

        const handleKeydown = (e) => {
            let sel = window.getSelection();
            let pos = sel.focusOffset;

            if (e.key === "Backspace" && pos > 0) {
                password = password.slice(0, pos - 1) + password.slice(pos);
            } else if (e.key.length === 1 && (!component.maxLength || password.length < component.maxLength)) {
                password = password.slice(0, pos) + e.key + password.slice(pos);
            } else if (e.key !== "ArrowLeft" && e.key !== "ArrowRight") {
                e.preventDefault();
            }

            updateDisplay();
            this.setCursorPosition(textDisplay, pos + (e.key === "Backspace" ? -1 : 1));
            e.preventDefault();
        };

        eyeIcon.addEventListener('click', () => {
            showPassword = !showPassword;
            updateDisplay();
        });

        textDisplay.addEventListener("keydown", handleKeydown);

        componentElement.appendChild(passwordInput);
        return componentElement;
    }

    renderTextArea(component, componentElement) {
        const textAreaElementInput = document.createElement('div');
        textAreaElementInput.className = 'textarea-element-input';

        const textDisplay = document.createElement('span');
        textDisplay.contentEditable = 'true';
        textDisplay.className = 'display';
        textDisplay.role = 'textbox';
        textDisplay.textContent = component.initialValue;

        textAreaElementInput.append(textDisplay);

        const handleInput = (e) => {
            if (component.maxLength && e.target.innerText.length > component.maxLength) {
                e.target.innerText = e.target.innerText.slice(0, component.maxLength);
            }
            if (component.onChange) {
                component.onChange(this, e.target.innerText);
                component.userSetValue = e.target.innerText;
            }
        };

        textDisplay.addEventListener("input", handleInput);

        componentElement.appendChild(textAreaElementInput);
        return componentElement;
    }

    renderSlider(component, componentElement) {
        const sliderContainer = document.createElement('div');
        sliderContainer.className = 'slider-container';

        const sliderTrack = document.createElement('div');
        sliderTrack.className = 'slider-track';

        const sliderThumb = document.createElement('div');
        sliderThumb.className = 'slider-thumb';

        const valueDisplay = document.createElement('span');
        valueDisplay.className = 'slider-value';
        valueDisplay.textContent = component.initialValue;

        sliderTrack.appendChild(sliderThumb);
        sliderContainer.append(sliderTrack, valueDisplay);

        const min = component.min;
        const max = component.max;
        const range = max - min;

        // Set initial position
        const initialPercentage = ((component.initialValue - min) / range) * 100;
        sliderThumb.style.left = `${initialPercentage}%`;

        let isDragging = false;

        const updateSliderValue = (clientX) => {
            const rect = sliderTrack.getBoundingClientRect();
            let percentage = (clientX - rect.left) / rect.width;
            percentage = Math.max(0, Math.min(percentage, 1));

            const value = min + percentage * range;
            const roundedValue = Math.round(value / component.step) * component.step;

            sliderThumb.style.left = `${percentage * 100}%`;
            valueDisplay.textContent = roundedValue.toFixed(getDecimalPlaces(component.step));

            if (component.onChange) {
                component.onChange(this, roundedValue);
                component.userSetValue = roundedValue;
            }
        };

        sliderTrack.addEventListener('mousedown', (e) => {
            isDragging = true;
            applyNoSelectionClass(true); // Apply no-selection class
            updateSliderValue(e.clientX);
        });

        document.addEventListener('mousemove', (e) => {
            if (isDragging) {
                updateSliderValue(e.clientX);
            }
        });

        document.addEventListener('mouseup', () => {
            isDragging = false;
            applyNoSelectionClass(false); // Apply no-selection class
        });

        componentElement.appendChild(sliderContainer);
        return componentElement;

        function getDecimalPlaces(num) {
            // Ensure num is a floating-point number
            let floatNum = parseFloat(num);
            if (Math.floor(floatNum) === floatNum) return 0; // Check if the number is an integer

            let decimalPart = floatNum.toString().split('.')[1];

            // Trim any trailing zeroes for a more sensible decimal count
            if (decimalPart) {
                decimalPart = decimalPart.replace(/0+$/, '');
                return Math.min(decimalPart.length, 2);
            } else {
                return 0;
            }
        }

        // Add this function to apply and remove the no-selection CSS class
        function applyNoSelectionClass(apply) {
            if (apply) {
                document.body.classList.add('no-select');
            } else {
                document.body.classList.remove('no-select');
            }
        }
    }

    renderColorPicker(component, componentElement) {
        const colorPickerContainer = document.createElement('div');
        colorPickerContainer.className = 'color-picker-container';

        const colorDisplay = document.createElement('div');
        colorDisplay.className = 'color-display';
        colorDisplay.style.backgroundColor = component.initialColor;

        const colorPalette = document.createElement('div');
        colorPalette.className = 'color-palette';
        colorPalette.style.display = "none";

        const hexInput = document.createElement('div');
        hexInput.className = 'hex-input';
        hexInput.contentEditable = 'true';
        hexInput.textContent = component.initialColor;

        const rgbInputs = ['R', 'G', 'B'].map(channel => {
            const input = document.createElement('div');
            input.className = 'rgb-input';
            input.contentEditable = 'true';
            input.textContent = '0';
            return input;
        });

        const colorCircle = document.createElement('canvas');
        colorCircle.className = 'color-circle';
        colorCircle.width = 150;
        colorCircle.height = 150;

        const ctx = colorCircle.getContext('2d');

// Create a conic gradient
        const gradient = ctx.createConicGradient(0, 75, 75);

// Add five color stops
        gradient.addColorStop(0, "red");
        gradient.addColorStop(1/6, "yellow");
        gradient.addColorStop(2/6, "lime");
        gradient.addColorStop(3/6, "cyan");
        gradient.addColorStop(4/6, "blue");
        gradient.addColorStop(5/6, "magenta");
        gradient.addColorStop(1, "red");

// Set the fill style and draw a rectangle
        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, 150, 150);

        const colorSelector = document.createElement('div');
        colorSelector.className = 'color-selector';

        const updateColorDisplay = (color) => {
            colorDisplay.style.backgroundColor = color;
            if (component.onChange) {
                component.onChange(this, color);
                component.userSetValue = color;
            }
        };

        const hexToRgb = (hex) => {
            const bigint = parseInt(hex.slice(1), 16);
            return {
                r: (bigint >> 16) & 255,
                g: (bigint >> 8) & 255,
                b: bigint & 255
            };
        };

        const rgbToHex = (r, g, b) => {
            return `#${((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1).toUpperCase()}`;
        };

        const setColorFromHex = (hex) => {
            const { r, g, b } = hexToRgb(hex);
            rgbInputs[0].textContent = r;
            rgbInputs[1].textContent = g;
            rgbInputs[2].textContent = b;
            updateColorDisplay(hex);
        };

        const setColorFromRgb = () => {
            const r = parseInt(rgbInputs[0].textContent, 10) || 0;
            const g = parseInt(rgbInputs[1].textContent, 10) || 0;
            const b = parseInt(rgbInputs[2].textContent, 10) || 0;
            const hex = rgbToHex(r, g, b);
            hexInput.textContent = hex;
            updateColorDisplay(hex);
        };

        hexInput.addEventListener('input', (e) => {
            const hex = e.target.textContent.trim();
            if (/^#[0-9A-F]{6}$/i.test(hex)) {
                setColorFromHex(hex);
            }
        });

        rgbInputs.forEach(input => {
            input.addEventListener('input', setColorFromRgb);
        });

        colorDisplay.addEventListener('click', () => {
            colorPalette.style.display = colorPalette.style.display === 'none' ? 'block' : 'none';
        });

        document.addEventListener('click', (event) => {
            if (!colorPickerContainer.contains(event.target)) {
                colorPalette.style.display = 'none';
            }
        });

        colorCircle.addEventListener('click', (e) => {
            const rect = colorCircle.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const pixel = ctx.getImageData(x, y, 1, 1).data;
            const hex = rgbToHex(pixel[0], pixel[1], pixel[2]);
            setColorFromHex(hex);
            colorSelector.style.left = `${x - 5}px`;
            colorSelector.style.top = `${y - 5}px`;
        });

        const { r, g, b } = hexToRgb(component.initialColor);
        rgbInputs[0].textContent = r;
        rgbInputs[1].textContent = g;
        rgbInputs[2].textContent = b;

        colorCircle.appendChild(colorSelector);
        colorPalette.append(hexInput, ...rgbInputs, colorCircle);
        colorPickerContainer.append(colorDisplay, colorPalette);
        componentElement.appendChild(colorPickerContainer);
        return componentElement;
    }

    renderDefault(component, componentElement) {
        componentElement.innerText = `couldn't render component of type: ${component.type}`;
        return componentElement;
    }

    renderRating(component, componentElement) {
        const ratingContainer = document.createElement('div');
        ratingContainer.className = 'rating-container';

        for (let i = 1; i <= component.maxRating; i++) {
            const star = document.createElement('span');
            star.textContent = '☆';
            star.dataset.value = i;

            if (i <= component.initialRating) {
                star.textContent = '★';
            }

            star.addEventListener('click', (e) => {
                const value = parseInt(e.target.dataset.value);
                ratingContainer.querySelectorAll('span').forEach((s, index) => {
                    s.textContent = index < value ? '★' : '☆';
                });
                if (component.onChange) {
                    component.onChange(this, value);
                    component.userSetValue = value;
                }
            });

            ratingContainer.appendChild(star);
        }

        componentElement.append(ratingContainer);
        return componentElement;
    }
    
    renderHTML(component, componentElement) {
        const htmlElement = document.createElement('div');
        htmlElement.innerHTML = component.html;
        componentElement.appendChild(htmlElement);
        return componentElement;
    }
    
    renderCodeBlock(component, componentElement) {
        const codeBlock = document.createElement('pre');
        codeBlock.innerText = component.code;

        //replace every <br><br> with <br>, and every <br><br><br><br> with <br><br>
        codeBlock.innerHTML = codeBlock.innerHTML.replace(/<br><br>/g, "<br>").replace(/<br><br><br><br>/g, "<br><br>").replace(/<br><br><br><br><br><br>/g, "<br><br>");
        
        //remove any leading or trailing <br> tags
        codeBlock.innerHTML = codeBlock.innerHTML.replace(/^<br>/, "").replace(/<br>$/, "");
        
        componentElement.appendChild(codeBlock);
        return componentElement;
    }

    renderList(component, componentElement) {
        const listElement = document.createElement(component.listType);
        component.items.forEach(item => {
            const listItem = document.createElement('li');
            listItem.innerText = item;
            listElement.appendChild(listItem);
        });
        componentElement.appendChild(listElement);
        return componentElement;
    }

    setCursorPosition(contentEditableElement, position) {
        position = Math.max(0, Math.min(position, contentEditableElement.textContent.length));
        const range = document.createRange();
        const sel = window.getSelection();
        range.setStart(contentEditableElement.childNodes[0] || contentEditableElement, position);
        range.collapse(true);
        sel.removeAllRanges();
        sel.addRange(range);
    }

    evaluateExpression(expression) {
        const comparisonMatch = expression.match(/^(\w+)\s*(==|!=|>=|<=|>|<)\s*(.+)$/);
        if (!comparisonMatch) return false;

        const [_, key, operator, value] = comparisonMatch;
        const conditionValue = this.conditions[key];
        const comparedValue = this.parseValue(value);

        const operations = {
            '==': (a, b) => a === b,
            '!=': (a, b) => a !== b,
            '>': (a, b) => a > b,
            '<': (a, b) => a < b,
            '>=': (a, b) => a >= b,
            '<=': (a, b) => a <= b
        };

        return operations[operator](conditionValue, comparedValue);
    }

    parseValue(value) {
        if (value === 'true') return true;
        if (value === 'false') return false;
        if (!isNaN(value)) return parseFloat(value);
        if (/^["'].*["']$/.test(value)) return value.slice(1, -1);
        throw new Error('Unsupported value type:' + value);
    }

    open() {
        const modalElement = this.render();
        
        const modalBackground = document.createElement('div');
        
        modalBackground.className = 'zermos-modal-background';
        if (!this.disableClosing) {
            modalBackground.addEventListener('click', (e) => {
                if (e.target === modalBackground) this.close();
            });
        }
        
        modalBackground.style.opacity = 0;
        
        //.zermos-modal-container
        const modalContainer = document.createElement('div');
        modalContainer.className = 'zermos-modal-container';

        modalContainer.appendChild(modalElement);
        modalBackground.appendChild(modalContainer);
        document.body.appendChild(modalBackground);

        ease(0, 1, 250, opacity => modalBackground.style.opacity = opacity);

        this.modalElement = modalBackground;
        this.openingType = "opening";
        return this;
    }
    
    openTroughAppending(element) {        
        const modalElement = this.render();
        
        var centeringDiv = document.createElement('div');
        centeringDiv.style.display = "flex";
        centeringDiv.style.justifyContent = "center";
        centeringDiv.style.alignItems = "center";
        centeringDiv.style.height = "100%";
        centeringDiv.style.width = "100%";

        centeringDiv.appendChild(modalElement);
        element.appendChild(centeringDiv);
        
        this.modalElement = modalElement;
        this.openingType = "appending";
        return this;
    }
    
    //close w
    close() {
        if (this.openingType === "opening") {
            ease(1, 0, 250, opacity => this.modalElement.style.opacity = opacity);
            setTimeout(() => this.modalElement.remove(), 300);
        }
        
        if (this.openingType === "appending") {
            this.modalElement.parentElement.remove();
        }
    }
    
    closeAll() {
        document.querySelectorAll('.zermos-modal-background').forEach(modal => modal.remove());
    }

    deepCopy() {
        const copiedComponents = this.components.map(component => ({...component}));
        const copiedConditions = {...this.conditions};
        return new ZermosModal(this.disableClosing, copiedComponents, copiedConditions);
    }
}


// Subclass for submenus, can override or add specific submenu behavior
class ZermosSubModal extends ZermosModal {}

window.unloadModal = () => {
    var modal = document.querySelector('.zermos-modal-background');
    if (modal) {
        ease(1, 0, 250, opacity => modal.style.opacity = opacity);
        setTimeout(() => modal.remove(), 300);
    }
}

function ease(start, end, time, callback) {
    start = Number(start);
    end = Number(end);
    const startTime = Date.now();
    const duration = time;

    function update() {
        const currentTime = Date.now();
        const elapsed = currentTime - startTime;

        if (elapsed >= duration) {
            callback(end);
        } else {
            const progress = elapsed / duration;
            const easedValue = start + (end - start) * (progress * (2 - progress));
            callback(easedValue);
            requestAnimationFrame(update);
        }
    }

    requestAnimationFrame(update);
}
