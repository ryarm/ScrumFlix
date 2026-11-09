/**
 * Export Reports: keep the form on the page while the POST prepares a file.
 * A native form download does not fire a completion event on the originating
 * page, so window focus is not a reliable signal to stop a loading spinner.
 */
(function () {
    'use strict';

    var forms = document.querySelectorAll('.sf-export-form');
    if (!forms.length) return;

    function setMessage(element, kind, message) {
        element.classList.remove('d-none', 'sf-flash-banner--danger', 'sf-flash-banner--info');
        element.classList.add('sf-flash-banner--' + kind);
        element.textContent = message;
    }

    function filenameFromHeader(header) {
        var encoded = /filename\*\s*=\s*UTF-8''([^;]+)/i.exec(header);
        var plain = /filename\s*=\s*(?:"([^"]+)"|([^;]+))/i.exec(header);
        var name = encoded ? encoded[1] : plain ? (plain[1] || plain[2]).trim() : '';
        if (encoded) {
            try { name = decodeURIComponent(name); } catch (_) { name = ''; }
        }
        // The response is same-origin, but the name still becomes a local filename.
        return name.replace(/[\\/\x00-\x1f<>:"|?*]/g, '_') || 'report';
    }

    function messageFromHtml(html) {
        var page = new DOMParser().parseFromString(html, 'text/html');
        var message = page.querySelector('.sf-flash-banner--danger, .sf-flash-banner--info, .validation-summary-errors, .field-validation-error');
        return message ? message.textContent.trim() : '';
    }

    forms.forEach(function (form) {
        var spinnerWrap = form.querySelector('.sf-export-spinner-wrap');
        var spinner = form.querySelector('.sf-export-spinner');
        var button = form.querySelector('.sf-export-btn');
        var result = form.querySelector('.sf-export-result');
        if (!spinnerWrap || !spinner || !button || !result) return;

        var busy = false;
        form.addEventListener('submit', async function (event) {
            if (event.defaultPrevented) return; // Respect client-side validation.
            event.preventDefault();
            if (busy) return;

            busy = true;
            button.disabled = true;
            result.classList.add('d-none');
            spinnerWrap.classList.remove('d-none');
            sfSpinner.indeterminate(spinner, true);

            try {
                var response = await fetch(form.action, {
                    method: 'POST',
                    credentials: 'same-origin',
                    body: new FormData(form)
                });
                var disposition = response.headers.get('Content-Disposition') || '';

                if (response.ok && /\battachment\b/i.test(disposition)) {
                    var file = await response.blob();
                    var url = URL.createObjectURL(file);
                    try {
                        var link = document.createElement('a');
                        link.href = url;
                        link.download = filenameFromHeader(disposition);
                        link.hidden = true;
                        document.body.appendChild(link);
                        link.click();
                        link.remove();
                    } finally {
                        // Allow the browser time to take ownership of the blob URL.
                        setTimeout(function () { URL.revokeObjectURL(url); }, 60000);
                    }
                } else if ((response.headers.get('Content-Type') || '').includes('text/html')) {
                    var message = messageFromHtml(await response.text());
                    setMessage(result, response.ok && !response.redirected ? 'info' : 'danger',
                        message || (response.redirected ? 'Your session may have expired. Sign in and try again.' : 'The report could not be generated.'));
                } else {
                    setMessage(result, 'danger', 'The report could not be generated. Please try again.');
                }
            } catch (_) {
                setMessage(result, 'danger', 'The download failed. Check your connection and try again.');
            } finally {
                sfSpinner.reset(spinner);
                spinnerWrap.classList.add('d-none');
                button.disabled = false;
                busy = false;
            }
        });
    });
}());
