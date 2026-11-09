const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');

async function main() {
    const listeners = {};
    const calls = [];
    const ajaxCalls = [];
    const options = [
        { value: '', dataset: {}, disabled: false },
        { value: '10', dataset: { location: '1' }, disabled: false },
        { value: '30', dataset: { location: '3' }, disabled: false }
    ];
    const elements = {
        'schedule-endpoints': { dataset: { hubUrl: '/scheduleHub', shiftsUrl: '/Admin/Schedule/ShiftsGrid',
            assignmentsUrl: '/Admin/Schedule/AssignmentsGrid', timelineUrl: '/Admin/Schedule/ScheduleVisual' } },
        'gantt-location': { value: '1', addEventListener(type, fn) { listeners['location-' + type] = fn; } },
        'gantt-month': { value: '2026-11' },
        'schedule-location-fallback': { value: '1' },
        'af-location': { value: '1' },
        'af-shift': { options, value: '10', get selectedOptions() {
            return options.filter(o => o.value === this.value);
        } }
    };
    const connection = {
        state: 'Connected',
        handlers: {},
        on(name, fn) { this.handlers[name] = fn; },
        onreconnecting(fn) { this.reconnecting = fn; },
        onreconnected(fn) { this.reconnected = fn; },
        withUrl() { return this; },
        withAutomaticReconnect() { return this; },
        build() { return this; },
        start() { return Promise.resolve(); },
        invoke(name, id) { calls.push([name, id]); return Promise.resolve(); }
    };
    const document = {
        readyState: 'complete',
        querySelectorAll() { return []; },
        querySelector() { return null; },
        getElementById(id) { return elements[id] || null; },
        addEventListener(type, fn) { listeners[type] = fn; }
    };
    const context = { document, signalR: {
        HubConnectionBuilder: class { withUrl() { return connection; } },
        HubConnectionState: { Connected: 'Connected' }
    }, htmx: { ajax(...args) { ajaxCalls.push(args); } },
    URLSearchParams, FormData, console };
    vm.runInNewContext(fs.readFileSync(require.resolve('../ScrumFlix/wwwroot/js/sf-schedule.js'), 'utf8'), context);
    await new Promise(setImmediate);
    assert.deepEqual(calls, [['JoinLocationGroup', 1]]);
    assert.equal(options[1].disabled, false);
    assert.equal(options[2].disabled, true);

    elements['gantt-location'].value = '3';
    listeners['location-change']();
    await new Promise(setImmediate);
    assert.deepEqual(calls.slice(1), [['LeaveLocationGroup', 1], ['JoinLocationGroup', 3]]);
    elements['af-location'].value = '3';
    listeners.change({ target: { id: 'af-location' } });
    assert.equal(options[1].disabled, true);
    assert.equal(options[2].disabled, false);
    assert.equal(elements['af-shift'].value, '');

    connection.reconnecting();
    connection.reconnected();
    await new Promise(setImmediate);
    assert.deepEqual(calls.at(-1), ['JoinLocationGroup', 3]);
    connection.handlers.ShiftsUpdated();
    assert.ok(ajaxCalls.some(([method, url]) => method === 'GET' &&
        url.includes('ScheduleVisual?') && url.includes('locationId=3') && url.includes('period=2026-11')));
    elements['shift-filter-form'] = {};
    context.FormData = class {
        *[Symbol.iterator]() {
            yield ['filterLocationId', '3'];
            yield ['filterRoleId', '3'];
            yield ['filterPage', '2'];
        }
    };
    connection.handlers.ShiftsUpdated();
    assert.ok(ajaxCalls.some(([method, url]) => method === 'GET' &&
        url.includes('ShiftsGrid?') && url.includes('filterLocationId=3') &&
        url.includes('filterRoleId=3') && url.includes('filterPage=2')));

    elements['gantt-location'] = null;
    elements['assignment-filter-location'] = { value: '1' };
    elements['assignment-filter-form'] = {};
    listeners['htmx:afterSwap']({ detail: {
        target: { id: 'assignments-grid', querySelectorAll() { return []; } },
        requestConfig: { elt: { id: 'assignment-filter-form' } }
    } });
    await new Promise(setImmediate);
    assert.deepEqual(calls.slice(-2), [['LeaveLocationGroup', 3], ['JoinLocationGroup', 1]]);
    assert.equal(elements['af-location'].value, '1');
    elements['assignments-grid'] = {};
    const pageInput = { value: '3' };
    context.FormData = class {
        *[Symbol.iterator]() {
            yield ['assignmentLocationId', '1'];
            yield ['assignmentSearch', 'Devin'];
            yield ['assignmentSort', 'employee_desc'];
            yield ['assignmentPage', pageInput.value];
            yield ['assignmentPageSize', '50'];
        }
    };
    connection.handlers.AssignmentsUpdated();
    assert.ok(ajaxCalls.some(([method, url]) => method === 'GET' &&
        url.includes('AssignmentsGrid?') && url.includes('assignmentSearch=Devin') &&
        url.includes('assignmentSort=employee_desc') && url.includes('assignmentPage=3') &&
        url.includes('assignmentPageSize=50')));
    const submittedForm = { id: 'assignment-filter-form', action: '/Admin/Schedule/Index',
        querySelector() { return pageInput; },
        setAttribute(name, value) { this[name] = value; } };
    listeners.submit({ target: submittedForm });
    assert.equal(pageInput.value, '1');
    assert.ok(submittedForm['hx-push-url'].includes('assignmentPage=1'));
    console.log('Schedule client smoke passed');
}
main().catch(error => { console.error(error); process.exitCode = 1; });
