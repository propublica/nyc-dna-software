function Trim(str)
{
var re1 = /^\s*/;
var re2 = /\s*$/;
return str.replace(re1, "").replace(re2, "");
}

var blnIsChanged = false;

function CancelDialog() {
	if (blnIsChanged) {
		if (window.confirm("Are you sure you want to cancel all changes?")) {
			window.close();
			return
		}
	}
	else
		window.close();
}
function ConfirmEditCancel(){
	return Confirm("This will drop all changes you have made");
}
function Confirm(sMessage){
	if (!window.confirm(sMessage)) {
		window.event.returnValue = false;
		return false;
	}
	return true;
}

function MessageListView(IncidentSeq, RoleCode) {
	window.showModalDialog("MessageListView.aspx?IncidentSeq="+IncidentSeq+"&RoleCode="+RoleCode, null, "DialogWidth:780px; DialogHeight:580px; center:1; help:no; resizable:yes; scroll:yes; menubar:no; status:no");
}
function ReportPreview(ReportURL) {
	window.showModalDialog(ReportURL, null, "DialogWidth:900px; DialogHeight:640px; center:1; help:no; resizable:yes; scroll:yes; menubar:no; status:yes");
}
function LabelPreview(ReportURL) {
	window.showModalDialog(ReportURL, null, "center:1; help:no; resizable:yes; scroll:yes; menubar:no; status:yes");
}

function OpenNotesDialog(title, notesControl, notesButton, buttonClassTD, maxLength){
	var objArgs = new Object();
	objArgs.pageTitle = title;
	objArgs.notesControl = notesControl;
	var retVal = window.showModalDialog("NotesEditDialog.aspx?MaxLength=" + maxLength, objArgs, "DialogWidth:600px; DialogHeight:250px; center:1; help:no; status:no; resizable:no; scroll:no");
	if (retVal != null && retVal != 0){
		notesButton.title = notesControl.value;
		if (notesControl.value != "") {
			notesButton.innerHTML = "...";
			buttonClassTD.className = "EditDataGridButton";
		} else {
			notesButton.innerHTML = "*";
			buttonClassTD.className = "button14";
		}
	}
}

function OpenNotesDialogFO(title, notesControl, notesButton, buttonClassTD, maxLength){
	var objArgs = new Object();
	objArgs.pageTitle = title;
	objArgs.notesControl = notesControl;
	var retVal = window.showModalDialog("../NotesEditDialog.aspx?MaxLength=" + maxLength, objArgs, "DialogWidth:600px; DialogHeight:250px; center:1; help:no; status:no; resizable:no; scroll:no");
	if (retVal != null && retVal != 0){
		notesButton.title = notesControl.value;
		if (notesControl.value != "") {
			notesButton.innerHTML = "...";
			buttonClassTD.className = "EditDataGridButton";
		} else {
			notesButton.innerHTML = "*";
			buttonClassTD.className = "button14";
		}
	}
}

function OpenNotesDialogByUrl(title, notesControl, notesButton, buttonClassTD, maxLength, url){
	var objArgs = new Object();
	objArgs.pageTitle = title;
	objArgs.notesControl = notesControl;
	var retVal = window.showModalDialog(url + "?MaxLength=" + maxLength, objArgs, "DialogWidth:600px; DialogHeight:250px; center:1; help:no; status:no; resizable:no; scroll:no");
	if (retVal != null && retVal != 0){
		notesButton.title = notesControl.value;
		if (notesControl.value != "") {
			notesButton.innerHTML = "...";
			buttonClassTD.className = "EditDataGridButton";
		} else {
			notesButton.innerHTML = "*";
			buttonClassTD.className = "button14";
		}
	}
}


function SetEnabled(element, setEnabled){
	element.disabled = !setEnabled;
}

function SetEnabledCheckBox(element, setEnabled){
	if (element.parentElement.tagName.toLowerCase() == "span")
		element.parentElement.disabled = !setEnabled;
	else
		element.disabled = !setEnabled;
}

