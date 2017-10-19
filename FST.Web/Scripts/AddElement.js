function addElement(index) {
            var ni = document.getElementById('tab'+index);
            //var numi = document.getElementById('theValue');
            //var num = (document.getElementById('theValue').value - 1) + 2;
            //numi.value = num;
            var newdiv = document.createElement('div');
            var divIdName = 'my' + index + 'Div';
            newdiv.setAttribute('id', divIdName);
            newdiv.innerHTML = "Element Number " + index + " has been added!";

            ni.appendChild(newdiv);
        }
function removeElement(divNum) {
    var d = document.getElementById('myDiv');
    var olddiv = document.getElementById(divNum);
    d.removeChild(olddiv);
}