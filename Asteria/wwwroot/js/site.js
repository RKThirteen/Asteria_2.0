var hair_color = 'brown'
var eye_color = 'blue'
var shirt_color = 'blue'
var pants_color = 'brown'
var skin_color = '#d1cfa5'
var shoe_color = 'rgb(0,0,0)';
var created;
var face_type = 3;

function random_color() {
    var randomColor = '#' + Math.floor(Math.random() * 16777215).toString(16);
    return randomColor;
}

function generator(char) {
    let r = 0;
    let g = 0;
    let b = 0;
    let len = char.length;
    if (len > 0) {
        for (let i = 0; i < len; i++) {
            if (i % 3 === 0) {
                r = Math.abs((r + char.charCodeAt(i) - 20 * (i + 1))) % 256
                //console.log(i);
            }
            if (i % 3 === 1) {
                g = Math.abs((g + char.charCodeAt(i) - 20 * (i + 1))) % 256
                // console.log(i);
            }
            if (i % 3 === 2) {
                b = Math.abs((b + char.charCodeAt(i) - 20 * (i + 1))) % 256
                //console.log(i);
            }
        }
        eye_color = 'rgb(' + r + ',' + g + ',' + b + ')';
        //console.log(eye_color);
        drawEyes(ctx);

        for (let i = 0; i < len; i++) {
            if (i % 3 === 0) {
                r = Math.abs((r + char.charCodeAt(i) - 40 * (i + 1))) % 256
                //console.log(i);
            }
            if (i % 3 === 1) {
                g = Math.abs((g + char.charCodeAt(i) - 40 * (i + 1))) % 256
                //console.log(i);
            }
            if (i % 3 === 2) {
                b = Math.abs((b + char.charCodeAt(i) - 40 * (i + 1))) % 256
                //console.log(i);
            }
        }
        shirt_color = 'rgb(' + r + ',' + g + ',' + b + ')';
        //console.log(eye_color);
        drawShirt(ctx);

        for (let i = 0; i < len; i++) {
            if (i % 3 === 0) {
                r = Math.abs((r + char.charCodeAt(i) - 30 * (i + 1))) % 256
                //console.log(i);
            }
            if (i % 3 === 1) {
                g = Math.abs((g + char.charCodeAt(i) - 30 * (i + 1))) % 256
                //console.log(i);
            }
            if (i % 3 === 2) {
                b = Math.abs((b + char.charCodeAt(i) - 30 * (i + 1))) % 256
                //console.log(i);
            }
        }
        pants_color = 'rgb(' + r + ',' + g + ',' + b + ')';
        //console.log(eye_color);
        drawPants(ctx);
    }

}

function drawHead(ctx) {
    ctx.fillStyle = skin_color;
    ctx.beginPath();

    ctx.fillRect(500, 50, 20, 16);
    ctx.closePath();
}

function drawHair(ctx) {
    ctx.fillStyle = hair_color;
    ctx.beginPath();
    ctx.fillRect(500, 45, 20, 5);
    ctx.clearRect(500, 45, 2, 2);
    ctx.clearRect(518, 45, 2, 2);
    ctx.closePath();
}

function drawEyes(ctx) {
    ctx.fillStyle = eye_color;
    ctx.beginPath();
    ctx.fillRect(503, 53, 4, 4);
    ctx.fillRect(512, 53, 4, 4);
    ctx.fillStyle = 'white';
    ctx.fillRect(503, 53, 2, 2)
    ctx.fillRect(512, 53, 2, 2)

}

function drawShirt(ctx) {
    ctx.fillStyle = shirt_color;
    ctx.beginPath();
    ctx.fillRect(495, 65, 30, 30);
}

function drawPants(ctx) {
    ctx.fillStyle = pants_color;
    ctx.beginPath();
    ctx.fillRect(495, 95, 12, 30);
    ctx.fillRect(513, 95, 12, 30);
}

