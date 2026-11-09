const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const source = fs.readFileSync(path.join(__dirname, '../ScrumFlix/wwwroot/js/sf-export.js'), 'utf8');

function element(initial = []) {
    const values = new Set(initial);
    return {
        classList: {
            add: (...names) => names.forEach(name => values.add(name)),
            remove: (...names) => names.forEach(name => values.delete(name)),
            contains: name => values.has(name)
        },
        textContent: '',
        disabled: false
    };
}

async function runCase(responseOrError) {
    const wrap = element(['d-none']);
    const spinner = element();
    const button = element();
    const result = element(['d-none']);
    const fields = { '.sf-export-spinner-wrap': wrap, '.sf-export-spinner': spinner, '.sf-export-btn': button, '.sf-export-result': result };
    let submit;
    const form = {
        action: '/Admin/AdminManage/ExportTickets',
        querySelector: selector => fields[selector],
        addEventListener: (name, handler) => { if (name === 'submit') submit = handler; }
    };
    const links = [];
    const calls = [];
    const context = {
        document: {
            querySelectorAll: () => [form],
            createElement: () => ({ click() { links.push(this.download); }, remove() {} }),
            body: { appendChild() {} }
        },
        sfSpinner: { indeterminate: () => calls.push('start'), reset: () => calls.push('stop') },
        FormData: class { constructor(value) { assert.equal(value, form); this.token = 'antiforgery'; } },
        URL: { createObjectURL: () => 'blob:report', revokeObjectURL() {} },
        DOMParser: class {
            parseFromString(html) {
                return { querySelector: () => ({ textContent: html }) };
            }
        },
        setTimeout: () => 1,
        fetch: async (url, options) => {
            assert.equal(url, form.action);
            assert.equal(options.method, 'POST');
            assert.equal(options.credentials, 'same-origin');
            assert.equal(options.body.token, 'antiforgery');
            if (responseOrError instanceof Error) throw responseOrError;
            return responseOrError;
        }
    };
    vm.runInNewContext(source, context, { filename: 'sf-export.js' });
    assert.equal(typeof submit, 'function');
    const event = { defaultPrevented: false, preventDefault() { this.defaultPrevented = true; } };
    await submit(event);
    assert.equal(event.defaultPrevented, true);
    assert.deepEqual(calls, ['start', 'stop']);
    assert.equal(wrap.classList.contains('d-none'), true, 'spinner must stop without focus or click');
    assert.equal(button.disabled, false);
    return { links, result };
}

(async () => {
    const file = await runCase({
        ok: true, redirected: false,
        headers: { get: key => key === 'Content-Disposition' ? 'attachment; filename="tickets_20261101_20261109.csv"' : 'text/csv' },
        blob: async () => new Blob(['TicketId\n1'])
    });
    assert.deepEqual(file.links, ['tickets_20261101_20261109.csv']);
    assert.equal(file.result.classList.contains('d-none'), true, 'success banner should not remain after download starts');

    const empty = await runCase({
        ok: true, redirected: false,
        headers: { get: key => key === 'Content-Type' ? 'text/html; charset=utf-8' : null },
        text: async () => 'No tickets found in the chosen date range.'
    });
    assert.deepEqual(empty.links, []);
    assert.match(empty.result.textContent, /No tickets found/);

    const failed = await runCase(new Error('offline'));
    assert.deepEqual(failed.links, []);
    assert.match(failed.result.textContent, /download failed/);
    console.log('Export client smoke passed');
})().catch(error => { console.error(error); process.exitCode = 1; });
