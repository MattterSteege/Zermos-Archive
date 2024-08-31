class ZermosModal {
    constructor(disableClosing = false, components = [], conditions = {}) {
        this.disableClosing = disableClosing;
        this.components = components;
        this.conditions = conditions;
        this.tabIndex = 0;
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

    addButton({text, onClick = () => {}, id = undefined}) {
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

    addTimePicker({required = false, initialTime = undefined, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'timePickerInput', required, initialTime, onChange, id: id});
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

    addTextInput({required = false, initialValue = '', maxLength = -1, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'textInput', required, initialValue, maxLength, onChange, id: id});
    }

    addPasswordInput({required = false, maxLength = -1, onChange = () => {}, id = undefined}) {
        return this.addComponent({ type: 'passwordInput', required, maxLength, onChange, id: id});
    }

    addTextArea({required = false, initialValue = '', maxLength = -1, onChange = () => {}, id = undefined}) {
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
            timePickerInput: this.renderTimePicker,
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
        const buttonElement = document.createElement("button");
        buttonElement.innerText = component.text;
        buttonElement.onclick = () => component.onClick(this);
        componentElement.appendChild(buttonElement);
        return componentElement;
    }

    renderDoubleButton(component, componentElement) {
        const button1= document.createElement("button");
        button1.innerText = component.text;
        button1.onclick = () => component.onClick(this);

        const button2 = document.createElement("button");
        button2.innerText = component.secondText;
        button2.onclick = () => component.secondOnClick(this);

        componentElement.append(button1, button2);
        return componentElement;
    }

    renderTripleButton(component, componentElement) {
        const button1 = document.createElement("button");
        button1.innerText = component.text;
        button1.onclick = () => component.onClick(this);

        const button2 = document.createElement("button");
        button2.innerText = component.secondText;
        button2.onclick = () => component.secondOnClick(this);

        const button3 = document.createElement("button");
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

        // Create the input element of type checkbox
        const toggleElement = document.createElement('input');
        toggleElement.type = 'checkbox';
        toggleElement.classList.add('toggle-switch');
        toggleElement.checked = component.state;
        toggleElement.setAttribute('role', 'switch');
        toggleElement.setAttribute('aria-checked', component.state ? 'true' : 'false');
        toggleElement.setAttribute('tabindex', this.getTabIndex());

        component.asCheckbox ? toggleElement.classList.add('as-checkbox') : null;

        toggleElement.addEventListener('change', () => {
            const isActive = toggleElement.checked;
            toggleElement.setAttribute('aria-checked', isActive ? 'true' : 'false');
            if (component.onChange) {
                component.onChange(this, isActive);
                component.userSetValue = isActive;
            }
        });

        // Create the label element and associate it with the input element
        const toggleLabelElement = document.createElement('label');
        toggleLabelElement.classList.add('toggle-label');
        
        const toggleLabelText = document.createElement('p');
        toggleLabelText.innerText = component.label;
        toggleLabelText.setAttribute('for', toggleElement.id);

        componentElement.appendChild(toggleLabelText);

        // Append the elements to the component element
        componentElement.appendChild(toggleElement);
        componentElement.appendChild(toggleLabelElement);

        return componentElement;
    }

    renderMultiToggle(component, componentElement) {
        //make a 2 column grid and add a toggle for each label (renderToggle)
        const toggleContainer = document.createElement('div');
        toggleContainer.classList.add('multi-toggle-container');
        toggleContainer.setAttribute('role', 'group');
        toggleContainer.setAttribute('aria-label', 'Multi Toggle Options');
        toggleContainer.setAttribute('tabindex', this.getTabIndex());

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

        component.url = window.location.origin + component.url;
        
        const urlField = document.createElement('div');
        urlField.className = 'url-field';
        urlField.textContent = component.showFull ? component.url : (new URL(component.url).pathname + new URL(component.url).search);
        urlField.contentEditable = !component.copyButton;
        urlField.setAttribute('role', 'link');
        urlField.setAttribute('tabindex', this.getTabIndex());

        if (component.copyButton) {
            const copyButton = document.createElement('button');
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
        imageElement.setAttribute('role', 'img');
        imageElement.setAttribute('aria-label', component.alt);
        
        componentElement.appendChild(imageElement);
        return componentElement;
    }

    renderDatePicker(component, componentElement) {
        const datepicker = document.createElement('div');
        datepicker.className = 'datepicker-parent';
        datepicker.setAttribute('role', 'combobox');
        datepicker.setAttribute('aria-expanded', 'false');
        datepicker.setAttribute('aria-haspopup', 'grid');
        datepicker.setAttribute('tabindex', this.getTabIndex());

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

        const prevMonthButton = document.createElement('button');
        prevMonthButton.className = 'prev-month';
        prevMonthButton.textContent = '<';
        calendarHeader.appendChild(prevMonthButton);

        const monthYearElem = document.createElement('span');
        monthYearElem.className = 'month-year';
        calendarHeader.appendChild(monthYearElem);

        const nextMonthButton = document.createElement('button');
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
                const dayElem = document.createElement('button');
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
            datepicker.setAttribute('aria-expanded', datepicker.classList.contains('selected') ? 'true' : 'false');
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

    renderTimePicker(component, componentElement) {
        const timepicker = document.createElement('div');
        timepicker.className = 'timepicker-parent';
        timepicker.setAttribute('role', 'combobox');
        timepicker.setAttribute('aria-expanded', 'false');
        timepicker.setAttribute('aria-haspopup', 'grid');
        timepicker.setAttribute('tabindex', this.getTabIndex());

        const selectedTimeElem = document.createElement('div');
        selectedTimeElem.className = 'selected-time';
        selectedTimeElem.textContent = component.initialTime ?? 'Kies een tijd';
        if (component.initialTime) component.userSetValue = component.initialTime;
        timepicker.appendChild(selectedTimeElem);

        const timepickerElem = document.createElement('div');
        timepickerElem.className = 'timepicker';

        const hoursElem = document.createElement('select');
        hoursElem.className = 'hours';
        for (let i = 0; i < 24; i++) {
            const option = document.createElement('option');
            option.value = i;
            option.textContent = i.toString().padStart(2, '0');
            hoursElem.appendChild(option);
        }

        const minutesElem = document.createElement('select');
        minutesElem.className = 'minutes';
        for (let i = 0; i < 60; i++) {
            const option = document.createElement('option');
            option.value = i;
            option.textContent = i.toString().padStart(2, '0');
            minutesElem.appendChild(option);
        }

        timepickerElem.append(hoursElem, minutesElem);
        componentElement.appendChild(timepicker);
        componentElement.appendChild(timepickerElem);

        let selectedTime = component.initialTime ? component.initialTime.split(':') : [0, 0];
        hoursElem.value = selectedTime[0];
        minutesElem.value = selectedTime[1];

        hoursElem.addEventListener('change', () => {
            selectedTime[0] = hoursElem.value.toString().padStart(2, '0');
            selectedTime[1] = minutesElem.value.toString().padStart(2, '0');
            selectedTimeElem.textContent = `${selectedTime[0].padStart(2, '0')}:${selectedTime[1].padStart(2, '0')}`;
            if (component.onChange) {
                component.onChange(this, selectedTime.join(':'));
                component.userSetValue = selectedTime.join(':');
            }
        });

        minutesElem.addEventListener('change', () => {
            selectedTime[0] = hoursElem.value.toString().padStart(2, '0');
            selectedTime[1] = minutesElem.value.toString().padStart(2, '0');
            selectedTimeElem.textContent = `${selectedTime[0].padStart(2, '0')}:${selectedTime[1].padStart(2, '0')}`;
            if (component.onChange) {
                component.onChange(this, selectedTime.join(':'));
                component.userSetValue = selectedTime.join(':');
            }
        });
        
        timepicker.addEventListener('click', () => {
            timepicker.classList.toggle("selected");
            timepickerElem.classList.toggle("show");
            timepicker.setAttribute('aria-expanded', timepicker.classList.contains('selected') ? 'true' : 'false');
        });
        
        return componentElement;
    }

    renderDropdown(component, componentElement) {

        const dropdownElement = document.createElement('div');
        dropdownElement.classList.add('dropdown');
        dropdownElement.setAttribute('role', 'combobox');
        dropdownElement.setAttribute('aria-expanded', 'false');

        const dropdownButton = document.createElement('button');
        dropdownButton.classList.add('dropdown-button');
        dropdownButton.innerText = 'Select...';
        dropdownButton.setAttribute('tabindex', this.getTabIndex());

        const dropdownMenu = document.createElement('div');
        dropdownMenu.classList.add('dropdown-menu');
        dropdownMenu.setAttribute('role', 'listbox');

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
            dropdownElement.setAttribute('aria-expanded', dropdownButton.classList.contains('selected') ? 'true' : 'false');
        });

        dropdownElement.appendChild(dropdownButton);
        dropdownElement.appendChild(dropdownMenu);

        componentElement.appendChild(dropdownElement);
        return componentElement;
    }

    renderNumberInput(component, componentElement) {
        const numberInput = document.createElement('input');
        numberInput.type = 'number';
        numberInput.className = 'number-input';
        numberInput.setAttribute('min', component.min);
        numberInput.setAttribute('max', component.max);
        numberInput.setAttribute('step', component.step);
        numberInput.setAttribute('value', component.initialValue.toFixed(component.decimals));
        numberInput.setAttribute('tabindex', this.getTabIndex());

        const updateValue = (newValue) => {
            const value = parseFloat(newValue);
            if (!isNaN(value) && value >= component.min && value <= component.max) {
                numberInput.value = value.toFixed(component.decimals);
                if (component.onChange) {
                    component.onChange(this, value);
                    component.userSetValue = value;
                }
            }
        };

        numberInput.addEventListener('input', (e) => updateValue(e.target.value));
        numberInput.addEventListener('change', (e) => updateValue(e.target.value));

        componentElement.appendChild(numberInput);
        return componentElement;
    }

    renderTextInput(component, componentElement) {
        const inputElement = document.createElement('input');
        inputElement.type = 'text';
        inputElement.className = 'text-element-input display';
        inputElement.value = component.initialValue;
        if (component.maxLength !== -1)
            inputElement.maxLength = component.maxLength;
        
        inputElement.tabIndex = this.getTabIndex();

        inputElement.addEventListener('input', (e) => {
            if (component.onChange) {
                component.onChange(this, e.target.value);
                component.userSetValue = e.target.value;
            }
        });

        componentElement.appendChild(inputElement);
        return componentElement;
    }

    renderPasswordInput(component, componentElement) {
        const inputElement = document.createElement('input');
        inputElement.type = 'password';
        inputElement.className = 'password-element-input display';
        inputElement.tabIndex = this.getTabIndex();
        if (component.maxLength !== -1)
            inputElement.maxLength = component.maxLength;

        const eyeIcon = document.createElement('div');
        eyeIcon.innerText = "👀";
        eyeIcon.className = 'eye-icon';
        eyeIcon.setAttribute('aria-label', 'Toggle password visibility');
        eyeIcon.tabIndex = 0;

        let showPassword = false;

        eyeIcon.addEventListener('click', () => {
            showPassword = !showPassword;
            inputElement.type = showPassword ? 'text' : 'password';
        });

        inputElement.addEventListener('input', (e) => {
            if (component.onChange) {
                component.onChange(this, e.target.value);
                component.userSetValue = e.target.value;
            }
        });

        componentElement.appendChild(inputElement);
        componentElement.appendChild(eyeIcon);
        return componentElement;
    }

    renderTextArea(component, componentElement) {
        const textareaElement = document.createElement('textarea');
        textareaElement.className = 'textarea-element-input display';
        textareaElement.value = component.initialValue;
        if (component.maxLength !== -1)
            textareaElement.maxLength = component.maxLength;
        
        textareaElement.tabIndex = this.getTabIndex();

        textareaElement.addEventListener('input', (e) => {
            if (component.onChange) {
                component.onChange(this, e.target.value);
                component.userSetValue = e.target.value;
            }
        });

        componentElement.appendChild(textareaElement);
        return componentElement;
    }

    renderSlider(component, componentElement) {
        const sliderContainer = document.createElement('div');
        sliderContainer.className = 'slider-container';

        const sliderInput = document.createElement('input');
        sliderInput.type = 'range';
        sliderInput.className = 'slider-input';
        sliderInput.min = component.min;
        sliderInput.max = component.max;
        sliderInput.step = component.step;
        sliderInput.value = component.initialValue;

        const valueDisplay = document.createElement('span');
        valueDisplay.className = 'slider-value';
        valueDisplay.textContent = component.initialValue;

        sliderInput.addEventListener('input', () => {
            const roundedValue = Math.round(sliderInput.value / component.step) * component.step;
            valueDisplay.textContent = roundedValue.toFixed(getDecimalPlaces(component.step));

            if (component.onChange) {
                component.onChange(this, roundedValue);
                component.userSetValue = roundedValue;
            }
        });

        sliderContainer.append(sliderInput, valueDisplay);
        componentElement.appendChild(sliderContainer);
        return componentElement;

        function getDecimalPlaces(num) {
            let floatNum = parseFloat(num);
            if (Math.floor(floatNum) === floatNum) return 0;

            let decimalPart = floatNum.toString().split('.')[1];
            if (decimalPart) {
                decimalPart = decimalPart.replace(/0+$/, '');
                return Math.min(decimalPart.length, 2);
            } else {
                return 0;
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

        const hexInput = document.createElement('input');
        hexInput.className = 'hex-input';
        hexInput.type = 'text';
        hexInput.value = component.initialColor;

        const rgbInputs = ['R', 'G', 'B'].map(channel => {
            const input = document.createElement('input');
            input.className = 'rgb-input';
            input.type = 'number';
            input.value = '0';
            input.min = '0';
            input.max = '255';
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
            rgbInputs[0].value = r;
            rgbInputs[1].value = g;
            rgbInputs[2].value = b;
            updateColorDisplay(hex);
        };

        const setColorFromRgb = () => {
            const r = parseInt(rgbInputs[0].value, 10) || 0;
            const g = parseInt(rgbInputs[1].value, 10) || 0;
            const b = parseInt(rgbInputs[2].value, 10) || 0;
            const hex = rgbToHex(r, g, b);
            hexInput.value = hex;
            updateColorDisplay(hex);
        };

        hexInput.addEventListener('input', (e) => {
            const hex = e.target.value.trim();
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
        rgbInputs[0].value = r;
        rgbInputs[1].value = g;
        rgbInputs[2].value = b;

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
            const starInput = document.createElement('input');
            starInput.type = 'radio';
            starInput.name = 'rating';
            starInput.id = `star-${i}`;
            starInput.value = i;
            starInput.style.display = 'none';

            const starLabel = document.createElement('label');
            starLabel.htmlFor = `star-${i}`;
            starLabel.textContent = i <= component.initialRating ? '★' : '☆';

            starInput.addEventListener('change', () => {
                ratingContainer.querySelectorAll('label').forEach((label, index) => {
                    label.textContent = index < starInput.value ? '★' : '☆';
                });
                if (component.onChange) {
                    component.onChange(this, i);
                    component.userSetValue = i;
                }
            });

            ratingContainer.appendChild(starInput);
            ratingContainer.appendChild(starLabel);
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

    getTabIndex() {
        return 1000 + this.tabIndex++;
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
        if (!this.disableClosing) {
            modalContainer.addEventListener('click', (e) => {
                if (e.target === modalContainer) this.close();
            });
        }
        
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