function drawShoes(ctx) {
    ctx.fillStyle = shoe_color;
    ctx.beginPath();
    ctx.fillRect(495, 125, 12, 6);
    ctx.fillRect(513, 125, 12, 6);
    ctx.clearRect(495, 125, 2, 2);
    ctx.clearRect(513, 125, 2, 2);
    ctx.fillStyle = 'white';
    ctx.fillRect(513, 127, 1.5, 1.5);
    ctx.fillRect(495, 127, 1.5, 1.5);
}

function drawFace(ctx) {
    ctx.beginPath();
    ctx.fillStyle = skin_color;
    ctx.fillRect(500, 58, 20, 6);
    if (face_type === 1) {

        ctx.beginPath();
        ctx.fillStyle = 'black';
        ctx.fillRect(505, 61, 10, 3)
        ctx.fillRect(503, 61, 2, 2);
        ctx.fillRect(515, 61, 2, 2);
    }
    if (face_type === 2) {

        ctx.beginPath();
        ctx.fillStyle = 'black';
        ctx.moveTo(507, 60);
        ctx.lineTo(511, 64);
        ctx.stroke();
        ctx.moveTo(511, 60);
        ctx.lineTo(507, 64);
        ctx.stroke();
    }
    if (face_type == 3) {
        ctx.beginPath();
        ctx.fillStyle = 'black';
        ctx.arc(510, 61, 2, 0, 2 * Math.PI);
        ctx.fill()
    }

}

function drawHands(ctx) {
    ctx.beginPath();
    ctx.fillStyle = skin_color;
    ctx.fillRect(487, 65, 8, 24);
    ctx.fillRect(525, 65, 8, 24);
}

let lastRender = Date.now();
let x1 = 487;
let y1 = 65;
let w = 8;
let h = 24;
function wave(ctx) {
    let delta = Date.now() - lastRender;
    x1 += delta / 2;
    y1 += delta / 2;
    ctx.fillStyle = skin_color;
    ctx.fillRect(x1, y1, w, h);
    requestAnimationFrame(wave(ctx));
    console.log('1');
}