function IsVisible(element){
	return (element.style.display == null || element.style.display != "none");
}
function SetVisible(element, visible){
	if (element != null)
		element.style.display = visible? "" : "none";
}
function ToggleVisible(element){
	var bToBeVisible = !IsVisible(element);
	SetVisible(element, bToBeVisible);
	return bToBeVisible;
}
/*
function SetVisibleWithValidator(element, visible, validatorID){
	if (IsVisible(element)) {
		var vdr = document.all[validatorID];
		if (vdr != null) {
			SetValidatorEnabled(vdr, visible);
		}
		SetVisible(element, visible);
	} else {
		SetVisible(element, visible);
		var vdr = document.all[validatorID];
		if (vdr != null) {
			SetValidatorEnabled(vdr, visible);
			vdr.evaluationfunction = RequiredFieldValidatorEvaluateIsValid;
		}
	}
}
*/
function SetVisibleCell(cell, visible, headerElement, columnCellArray){
	if (cell == null || IsVisible(cell) == visible) {
		return;
	}
	SetVisible(cell, visible);
	if (visible) {
		SetVisible(headerElement, true);
	} else {
		RefreshHeaderVisibility(headerElement, columnCellArray);
	}
}
function RefreshHeaderVisibility(headerElement, columnCellArray){
	if (headerElement == null) {
		return;
	}
	if (columnCellArray == null) {
		SetVisible(headerElement, false);
		return;
	}
	var bHeaderVisible = false;
	for (var i = 0; i<columnCellArray.length; i++) {
		if (IsVisible(columnCellArray[i])) {
			bHeaderVisible = true;
			break;
		}
	}
	SetVisible(headerElement, bHeaderVisible);
}
function SetAgeEnabled(txtAge, bEnabled) {
	if (bEnabled) {
		txtAge.disabled = false;
	} else {
		txtAge.value = "";
		txtAge.disabled = true;
	}
}

function ValidateKeyNumeric(){
	var keyCode = event.keyCode;
	if (!(keyCode>=48 && keyCode<=57)) {
		event.returnValue = false;
		return false;
	}
	return true;
}
function ValidateKeyAlphaNumeric(){
	var keyCode = event.keyCode;
	if (!((keyCode>=48 && keyCode<=57) || (keyCode>=65 && keyCode<=90) || (keyCode>=97 && keyCode<=122))) {
		event.returnValue = false;
		return false;
	}
	return true;
}

function ValidateKeyAlphaNumericD(){
	var keyCode = event.keyCode;
	if (!((keyCode>=48 && keyCode<=57) || (keyCode>=65 && keyCode<=90) || (keyCode>=97 && keyCode<=122) || (keyCode==45))) {
		event.returnValue = false;
		return false;
	}
	return true;
}

function DoPostBackOnEnter(eventTarget, eventArgument) {
	if (event.keyCode == 13) {
		if (typeof(Page_ClientValidate) != 'function' || Page_ClientValidate())
			__doPostBack(eventTarget, eventArgument);
	}
}
function SetValidatorEnabled(objValidator, bEnabled) {
		objValidator.enabled = bEnabled;
		if (!bEnabled) {
			objValidator.isvalid = true;
			ValidatorUpdateDisplay(objValidator);
		}
}
function ValidateRequiredFields() {
	if (typeof(Page_ClientValidate) == 'function' && !Page_ClientValidate()){
		window.alert("Invalid data entered - please check exclamation marks (!) on the form");
		event.returnValue = false;
		return false;
	}
	return true;
}

function NotifyValidationErrors()
{
	if (typeof(Page_ClientValidate) == 'function' && !Page_ClientValidate())
		window.alert("Invalid data entered - please check exclamation marks (!) on the form");
}

