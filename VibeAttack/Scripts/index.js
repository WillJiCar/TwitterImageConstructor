//Find event that handles scroll wheel change
//Get Scroll wheel value
//Use that value to change the background colour
//Not a fan? Move and scroll your mousewheel. 

var wheelY = 123;
var mouseX = 0;
var mouseY = 0;

var toggleBackgroundChange = false;

window.addEventListener("click", function () {
    toggleBackgroundChange = !toggleBackgroundChange;
    console.log("Clicked window, toggle = " + toggleBackgroundChange);
});



window.addEventListener("wheel", function (event) {
    if (toggleBackgroundChange) {
        console.log("Wheel event, Y = " +  wheelY);
        if (event.deltaY < 0 && wheelY < 255) {
            //scroll up
            wheelY = wheelY + 10;
            changeBackground();
        } else if (event.deltaY > 0 && wheelY > 0) {
            //scroll down
            wheelY = wheelY - 10;
            changeBackground();
        }
    }
});

window.addEventListener("mousemove", function (event) {
    if (toggleBackgroundChange) {
        mouseX = event.clientX;
        mouseY = event.clientY;
        changeBackground();
    }    
});


function changeBackground() {
    var width = window.innerWidth;
    var height = window.innerHeight;
    var red = wheelY;
    var green = (mouseX / width) * 255;
    var blue = (mouseY /height) * 255;
    document.body.style.backgroundColor = 'rgb(' + red + ',' + green + ',' + blue + ')';

}