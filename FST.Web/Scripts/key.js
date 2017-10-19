


function setINC_NEG(obj)
{
   var ctrl = window.document.getElementById(obj);
   if (ctrl !=null)
   {
        ctrl.style.display="block";}
}

function hiding (obj)
{
    alert("me again.");
    var ctrl = window.document.getElementById(obj);
   if (ctrl !=null)
   {
        ctrl.style.display="none";}
}


function dropdownlistChange(ddlName,ddlClinetName,incbox,negbox)
{
    
    var obj = window.document.getElementById(ddlClinetName);
    var incObj = window.document.getElementById(incbox);
    var negObj = window.document.getElementById(negbox);
    
    if(obj != null)
    {
        if ( obj.options[obj.selectedIndex].text =='NEG')
        {
            incObj.value = incObj.value.replace(ddlName,'');
            negObj.value =  negObj.value + "," + ddlName;
            //alert(incObj.value +"|" + negObj.value);
        }
        else
        {
            if (obj.options[obj.selectedIndex].text  =='INC')
            {
                negObj.value = negObj.value.replace(ddlName,'');
                incObj.value = incObj.value +"," + ddlName;
                //alert(incObj.value +"|" + negObj.value );
            }
        }
    }
}