function ValidateAddressFields(ddlAddressType, txtAddressLine1) {
//	if (ddlAddressType != null && ddlAddressType.selectedIndex <= 0){
//		window.alert("Please select the address type");
//		ddlAddressType.focus();
//		event.returnValue = false;
//		return false;
//	} else
	if (txtAddressLine1.value == ""){
		window.alert("Please enter the address info");
		txtAddressLine1.focus();
		event.returnValue = false;
		return false;
	}
	return true;
}
function ValidateEmploymentFields(ddlEmploymentStatus) {
	if (ddlEmploymentStatus.selectedIndex <= 0){
		window.alert("Please select the employment status");
		ddlEmploymentStatus.focus();
		event.returnValue = false;
		return false;
	}
	return true;
}
function ValidatePersonFields(ddlPersonSalutation, txtPersonLastName) {
	if (ddlPersonSalutation != null && ddlPersonSalutation.selectedIndex <= 0){
		window.alert("Please select the salutation");
		ddlPersonSalutation.focus();
		event.returnValue = false;
		return false;
	} else if (txtPersonLastName.value == ""){
		window.alert("Please enter the last name");
		txtPersonLastName.focus();
		event.returnValue = false;
		return false;
	}
	return true;
}
function ValidateAuthorityRecordFields(ddlAuthorityRecordType) {
	if (ddlAuthorityRecordType.selectedIndex <= 0){
		window.alert("Please select the record type");
		ddlAuthorityRecordType.focus();
		event.returnValue = false;
		return false;
	}
	return true;
}
function ValidateAuthorityFields(ddlAuthorityType, txtAuthorityName) {
	if (ddlAuthorityType != null && ddlAuthorityType.selectedIndex <= 0){
		window.alert("Please select the entity");
		ddlAuthorityType.focus();
		event.returnValue = false;
		return false;
	} else if (txtAuthorityName.value == ""){
		window.alert("Please enter the entity name");
		txtAuthorityName.focus();
		event.returnValue = false;
		return false;
	}
	return true;
}
function GridControlIdPrefix(controlId) {
	//Control id is assumed like this: "grdData__ctl2_btnChangeStatus".
	return controlId.substring(0, controlId.lastIndexOf("_") + 1);
}
function ReloadSubTypeSelect(nTypeIndex, ddlSubType, asSubTypeValues, asSubTypeText) {
	ddlSubType.length = asSubTypeValues[nTypeIndex].length;
	for (var i=0; i<ddlSubType.length; i++){
		ddlSubType.options[i] = new Option(asSubTypeText[nTypeIndex][i], asSubTypeValues[nTypeIndex][i]);
	}
	ddlSubType.selectedIndex = 0;
}
function ReloadSubTypeSelectByValue(sTypeValue, asTypeValues, ddlSubType, asSubTypeValues, asSubTypeText) {
	for (var i=0; i<asTypeValues.length; i++) {
		if (asTypeValues[i] == sTypeValue) {
			break;
		}
	}
	if (i < asTypeValues.length) {
		ReloadSubTypeSelect(i, ddlSubType, asSubTypeValues, asSubTypeText);
	} else {
		ddlSubType.length = 0;
	}
}
function RefreshOnMutexCheck(checkBox, controlIDsPattern) {
	if (checkBox.checked) {
		var rgIDs = new RegExp(controlIDsPattern);
		var colControls = document.forms[0].elements;
		for (var i=0; i<colControls.length; i++) {
			var chkCtl = colControls[i];
			if (chkCtl.id != null) {
				if (chkCtl.id.search(rgIDs) == 0 && chkCtl != checkBox && chkCtl.checked) {
					chkCtl.checked = false;
					break;
				}
			}
		}
	}
}
function CheckAddressExists(checkBox, sExists) {
	if (checkBox.checked) {
		if (sExists != "1") {
			window.alert("This reporter has no primary address. Please enter the address first");
			return false;
		}
	}
	return true;
}
function CopyAddress(sOwnerFrom, sOwnerTo) {
	var sPrefix = "ddl";
	var sBase = "AddressType";
	var objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	var objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.selectedIndex = objControlFrom.selectedIndex;
	}

	sPrefix = "chk";
	sBase = "AddressIsPrimary";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.value = objControlFrom.value;
	}

	sPrefix = "txt";
	sBase = "AddressLine1";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.value = objControlFrom.value;
	}

	sPrefix = "txt";
	sBase = "AddressLine2";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.value = objControlFrom.value;
	}

	sPrefix = "txt";
	sBase = "City";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.value = objControlFrom.value;
	}

	sPrefix = "ddl";
	sBase = "State";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.selectedIndex = objControlFrom.selectedIndex;
	}

	sPrefix = "td";
	sBase = "OtherState";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		if (IsVisible(objControlTo) != IsVisible(objControlFrom)) {
			ToggleVisible(objControlTo);
		}
	}

	sPrefix = "txt";
	sBase = "OtherState";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.value = objControlFrom.value;
	}

	sPrefix = "txt";
	sBase = "Zip";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.value = objControlFrom.value;
	}

	sPrefix = "ddl";
	sBase = "Country";
	objControlFrom = document.all[sPrefix + sOwnerFrom + sBase];
	objControlTo = document.all[sPrefix + sOwnerTo + sBase];
	if (objControlFrom != null && objControlTo != null) {
		objControlTo.selectedIndex = objControlFrom.selectedIndex;
	}
}

