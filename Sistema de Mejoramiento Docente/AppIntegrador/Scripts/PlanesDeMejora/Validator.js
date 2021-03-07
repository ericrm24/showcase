// Class that validates the "Planes de Mejora" form
class Validador {
    _maxNameCharacters = -1;
    _maxDescriptionCharacters = -1;

    _minDate = Date.now();
    _maxDate = Date.now();

    _minDateStr = '';
    _maxDateStr = '';

    _minDateNumber = -1;
    _maxDateNumber = -1;

    _validDateMsj = '';
    _invalidDateMsj = '';
    _endMsj = '.';
    _remainingLettersMsj = '';

    _submitBtnId = '';
    _validSubmitForm = false;
    _initialDateTooLate = '';
    _tooSoonFinalDate = '';
    _tooSoonAnyDateCompareToMinimum = '';
    _validDateMsj = '';
    _defaultStartDateMsj = '';
    _defaultFinalDateMsj = '';

    _totalValidations = 0;
    _arrOfValidations = [];

    /*
     EFE:
        Construct an instance of the Validator class
     REQ:
               maxCaracName: max amount of characters for the name.
        maxCaracDescription: max amount of characters for the description.
                    minDate: minimum date that the validator will accept.
                submitBtnId: id of the button of submit that the validator will manage.
     MOD:
        ---
     */
    constructor(maxCaracName, maxCaracDescription, minDate, maxDate, submitBtnId) {
        this._maxNameCharacters = maxCaracName;
        this._maxDescriptionCharacters = maxCaracDescription;

        this._minDate = minDate;
        this._maxDate = maxDate;

        this._minDateStr = this.setStrDate(this._minDate);
        this._maxDateStr = this.setStrDate(this._maxDate);

        this._minDateNumber = this.getIntOfDate(this._minDateStr);
        this._maxDateNumber = this.getIntOfDate(this._maxDateStr);

        this._submitBtnId = submitBtnId;
        this._subMsjAcro = "_subMsj";

        this._validDateMsj = 'Fecha Válida.';
        this._invalidDateMsj = 'Fecha Inválida, rango aceptado: ' + "<br/>";
        this._endMsj = '.';
        this._remainingLettersMsj = '';
        this._errorStartAfterEndMsj = 'No puede ser despues de la fecha de Fin.';
        this._errorEndBeforeStartMsj = 'No puede ser antes de la fecha de Inicio.';
        
        // Disabling the submit button
        this.disableSubmit(true);
    }

    /*
     EFE:
        The following are simple set methods to display messages according the needs
     REQ:
        msj: A string that represents the msj that will be displayed.
     MOD:
        The corresponding instance variable.
     */
    setRemainingLettersInitialMsj(msj) {
        this._remainingLettersMsj = msj;
    }
    setValidDateMsj(msj) {
        this._validDateMsj = msj;
    }

    /*
     EFE:
        Print some of the important values saved by the object
     REQ:
        ---
     MOD:
        ---
     */
    printInfo() {
        console.log('       this._maxNameCharacters = ' + this._maxNameCharacters);
        console.log('this._maxDescriptionCharacters = ' + this._maxDescriptionCharacters);
        console.log('             this._submitBtnId = ' + this._submitBtnId);
        console.log('');
        console.log('                 this._minDate = ' + this._minDate);
        console.log('           this._minDateNumber = ' + this._minDateNumber);
        console.log('---------------------------------');

        console.log('                 this._maxDate = ' + this._maxDate);
        console.log('           this._maxDateNumber = ' + this._maxDateNumber);
        console.log('---------------------------------' );

        console.log('         this._validSubmitForm = ' + this._validSubmitForm);
        console.log('');
        console.log('');
    }

    /*
     EFE:
        Disable and enables the submit button that the validator will manage
     REQ:
        status: status of the disabled attribute of the submit button.
     MOD:
        Disabled status of the submit button.
     */
    disableSubmit(status) {
        this._validSubmitForm = !status;
        document.getElementById(this._submitBtnId).disabled = status;
    }

    /*
     EFE: 
        Another way to turn a date into a numeric value
     REQ: 
        str: string that represents a day in the format day-month-year
     MOD:
        ---
     */
    strdDateToNumber(dateStr) {
        let result = 1;
        while (dateStr.length > 0) {
            let index = dateStr.indexOf('-');
            let elemento;
            if (index == -1) {
                elemento = dateStr
            } else {
                elemento = dateStr.substr(0, index);
            }
            result = result * parseInt(elemento);
            if (dateStr.length == 1) {
                break;
            } else {
                dateStr = dateStr.substr(index + 1, dateStr.length - 1);
            }
        }

        return result;
    }

