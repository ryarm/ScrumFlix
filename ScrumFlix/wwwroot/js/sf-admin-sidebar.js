/** Admin navigation for desktop and narrow screens. Loaded once by _AdminLayout. */
(function () {
    'use strict';

    const storageKey = 'sf-sidebar-open';
    const sidebar = document.getElementById('sf-admin-sidebar');
    const toggle = document.getElementById('sf-sidebar-toggle');
    const backdrop = document.getElementById('sf-sidebar-backdrop');
    if (!sidebar || !toggle || !backdrop) return;

    const mobile = window.matchMedia('(max-width: 991px)');
    let desktopOpen = true;
    try {
        const saved = localStorage.getItem(storageKey);
        desktopOpen = saved === null || saved === '1';
    } catch (_) {
        // Navigation still works if browser storage is unavailable.
    }

    sidebar.setAttribute('tabindex', '-1');

    function setOpen(open) {
        sidebar.classList.toggle('sf-sidebar-collapsed', !open);
        sidebar.inert = !open;
        sidebar.setAttribute('aria-hidden', String(!open));
        toggle.setAttribute('aria-expanded', String(open));
        toggle.setAttribute('aria-label', open ? 'Close staff navigation' : 'Open staff navigation');
        backdrop.hidden = !mobile.matches || !open;
    }

    setOpen(mobile.matches ? false : desktopOpen);

    toggle.addEventListener('click', function () {
        const open = sidebar.classList.contains('sf-sidebar-collapsed');
        setOpen(open);
        if (mobile.matches && open) {
            sidebar.focus();
        } else if (!mobile.matches) {
            desktopOpen = open;
            try { localStorage.setItem(storageKey, open ? '1' : '0'); } catch (_) { /* optional */ }
        }
    });

    backdrop.addEventListener('click', function () {
        setOpen(false);
        toggle.focus();
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && mobile.matches && !backdrop.hidden) {
            setOpen(false);
            toggle.focus();
        }
    });

    mobile.addEventListener('change', function () {
        setOpen(mobile.matches ? false : desktopOpen);
    });
}());