function RefreshDOB(txtDOB, sYear, sMonth, sDay) {
	if (sYear != "" && sMonth != "" && sDay != "") {
		txtDOB.value = sMonth + "/" + sDay + "/" + sYear;
	} else {
		txtDOB.value = "";
	}
}

function CalculateAge(txtAge, ddlAgeUnit, sDOBYear, sDOBMonth, sDOBDay, anYMDSeqs) {
//	if (txtAge.value != "" || ddlAgeUnit.selectedIndex != 0) {
//		return;
//	}
	if (sDOBYear == "") {
		return;
	}
	var nDOBYear = sDOBYear - 0;
	var nDOBMonth = (sDOBMonth != "")? (sDOBMonth - 0):1;
	var nDOBDay = (sDOBDay != "")? (sDOBDay - 0):1;

	var dtNow = new Date();
	var nCurrentYear = dtNow.getFullYear();
	var nCurrentMonth = dtNow.getMonth() + 1;
	var nCurrentDay = dtNow.getDate();

	var nYears = nCurrentYear - nDOBYear - ((nCurrentMonth * 100 + nCurrentDay < nDOBMonth * 100 + nDOBDay)? 1:0);
	//If there are more than 0 whole years, set age in the years:
	if (nYears > 0) {
		txtAge.value = nYears;
		ddlAgeUnit.value = anYMDSeqs[1];
		return;
	}
	var nMonths = (nCurrentYear - nDOBYear) * 12 + nCurrentMonth - nDOBMonth - ((nCurrentDay < nDOBDay)? 1:0);
	//If there are more than 0 whole months, set age in months:
	if (nMonths > 0) {
		txtAge.value = nMonths;
		ddlAgeUnit.value = anYMDSeqs[2];
		return;
	}
	//Otherwise - set age to nothing:
	txtAge.value = "";
	ddlAgeUnit.value = anYMDSeqs[0];
}

function GuessGender(ddlGender, nSalutationIndex, asSalutationGenders) {
	var sGender = asSalutationGenders[nSalutationIndex];
	if (sGender == "M" || sGender == "F") {
		ddlGender.value = sGender;
	}
}

function ReloadRelshipSelect(ddlRelshipToRM, sGender, asRelshipValues, asRelshipText, asRelshipGenders) {
	var sSavedRelshipSeq = ddlRelshipToRM.value;
	if (sGender == "M" || sGender == "F") {
		ddlRelshipToRM.length = asRelshipValues.length;
		var nListIndex = 0;
		for (var i=0; i<asRelshipValues.length; i++){
			if (asRelshipGenders[i] == sGender || asRelshipGenders[i] == "U") {
				ddlRelshipToRM.options[nListIndex] = new Option(asRelshipText[i], asRelshipValues[i]);
				nListIndex = nListIndex + 1;
			}
		}
		ddlRelshipToRM.length = nListIndex;
		for (var i=0; i<ddlRelshipToRM.length; i++) {
			if (ddlRelshipToRM.options[i].value == sSavedRelshipSeq) {
				ddlRelshipToRM.selectedIndex = i;
				break;
			}
		}
	} else {
		if (ddlRelshipToRM.length == asRelshipValues.length) {
			return;
		}
		ddlRelshipToRM.length = asRelshipValues.length;
		for (var i=0; i<asRelshipValues.length; i++){
			ddlRelshipToRM.options[i] = new Option(asRelshipText[i], asRelshipValues[i]);
		}
		ddlRelshipToRM.value = sSavedRelshipSeq;
	}

}
function WriteError(errorString)
{
	document.getElementById('ErrorString').innerHTML = errorString;
}


