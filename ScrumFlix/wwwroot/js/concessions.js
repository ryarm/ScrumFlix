// site.js or concessions.js
(function () {
    // 1. Toast Notification Logic
    function showToast(message, isError) {
        var region = document.getElementById('sf-toast-region');
        if (!region) return;

        var t = document.createElement('div');
        // Uses classes defined in your CSS/CSHTML Phase 3 updates [cite: 2, 43]
        t.className = isError
            ? 'sf-flash-banner sf-flash-banner--error'
            : 'sf-flash-banner sf-flash-banner--success';

        t.style.cssText = 'pointer-events:auto; margin-top:.5rem; max-width:280px; opacity:1; transition:opacity .3s;';
        t.setAttribute('role', 'status');
        t.textContent = message;

        region.appendChild(t);
        setTimeout(function () {
            t.style.opacity = '0';
            setTimeout(function () { t.remove(); }, 320);
        }, 2200);
    }

    // AJAX Add-to-Cart stays in application JS. Alpine owns only the stepper.
    document.addEventListener('submit', async function (e) {
        var form = e.target.closest('form.js-add-concession');
        if (!form) return;

        e.preventDefault();
        // Clamp even when a manually edited number has not blurred yet.
        var qtyInput = form.querySelector('input[name="quantity"]');
        if (qtyInput) {
            var parsed = Number.parseInt(qtyInput.value, 10);
            qtyInput.value = String(Math.max(1, Math.min(20, Number.isFinite(parsed) ? parsed : 1)));
        }
        var submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) submitBtn.disabled = true;

        try {
            var resp = await fetch(form.action, {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                body: new FormData(form)
            });

            if (resp.ok) {
                var data = await resp.json();
                // success/message keys from the backend response [cite: 49]
                showToast(data.message || 'Added to cart!', !data.success);
                if (data.success && typeof refreshCartBadge === 'function') {
                    refreshCartBadge();
                }
            } else {
                showToast('Could not add to cart. Please try again.', true);
            }
        } catch (err) {
            showToast('Network error — please try again.', true);
            console.error('AddConcession error:', err);
        } finally {
            if (submitBtn) submitBtn.disabled = false;
        }
    });
})();
