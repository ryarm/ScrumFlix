/**
 * File:    /wwwroot/js/sf-schedule.js
 * Purpose: SignalR hub connection and HTMX refresh triggers for the Admin
 *          Schedule page (Schedule/Index.cshtml).
 *
 * Replaces the inline <script> block formerly in Schedule/Index.cshtml.
 * Moved to an external file to comply with the CSP script-src directive,
 * which does not include 'unsafe-inline'.
 *
 * Dependencies (must be loaded before this file):
 *   - signalr.min.js  (/js/signalr/dist/browser/signalr.min.js)
 *   - htmx            (cdn.jsdelivr.net — already in _Layout.cshtml)
 *
 * Behaviour:
 *   - Connects to /scheduleHub via SignalR.
 *   - On "ShiftsUpdated": refreshes the #shifts-grid partial via HTMX.
 *   - On "AssignmentsUpdated": refreshes the #assignments-grid partial,
 *     passing the currently selected locationId from the Gantt dropdown.
 */
(function () {
    'use strict';

    // Stop nested delete clicks at their cell, before the parent row's hx-get.
    // Rebind only new cells after HTMX swaps; existing cells are marked once.
    function bindActionCells(root) {
        root.querySelectorAll('[data-sf-stop-row-click]').forEach(function (cell) {
            if (cell.dataset.sfRowClickBound) return;
            cell.dataset.sfRowClickBound = 'true';
            cell.addEventListener('click', function (event) { event.stopPropagation(); });
        });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () { bindActionCells(document); });
    } else {
        bindActionCells(document);
    }
    document.addEventListener('htmx:afterSwap', function (event) {
        bindActionCells(event.detail.target);
        filterAssignmentShifts();
        if (event.detail.target?.id === 'assignments-grid' &&
            event.detail.requestConfig?.elt?.id === 'assignment-form') {
            const selected = document.getElementById('af-location')?.value;
            const filter = document.getElementById('assignment-location-filter');
            const fallback = document.getElementById('schedule-location-fallback');
            if (selected && filter) filter.value = selected;
            if (selected && fallback) fallback.value = selected;
        }
        joinSelectedLocation().catch(err => console.error('Schedule location connection error:', err));
    });

    function filterAssignmentShifts() {
        const location = document.getElementById('af-location')?.value;
        const shift = document.getElementById('af-shift');
        if (!shift) return;
        for (const option of shift.options) {
            if (!option.value) continue;
            option.disabled = !location || option.dataset.location !== location;
        }
        if (shift.selectedOptions[0]?.disabled) shift.value = '';
    }
    document.addEventListener('change', function (event) {
        if (event.target?.id === 'af-location') filterAssignmentShifts();
    });
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', filterAssignmentShifts);
    } else {
        filterAssignmentShifts();
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/scheduleHub')
        .withAutomaticReconnect()
        .build();

    let joinedLocation = null;

    async function joinSelectedLocation() {
        if (connection.state !== signalR.HubConnectionState.Connected) return;
        const selected = Number(document.getElementById('gantt-location')?.value ||
            document.getElementById('shift-filter-location')?.value ||
            document.getElementById('schedule-location-fallback')?.value);
        if (!Number.isSafeInteger(selected) || selected <= 0) return;
        if (joinedLocation === selected) return;
        if (joinedLocation !== null) {
            await connection.invoke('LeaveLocationGroup', joinedLocation);
        }
        joinedLocation = null;
        await connection.invoke('JoinLocationGroup', selected);
        joinedLocation = selected;
    }

    document.getElementById('gantt-location')?.addEventListener('change', function () {
        joinSelectedLocation().catch(err => console.error('Schedule location connection error:', err));
    });
    connection.onreconnecting(function () { joinedLocation = null; });
    connection.onreconnected(function () {
        joinSelectedLocation().catch(err => console.error('Schedule location connection error:', err));
    });

    function refreshTimeline() {
        const locationId = document.getElementById('gantt-location')?.value;
        const period = document.getElementById('gantt-month')?.value;
        if (locationId && period) {
            const query = new URLSearchParams({ locationId, period });
            const weekStart = document.getElementById('gantt-week-start')?.value;
            if (weekStart) query.set('weekStart', weekStart);
            htmx.ajax('GET', '/Admin/Schedule/ScheduleVisual?' + query, {
                target: '#gantt-container', swap: 'innerHTML'
            });
        }
    }

    connection.on('ShiftsUpdated', function () {
        const filters = document.getElementById('shift-filter-form');
        if (filters) {
            htmx.ajax('GET', '/Admin/Schedule/ShiftsGrid?' + new URLSearchParams(new FormData(filters)), {
                target: '#shifts-grid', swap: 'innerHTML'
            });
        }
        refreshTimeline();
    });

    connection.on('AssignmentsUpdated', function () {
        refreshTimeline();
        if (!document.getElementById('assignments-grid')) return;
        const locEl = document.getElementById('gantt-location');
        const locId = locEl ? locEl.value : document.getElementById('schedule-location-fallback')?.value;
        htmx.ajax('GET', '/Admin/Schedule/AssignmentsGrid?locationId=' + locId, {
            target: '#assignments-grid',
            swap:   'innerHTML'
        });
    });

    connection.start().then(joinSelectedLocation).catch(function (err) {
        console.error('SignalR connection error:', err);
    });
}());
