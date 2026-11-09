const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const root = path.join(__dirname, '..', 'ScrumFlix');
const partial = fs.readFileSync(path.join(root, 'Areas/Admin/Views/Shared/_AdminSidebar.cshtml'), 'utf8');
const layout = fs.readFileSync(path.join(root, 'Areas/Admin/Views/Shared/_AdminLayout.cshtml'), 'utf8');
assert.equal((partial + layout).match(/<script[^>]+sf-admin-sidebar\.js/g)?.length, 1,
    'the sidebar script must be loaded once');
assert.match(partial, /id="sf-sidebar-backdrop"/);

const script = fs.readFileSync(path.join(root, 'wwwroot/js/sf-admin-sidebar.js'), 'utf8');

function run(startMobile, saved, storageFails = false) {
    function element() {
        const listeners = {};
        const attrs = {};
        const classes = new Set();
        return {
            listeners, attrs, classes, hidden: true, inert: false, focused: false,
            setAttribute(k, v) { attrs[k] = v; },
            addEventListener(k, fn) { listeners[k] = fn; },
            focus() { this.focused = true; },
            classList: {
                toggle(k, force) { if (force) classes.add(k); else classes.delete(k); },
                contains(k) { return classes.has(k); }
            },
            fire(k, e = {}) { listeners[k](e); }
        };
    }
    const sidebar = element(), toggle = element(), backdrop = element(), document = element();
    const media = element(); media.matches = startMobile;
    const writes = [];
    vm.runInNewContext(script, {
        document: {
            getElementById(id) { return ({ 'sf-admin-sidebar': sidebar, 'sf-sidebar-toggle': toggle, 'sf-sidebar-backdrop': backdrop })[id]; },
            addEventListener: document.addEventListener.bind(document)
        },
        window: { matchMedia() { return media; } },
        localStorage: {
            getItem() { if (storageFails) throw Error('storage unavailable'); return saved; },
            setItem(k, v) { if (storageFails) throw Error('storage unavailable'); writes.push([k, v]); }
        }
    });
    const open = () => toggle.attrs['aria-expanded'] === 'true';
    return { sidebar, toggle, backdrop, document, media, writes, open };
}

let ui = run(false, null);
assert.equal(ui.open(), true, 'desktop defaults open');
ui.toggle.fire('click');
assert.equal(ui.open(), false, 'one click closes desktop menu');
assert.equal(ui.sidebar.inert, true);
assert.deepEqual(ui.writes, [['sf-sidebar-open', '0']]);
ui.toggle.fire('click');
assert.equal(ui.open(), true, 'next click reopens desktop menu');

ui = run(true, '1');
assert.equal(ui.open(), false, 'mobile starts closed even with desktop open preference');
ui.toggle.fire('click');
assert.equal(ui.open(), true, 'mobile toggle exposes links');
assert.equal(ui.backdrop.hidden, false);
assert.equal(ui.sidebar.inert, false);
assert.equal(ui.sidebar.focused, true);
ui.backdrop.fire('click');
assert.equal(ui.open(), false, 'backdrop closes mobile menu');
ui.toggle.fire('click');
ui.document.fire('keydown', { key: 'Escape' });
assert.equal(ui.open(), false, 'Escape closes mobile menu');
assert.equal(ui.toggle.focused, true);
assert.equal(ui.writes.length, 0, 'mobile does not override desktop preference');
ui.media.matches = false; ui.media.fire('change');
assert.equal(ui.open(), true, 'desktop preference restored after resize');

ui = run(false, null, true);
ui.toggle.fire('click');
assert.equal(ui.open(), false, 'menu works when localStorage is blocked');
console.log('Admin sidebar smoke passed');