window.onload = () => {
    //console.log(localStorage['character']);
    k = localStorage.getItem('character')
    if (k) {
        //console.log(k);
        const avatarpic = document.createElement('img');
        avatarpic.src = k;
        const div = document.getElementById('character');
        avatarpic.style.position = 'absolute';
        avatarpic.style.left = '30%';
        avatarpic.style.zIndex = '-1';
        div.appendChild(avatarpic);
        const nem = document.createElement('div');
        div.classList.add('whattext');
        div.style.opacity = '1'
        div.style.textAlign = 'right';
        nem.innerHTML = "Username: " + localStorage.getItem('name');
        div.appendChild(nem);

    }

    const username = document.createElement('div')
    username.setAttribute('id', 'username');
    // username.style.position='absolute';
    // username.style.top='50%';
    // username.style.left='50%';
    username.classList.add('whattext');
    username.style.opacity = '1';
    const header = document.getElementsByTagName('header');
    header[0].appendChild(username);

    const canvas = document.getElementById('canvas');
    if (canvas) {
        canvas.style.display = 'flex';
        canvas.style.justifyContent = 'center';
        canvas.style.alignItems = 'center';
        ctx = canvas.getContext('2d');
        if (ctx) {
            drawHead(ctx);
            drawHair(ctx);
            drawEyes(ctx);
            drawShirt(ctx);
            drawHands(ctx);
            drawPants(ctx);
            drawShoes(ctx);
            drawFace(ctx);
            //wave(ctx);
        }
        setInterval(function () {
            //console.log('1');
            ctx.fillStyle = 'white';
            ctx.beginPath();
            ctx.fillRect(505, 53, 2, 2);
            ctx.fillRect(514, 53, 2, 2);
            ctx.fillStyle = eye_color;
            ctx.fillRect(503, 53, 2, 2);
            ctx.fillRect(512, 53, 2, 2);
            setTimeout(back, 500);
            function back() {
                //console.log('2');
                ctx.fillStyle = eye_color;
                ctx.fillRect(505, 53, 2, 2);
                ctx.fillRect(514, 53, 2, 2);
                ctx.fillStyle = 'white';
                ctx.fillRect(503, 53, 2, 2);
                ctx.fillRect(512, 53, 2, 2);
            }
        }, 1000);


        const displayname = document.getElementById('nume')
        displayname.addEventListener('change', avatarname)
        function avatarname() {
            username.innerHTML = 'Your character is named ' + displayname.value;
            generator(String(displayname.value));
        }

        const haircolor = document.getElementById('hair');
        haircolor.addEventListener('change', change_hair);

        const skincolor = document.getElementById('skin');
        skincolor.addEventListener('change', change_skin);

        const eyecolor = document.getElementById('eyes');
        eyecolor.addEventListener('change', change_eyes);

        const shirtcolor = document.getElementById('shirt');
        shirtcolor.addEventListener('change', change_shirt);

        const pantscolor = document.getElementById('pants');
        pantscolor.addEventListener('change', change_pants);

        const shoecolor = document.getElementById('shoes');
        shoecolor.addEventListener('change', change_shoes);

        const submit = document.getElementById('submit');
        submit.addEventListener('click', createavatar);

        const facetype = document.getElementById('face');
        facetype.addEventListener('change', change_face);

        window.addEventListener('keydown', random_avatar);
        function random_avatar(event) {//console.log('1')
            switch (event.key) {
                case 'r':
                    hair_color = random_color();
                    //console.log(hair_color);
                    eye_color = random_color()
                    // console.log(eye_color);
                    shirt_color = random_color()
                    pants_color = random_color()
                    skin_color = random_color()
                    shoe_color = random_color()
                    face_type = Math.floor(Math.random() * 3) + 1;
                    drawHead(ctx);
                    drawHair(ctx);
                    drawEyes(ctx);
                    drawPants(ctx);
                    drawShirt(ctx);
                    drawHands(ctx);
                    drawShoes(ctx);
                    drawFace(ctx);
                    return;
                default: return;
            }
        }
        function change_hair(event) {
            event.stopPropagation();
            //console.log('1')
            hair_color = event.target.value;
            drawHair(ctx);
        }
        function change_skin(event) {
            event.stopPropagation();
            //console.log('1')
            skin_color = event.target.value;
            drawHands(ctx);
            drawHead(ctx);
            // drawEyes(ctx);
            //drawFace(ctx);
        }
        function change_eyes(event) {
            event.stopPropagation();
            //console.log('2')
            eye_color = event.target.value;
            drawEyes(ctx);
        }
        function change_shirt(event) {
            event.stopPropagation();
            //console.log('3')
            shirt_color = event.target.value;
            drawShirt(ctx);
        }
        function change_pants(event) {
            event.stopPropagation();
            //console.log('4')
            pants_color = event.target.value;
            drawPants(ctx);
        }
        function change_shoes(event) {
            event.stopPropagation();
            //console.log('5')
            shoe_color = event.target.value;
            drawShoes(ctx);
        }
        function change_face(event) {
            //console.log('6')
            event.stopPropagation();
            face_type = parseInt(event.target.value);
            //console.log(event.target.value)
            drawFace(ctx);
        }
        function createavatar() {
            let avatar = canvas.toDataURL();
            let character = localStorage.getItem('character', avatar);
            let name = document.getElementById('username').innerHTML;
            if (!character) {
                character = localStorage.setItem('character', avatar);
                localStorage.setItem('name', name);
                created = document.createElement('div');
                created.classList.add('whattext');
                created.innerHTML = 'Character Created and Saved!';
                created.style.opacity = '1';
                header[0].appendChild(created);
            }
            else {
                let conf = confirm("overwrite character?");
                if (conf) {
                    character = localStorage.setItem('character', avatar);
                    localStorage.setItem('name', name);
                    created = document.getElementById('created');
                    if (created) {
                        header[0].removeChild(created);
                        created.remove();
                    }


                    created = document.createElement('div');
                    created.classList.add('whattext');
                    created.setAttribute('id', 'created');
                    created.innerHTML = 'Character Overwritten!';
                    created.style.opacity = '1';
                    header[0].appendChild(created);
                }
            }
            //console.log(localStorage.getItem('name'));
        }
    }

}