    /*
     EFE: 
        turns a date into the format day-month-year
     REQ: 
        date: a date object
     MOD:
        ---
     */
    setStrDate(date) {
        let tmpDate = new Date(date);
        let day = tmpDate.getDate();
        var month = tmpDate.getMonth() + 1 ; //months from 1-12
        var year = tmpDate.getFullYear();

        let newdate = year + "-" + month + "-" + day;

        return newdate;
    }

    /*
     EFE: 
        turns a date into the format day-month-year
     REQ: 
        date: a date object
     MOD:
        ---
     */
    getIntOfDate(date) {
        let tmp = new Date(date);
        return tmp.getTime();
    }

    /*
     EFE:
        Changes the innerHtml of the dom element with the specific id,
        and changes the style of the dom element, adds the style according to
        the fourth parameter
     REQ:
        domObject: DOM object
          newMesj: message that will be displayed by the dom element.
         newStyle: the style that will be removed or added to the dom element.
         addStyle: tells if the style will be added or removed.
     MOD:
        The style and the inner html that displays the DOM element.
     */
    changeMsj(domObjectId, newMesj, newStyle, addStyle) {
        document.getElementById(domObjectId).innerHTML = newMesj;
        if (newStyle && addStyle) {
            document.getElementById(domObjectId).style.color = newStyle;
        }
    }

    /* 
     EFE:
        Validates if there is something selected by the user in the select object (selectElement).
     REQ:
        selectElement: DOM element of type slelect
     MOD:
        ---
     */
    validateSomethingInSelectInput(selectElement) {
        let result = false;


        return result;
    }

    /*
     EFE:
        Validates if the firstDate is before the secondDate
     REQ:
         firstDate: A DOM object that is of type date, and must have a value
        secondDate: A DOM object that is of type date, and must have a value
     MOD:
        ---
     */
    validateOrderOfDates(firstDate, secondDate) {
        let result = false;
        let firstDateNumber = this.dateToNumber(this.dateToSTR(firstDate.value));
        let secondDateNumber = this.dateToNumber(this.dateToSTR(secondDate.value));
        if (firstDateNumber <= secondDateNumber) {
            result = true;
        }
        return result;
    }

    /*
     EFE:
        Validates the date dom element if it is between the minDate and the maxDate.
        It could be the min or max
        Otherwise the function returns false
     REQ:
        dateObj: A DOM object that is of type date, and must have a value
     MOD:
        ---
     */
    validateDateInsideLimits(dateObj) {
        let result = false;

        let tempDate = new Date(dateObj);

        //Turn the minimun date to number
        let strDate = this.setStrDate(tempDate);
        let intDate = this.getIntOfDate(strDate);

        //console.log('           min = ' + this._minDateNumber);
        //console.log('      tempDate = ' + strDate);
        //console.log('       intDate = ' + intDate);
        //console.log('           max = ' + this._maxDateNumber);
        //console.log('');

        if (intDate >= this._minDateNumber && intDate <= this._maxDateNumber) {
            result = true;
        }
        else {
            result = false;
        }
        return result;
    }

    /*
     EFE:
        Sets the error mesaje for a dom element
     REQ:
        bigElementId: element that is wrong and needs a msj
     MOD:
        dom element spacified in the parameters
     */
    getOutOfRangeMsj(bigElementId) {
        let result = this._invalidDateMsj + '(' + this._minDateStr + ') ' + ' - (' + this._maxDateStr + ')';
        let elem = document.getElementById(bigElementId + this._subMsjAcro);
        elem.innerHTML = result;
        elem.style.color = "red";
    }

    /*
     EFE:
        Determines the amount of validations that we are going to use
     REQ:
        totalValidations: integer of all the validation that are going to be applied.
     MOD:
        totalValidations: the instance value
     */
    setTotalValidations(totalValidations) {
        this._totalValidations = totalValidations;
    }

    /*
     EFE:
        Method that will save all the results of the validations in the instance array
     REQ:
        validationResult: a boolean result of a validation
     MOD:
        arrOfValidations list
     */
    addValidation(validationResult) {
        this._arrOfValidations.push(validationResult);
    }

    /*
     EFE:
        Returns true if all the validations are passed, false otherwise
     REQ:
        ---
     MOD:
        ---
     */
    validityOfForm() {
        let result = false;
        let amountOfTrues = 0;
        for (let index = 0; index < this._arrOfValidations.length; ++index) {
            if (this._arrOfValidations[index]) {
                amountOfTrues = amountOfTrues + 1;
            }
        }

        if (amountOfTrues === this._totalValidations) {
            result = true;
            document.getElementById(this._submitBtnId).disabled = false;
        } else {
            result = false;
            document.getElementById(this._submitBtnId).disabled = true;
        }

        return result;
    }