function WithOutDialog()
{

	if (VSTwain1.maxImages == null)
	{
		WriteError ("Security settings of the browser do not allow to operate with imaging device.");
		return;
	}
	VSTwain1.StartDevice();
	VSTwain1.Register("Sapphire International Inc","csc.nycnet","EF05698C52D621979F4D148AB4510324C0DEC59445A819A752D690AF7D5D8721AC1AE2B1C2ABCB4F77DBA6A808CF77096523526A53F10533AD1CB3468902ED94B2CC4DC1760A341E2439872A4179E2DB5E7FA5C24507637BF138E38FD596CE06D68EFFF9F8DA71528934CBB0C925E198BA6845705694E481C7ACBD84DA5003545602832AF4B8461B12364D64CD3AF9C4409B8EDAD751C156F72B2A8B29F1BA39");
	VSTwain1.ShowUI=true;
	VSTwain1.maxImages=1;
	var deviceSelected = false;
	if (window._formMain.inputDevice.value != "")
	{
		var deviceName = window._formMain.inputDevice.value;
		for (var i=0; i < VSTwain1.sourcesCount; i++)
			if (VSTwain1.GetSourceProductName(i) == deviceName)
				VSTwain1.object.sourceIndex = i;
		deviceSelected = true;
	}
	else
	{
		deviceSelected = VSTwain1.SelectSource();
		if (deviceSelected)
		{
			if (ShowConfirmDialog("Preferences","Do you want to select this device as default?"))
			{
				window._formMain.inputDevice.value = VSTwain1.GetSourceProductName(VSTwain1.object.sourceIndex);
			}
		}
	}
	if (deviceSelected)
	{
		try
		{
			VSTwain1.OpenDataSource();
			VSTwain1.unitOfMeasure=0;
			VSTwain1.pixelType=1;
			VSTwain1.resolution=200;
			if (VSTwain1.AcquireModal() == true)
			{
				var fileName = window._formMain.inputSID.value + ".jpg";
				var pos = window.location.href.indexOf("PhotoUploadDialog.aspx");
					if (!VSTwain1.SetHttpServerParams(window.location.href.substr(0,pos) + "ImageUpload.aspx" + "?Arguments=" + window._formMain.inputUploadArguments.value ,"",10))
					{
						WriteError("Error happened during image upload to a server: " + VSTwain1.errorString);
						return;
				}
				else
				{
					if (!VSTwain1.SaveImageToHttp(0,"txtFileInput",fileName))
					{
						WriteError("Error happened during image upload to the server: " + VSTwain1.errorString);
					}

				}
				window.setTimeout("WaitForUpload (1)",1000);
				document.all.divPreview.innerHTML = '<img id="imgPreview" src="images/image_not_selected.gif" />';
				blnIsChanged = true;
				var btnSave = document.getElementById('btnSave');
				btnSave.disabled = "";
				btnSave.href = "javascript:__doPostBack('btnSave','');"
			}
		}
		finally
		{
			VSTwain1.CloseDataSource();
		}
	}
}

// recurently check camera control params and waits for image upload ending
function WaitForUpload(depth)
{
	if (depth > 100)
	{
		WriteError("Error happened during image upload to server: Image upload timeout.");
		return;
	}
	if (VSTwain1.httpState == 6)
	{
		window.setTimeout(";",2000);
		// set preview image source (image writes asp page responce with content type image/bmp
		document.all.divPreview.innerHTML = '<img id="imgPreview" src="PhotoPreview.aspx" />';
		return;
	}
	if (VSTwain1.httpErrorCode != 0)
	{
		WriteError("Error happened during image upload to server: Error code " + VSTwain1.httpErrorCode);
		return;
	}
	depth += 1;
	window.setTimeout("WaitForUpload (" + depth + ")",1000);
}

function ChangeScaningDeviceDialog() {
		VSTwain1.Register("Sapphire International Inc","csc.nycnet","EF05698C52D621979F4D148AB4510324C0DEC59445A819A752D690AF7D5D8721AC1AE2B1C2ABCB4F77DBA6A808CF77096523526A53F10533AD1CB3468902ED94B2CC4DC1760A341E2439872A4179E2DB5E7FA5C24507637BF138E38FD596CE06D68EFFF9F8DA71528934CBB0C925E198BA6845705694E481C7ACBD84DA5003545602832AF4B8461B12364D64CD3AF9C4409B8EDAD751C156F72B2A8B29F1BA39");
		VSTwain1.StartDevice();
		if (VSTwain1.SelectSource())
		{
			if (ShowConfirmDialog("Preferences","Do you want to select this device as default?"))
			{
				window._formMain.inputDevice.value = VSTwain1.GetSourceProductName(VSTwain1.object.sourceIndex);
			}
		}
}

