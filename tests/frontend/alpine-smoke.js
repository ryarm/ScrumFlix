const fs = require('fs');
const assert = require('assert');
const { JSDOM } = require('jsdom');
const root = __dirname + '/../../ScrumFlix/wwwroot/js/alpine/';
const html = `<!doctype html><html><body>
<div x-data="passwordVisibility" data-password-label="current password"><input id="pw" type="password" x-bind:type="inputType"><button id="toggle" type="button" x-on:click="toggle" x-bind:aria-label="label" x-bind:aria-pressed="shown"></button></div>
<form id="ticket" x-data="ticketQuantity"><span id="priceDisplay" data-price="12.50"></span><input id="qtyInput" value="2" min="1" max="3" readonly><strong id="lineTotal"></strong><button id="plus" type="button" x-on:click="increment">+</button><button id="minus" type="button" x-on:click="decrement">-</button></form>
<form id="cart" x-data="cartQuantity"><input name="cartItemId" value="44"><input name="quantity" type="number" value="2" x-on:change="changed"><button id="cartplus" type="button" x-on:click="increment">+</button><button id="cartminus" type="button" x-on:click="decrement">-</button></form>
<form id="cart2" x-data="cartQuantity"><input name="cartItemId" value="45"><input name="quantity" type="number" value="2" x-on:change="changed"></form>
<form id="conc" x-data="concessionQuantity"><input name="quantity" type="number" value="2" x-on:change="changed"><button id="concplus" type="button" x-on:click="increment">+</button></form>
<form id="confirm" x-data="confirmation" data-confirm-prompt="Delete this?" x-on:submit="handleSubmit" x-on:keydown.escape="cancel"><input name="id" value="7"><button id="delete" type="button" x-on:click="open">Delete</button><span x-cloak x-show="confirming"><span id="prompt" x-text="prompt"></span><button id="yes" data-confirm-yes type="button" x-on:click="confirm">Yes</button><button id="cancel" type="button" x-on:click="cancel">Cancel</button></span></form>
</body></html>`;
const dom = new JSDOM(html, {runScripts:'dangerously', url:'https://scrumflix.test/'});
const {window:w} = dom;
const errors=[];w.addEventListener('error', e=>errors.push(e.message));
let posts=[];
w.HTMLFormElement.prototype.requestSubmit=function(){const e=new w.Event('submit',{bubbles:true,cancelable:true});if(this.dispatchEvent(e) && !e.defaultPrevented) posts.push(this.id);};
w.eval(fs.readFileSync(root+'scrumflix-alpine.js','utf8'));
w.eval(fs.readFileSync(root+'alpine-csp.min.js','utf8'));
const tick=()=>new Promise(resolve=>setTimeout(resolve,25));
(async()=>{
 await tick();
 const click=id=>w.document.getElementById(id).click();
 assert.equal(w.document.getElementById('toggle').getAttribute('aria-label'),'Show current password');
 click('toggle');await tick();assert.equal(w.document.getElementById('pw').type,'text');
 let events=0; w.document.getElementById('qtyInput').addEventListener('qtychanged',()=>events++);
 assert.equal(w.getQty(),2);assert.equal(w.document.getElementById('qtyInput').readOnly,true);click('plus');await tick();assert.equal(w.getQty(),3);assert.equal(w.document.getElementById('lineTotal').textContent,'$37.50');assert.equal(events,1);
 click('plus');await tick();assert.equal(w.getQty(),3);
 click('cartplus');await tick();assert.deepEqual(posts,['cart']);assert.equal(w.document.querySelector('#cart input[name=quantity]').value,'3');
 const cartInput=w.document.querySelector('#cart2 input[name=quantity]');cartInput.value='99';cartInput.dispatchEvent(new w.Event('change',{bubbles:true}));await tick();assert.equal(cartInput.value,'20');assert.deepEqual(posts,['cart','cart2']);
 const c=w.document.querySelector('#conc input');c.value='200';c.dispatchEvent(new w.Event('change',{bubbles:true}));await tick();assert.equal(c.value,'20');
 click('delete');await tick();assert.equal(posts.length,2);assert.equal(w.document.getElementById('prompt').textContent,'Delete this?');assert.equal(w.document.activeElement.id,'yes');
 click('cancel');await tick();assert.equal(posts.length,2);
 click('delete');await tick();click('yes');await tick();assert.deepEqual(posts,['cart','cart2','confirm']);
 assert.deepEqual(errors,[]);console.log('Alpine CSP smoke passed');
})().catch(e=>{console.error(e,errors);process.exit(1)});