    /*
     EFE:
        Set the amount of characters left of the textElement in the textElementSubMsj
     REQ:
          textElement: dom element that will wait for input of the user.
        maxCharacters: the max amount if characters that the element, textElement, will/can have.
     MOD:
        DOM element message represented by the textElement.
     */
    countTextElements(textElement, maxCharacters) {
        let totalCharactersWritten = document.getElementById(textElement.id).value.length;
        //console.log(textElement.id + this._subMsjAcro);
        var subMessageDomElement = document.getElementById(textElement.id + this._subMsjAcro);



        let unusedSpaces = maxCharacters - totalCharactersWritten;

        //console.log(unusedSpaces);
        //console.log(maxCharacters);
        //console.log(totalCharactersWritten);

        subMessageDomElement.innerHTML = (maxCharacters - unusedSpaces) + '/'  + maxCharacters + ' caracteres usados';
        return totalCharactersWritten;
    }

    /* 
     EFE:
        Validates if there is something written by the user in the input text field (textElement).
     REQ:
        testElement: DOM element of type text
     MOD:
        ---
     */
    validateSomethingInTextInput(textElement, maxAmountOfElements) {
        let result = false;
        let totalCharactersWritten = this.countTextElements(textElement, maxAmountOfElements);
        //Validating only if there are characters on the field
        if (totalCharactersWritten > 0) {
            result = true;
        }
        this.addValidation(result);
        return result;
    }

    /*
     EFE:
        Method that validates if the firstDate is before the secondDate.
     REQ:
         dateOne: DOM element that represents a date.
         dateTwo: DOM element that represents a date.
     MOD:
        --
     */
    validateDates(dateOne, dateTwo) {
        let result = false;
        let resultFechaInicial = false;
        let resultFechaFinal = false;

        let fechaUno = document.getElementById(dateOne.id);
        let fechaDos = document.getElementById(dateTwo.id);

        // The following dates are according to the current location
        let f1 = new Date(fechaUno.value + 'CST');
        let f2 = new Date(fechaDos.value + 'CST');

        // Looking if the dates are inside the min and max dates values
        if (fechaUno.value) {
            resultFechaInicial = this.validateDateInsideLimits(f1);
            if (!resultFechaInicial) {
                resultFechaInicial = false;
                this.getOutOfRangeMsj(dateOne.id);
            } else {
                resultFechaInicial = true;
                this.changeMsj(dateOne.id + this._subMsjAcro, this._validDateMsj, 'green', true);
            }
        }
        if (fechaDos.value) {
            resultFechaFinal = this.validateDateInsideLimits(f2);
            if (!resultFechaFinal) {
                resultFechaFinal = false;
                this.getOutOfRangeMsj(dateTwo.id);
            } else {
                resultFechaFinal = true;
                this.changeMsj(dateTwo.id + this._subMsjAcro, this._validDateMsj, 'green', true);
            }
        }

        result = resultFechaInicial && resultFechaFinal;

        //Now we are looking for the order of the dates
        if (fechaUno.value && fechaDos.value && result) {
            let fechaInicioStr = this.setStrDate(f1);
            let fechaInicioInt = this.getIntOfDate(fechaInicioStr);

            let fechaFinalStr = this.setStrDate(f2);
            let fechaFinalInt = this.getIntOfDate(fechaFinalStr);

            if (fechaFinalInt >= fechaInicioInt) {
                result = true;
                this.changeMsj(dateOne.id + this._subMsjAcro, this._validDateMsj, 'green', true);
                this.changeMsj(dateTwo.id + this._subMsjAcro, this._validDateMsj, 'green', true);
            } else {
                result = false;
                this.changeMsj(dateOne.id + this._subMsjAcro, this._errorStartAfterEndMsj, 'red', true);
                this.changeMsj(dateTwo.id + this._subMsjAcro, this._errorEndBeforeStartMsj, 'red', true);
            }
        }
        this.addValidation(result);
        return result;
    }
}


//function validarPlanDeMejora() {
//    let fechaInicioPlan = document.getElementById('fechaInicioPlanDM');
//    let fechaFinalPlan = document.getElementById('fechaFinalPlanDM');
//    let nombrePlan = document.getElementById('nombrePlanDM');

//    // Dejando el limite superior de las fechas a 10 años en el caso de la creacion de los planes de mejora
//    let minDate = new Date(); // Todays Date
//    let topDate = new Date(minDate.getFullYear() + 10, minDate.getMonth(), minDate.getDate()); //10 years from now
//    let validator = new Validador(50, 50, minDate, topDate, 'sendPDMListo');

//    //Definimos la cantidad de validaciones
//    validator.setTotalValidations(2);

//    // Ahora haciendo las validaciones
//    validator.validateSomethingInTextInput(nombrePlan);
//    validator.validateDates(fechaInicioPlan, fechaFinalPlan);  
//}