function SetTypeNotEqualByDetail(txtTypeNotEqual, sDetail) {
	txtTypeNotEqual.value = (sDetail != "")? "0":"";
}

function UpdateRadioButtons(arrGroups, objItems1)
{
	var yesCounter = 0;
	for (var i = 0; i < objItems1.length; i++)
		if (objItems1[i].checked == true)
			yesCounter++;
	if (yesCounter == 5)
		arrGroups[0].checked = true;
	else if (yesCounter == 0)
		arrGroups[1].checked = true;
	else
		arrGroups[2].checked = true;
}

function UpdateGroups()
{
	UpdateRadioButtons(actlRightHandGroups, actlRightHandYes);
	UpdateRadioButtons(actlLeftHandGroups, actlLeftHandYes);
}

function UpdateYesColumn(actlYes, actlMissing, actlNotPossible, actlOtherReason)
{
	for (var i = 0; i < actlYes.length; i++)
	{
		actlYes[i].checked = true;
		actlMissing[i].checked = false;
		actlMissing[i].parentElement.disabled = true;
		actlNotPossible[i].checked = false;
		actlNotPossible[i].parentElement.disabled = true;
		actlOtherReason[i].style.display = 'none';
	}
}

function UpdateNoColumn(actlNo, actlMissing, actlNotPossible, actlOtherReason)
{
	for (var i = 0; i < actlNo.length; i++)
	{
		actlNo[i].checked = true;
		actlMissing[i].parentElement.disabled = false;
		actlMissing[i].checked = true;
		actlNotPossible[i].parentElement.disabled = false;
		actlNotPossible[i].checked = false;
		actlOtherReason[i].style.display = 'none';
	}
}

function txtFileInputOnChange()
{
	if (document.all.txtFileInput.value != '')
	{
		document.all.divPreview.innerHTML = "<img id='imgPreview' class='resizeImage' src='file://" + document.all.txtFileInput.value + "' />"
		var img = document.all.imgPreview;
		if (img.offsetHeight < 10 || img.offsetWidth < 10)
			alert("min values " +  img.offsetHeight + " " +  img.offsetWidth);
		if (img.offsetHeight >= img.offsetWidth)
		{
			if (img.offsetHeight > 300)
			{
				img.style.width = 300/img.offsetHeight*img.offsetWidth + 'px';
				img.style.height = '300px';
			}
		}
		else
		{
			if (img.offsetWidth > 400)
			{
				img.style.height = 400/img.offsetWidth*img.offsetHeight + 'px';
				img.style.width = '400px';
			}
		}
		document.all.btnSave.disabled=false;
		document.all.btnSave.href="javascript:__doPostBack('lbD','')";
	}
}

function CloseDialog(retValue)
{
	window.returnValue=retValue;
	window.close();
}

function OpenUserSelectDialog(path, ddlUsers, txtUserName, hdnUserSeq, roleCode)
{
	var objArgs = new Object();
	objArgs.pageTitle = "UVIS User Select";
	objArgs.frameSrc = path + "UserSelect.aspx?RoleCode=" + roleCode;
	var retVal = window.showModalDialog(path + "ModalFrame.htm", objArgs,
		"DialogWidth:800px; DialogHeight:600px; center:1; help:no; resizable:yes; scroll:yes; status:no;");
	if (retVal != null){
		if (ddlUsers != null)
			SelectDropDownListItemByValue(ddlUsers, retVal.UserSeq);
		if (hdnUserSeq != null)
			hdnUserSeq.value = retVal.UserSeq;
		txtUserName.value = retVal.FullName;
	}
	return true;
}

function OpenEntitySelectDialog(path, entityName, entityDisplayName, txtEntityName, hdnEntitySeq)
{
	var objArgs = new Object();
	objArgs.pageTitle = "UVIS Select " + entityDisplayName;
	objArgs.frameSrc = path + "EntitySelect.aspx?EntityName=" + entityName + "&EntityDisplayName=" + entityDisplayName;
	var retVal = window.showModalDialog(path + "ModalFrame.htm", objArgs, "DialogWidth:500px; DialogHeight:600px; center:1; help:no; resizable:yes; scroll:yes; status:no;");
	if (retVal != null){
		if (hdnEntitySeq != null)
			hdnEntitySeq.value = retVal.EntitySeq;
		txtEntityName.value = retVal.EntityName;
	}
	return true;
}

function OpenDialogWithPostBackOnClose(appPath, pageUrl, dialogTitle, postBackControlName, postBackArguments, width, height)
{
	var objArgs = new Object();
	objArgs.pageTitle = dialogTitle;
	objArgs.frameSrc = pageUrl;
	var retVal = window.showModalDialog(appPath + "ModalFrame.htm", objArgs, "DialogWidth:" + width + "px; DialogHeight:" + height +"px; center:1; help:no; resizable:yes; scroll:yes; status:no;");
	if (retVal == null || retVal == 0){
		return false;
	}
	return true;
}

function SelectDropDownListItemByValue (ddl, value)
{
	for (var i = 0; i < ddl.options.length; i++)
		if (ddl.options[i].value == value)
		{
			ddl.selectedIndex = i;
			return;
		}
}

function ValidateRequiredField(sender, args)
{
	var val = Trim(args.Value);
	if (val == '' || val == '0')
	{
		var controls = sender.LinkedControls.split(',');
		var filled = false;
		for (var i = 0; i < controls.length; i++)
		{
			var control = document.getElementById(controls[i]);
			if (control.tagName.toLowerCase() == 'input')
			{
				if (control.type == 'text')
					filled = filled || (Trim(control.value) != '');
			}
			if (control.tagName.toLowerCase() == 'select')
			{
				filled = filled || (Trim(control.value) != '' && Trim(control.value) != '0');
			}
		}
		args.IsValid = !filled;
	}
}

function OpenUploadPhotoDialog(appPath, pageUrl, hidControl)
{
	var objArgs = new Object();
	objArgs.pageTitle = "Upload photo";
	objArgs.frameSrc = pageUrl;
	var retVal = window.showModalDialog(appPath + "ModalFrame.htm", objArgs, "DialogWidth:800px; DialogHeight:600px; center:1; help:no; resizable:yes; scroll:yes; status:no;");
	if (retVal == null || retVal == 0){
		return false;
	}
	hidControl.value = retVal;
	return true;
}

function OpenShowPhotoDialog(appPath, pageUrl)
{
	var objArgs = new Object();
	objArgs.pageTitle = "Photo";
	objArgs.imageSrc = pageUrl;
	window.showModalDialog(appPath + "PhotoViewDialog.htm", objArgs, "DialogWidth:800px; DialogHeight:600px; center:1; help:no; resizable:yes; scroll:yes; status:no;");
	return false;
}

function OpenYesNoDialog(appPath, message)
{
	var objArgs = new Object();
	objArgs.pageTitle = "OCME UVIS";
	objArgs.Message = message;
	var retVal = window.showModalDialog(appPath + "ConfirmDialog.htm", objArgs, "DialogWidth:400px; DialogHeight:120px; center:1; help:no; resizable:no; scroll:no; status:no;");
	if (retVal == null || retVal == 2){
		return false;
	}
	return true;
}

function OpenYesNoCancelDialog(appPath, message)
{
	var objArgs = new Object();
	objArgs.pageTitle = "OCME UVIS";
	objArgs.Message = message;
	var retVal = window.showModalDialog(appPath + "ConfirmDialogYesNoCancel.htm", objArgs, "DialogWidth:400px; DialogHeight:120px; center:1; help:no; resizable:no; scroll:no; status:no;");
	if (retVal == 1)
		return "Yes";
	if (retVal == 2)
		return "No";
	return "Cancel";
}

function TrackChecked(control)
{
	if(event.propertyName == "checked")
	{
		if(control.checked)
			control.setAttribute("wasChecked","false");
	}
}

function CheckRadio(control)
{
	if(control.getAttribute("wasChecked")=="true")
	{
		control.checked = false;
		control.setAttribute("wasChecked","false");
	} 
	else 
	{
		control.setAttribute("wasChecked","true");
	}
}