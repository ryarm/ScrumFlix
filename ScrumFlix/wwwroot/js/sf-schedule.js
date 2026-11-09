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
        if (event.detail.target?.id === 'assignments-grid' &&
            event.detail.requestConfig?.elt?.id === 'assignment-filter-form' &&
            !document.querySelector('#assignment-form [name="AssignmentId"]')) {
            const selected = document.getElementById('assignment-filter-location')?.value;
            const workLocation = document.getElementById('af-location');
            if (selected && workLocation) workLocation.value = selected;
        }
        filterAssignmentShifts();
        if (event.detail.target?.id === 'assignments-grid' || event.detail.target?.id === 'shifts-grid') {
            const selected = document.getElementById('assignment-filter-location')?.value ||
                document.getElementById('shift-filter-location')?.value;
            const fallback = document.getElementById('schedule-location-fallback');
            if (selected && fallback) fallback.value = selected;
            if (selected) {
                document.querySelectorAll('[data-schedule-tab]').forEach(function (link) {
                    const url = new URL(link.href);
                    url.searchParams.set('locationId', selected);
                    link.href = url.toString();
                });
            }
        }
        joinSelectedLocation().catch(err => console.error('Schedule location connection error:', err));
    });

    // A new filter or page size begins at page 1; paging links and CRUD retain
    // their current page because they do not submit these filter forms.
    document.addEventListener('submit', function (event) {
        if (event.target?.id === 'shift-filter-form') {
            event.target.querySelector('[name="filterPage"]').value = '1';
        } else if (event.target?.id === 'assignment-filter-form') {
            event.target.querySelector('[name="assignmentPage"]').value = '1';
        } else {
            return;
        }
        event.target.setAttribute('hx-push-url', event.target.action + '?' +
            new URLSearchParams(new FormData(event.target)));
    }, true);

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

    const endpoints = document.getElementById('schedule-endpoints')?.dataset;
    if (!endpoints) return;
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(endpoints.hubUrl)
        .withAutomaticReconnect()
        .build();

    let joinedLocation = null;

    async function joinSelectedLocation() {
        if (connection.state !== signalR.HubConnectionState.Connected) return;
        const selected = Number(document.getElementById('gantt-location')?.value ||
            document.getElementById('assignment-filter-location')?.value ||
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
            htmx.ajax('GET', endpoints.timelineUrl + '?' + query, {
                target: '#gantt-container', swap: 'innerHTML'
            });
        }
    }

    connection.on('ShiftsUpdated', function () {
        const filters = document.getElementById('shift-filter-form');
        if (filters) {
            htmx.ajax('GET', endpoints.shiftsUrl + '?' + new URLSearchParams(new FormData(filters)), {
                target: '#shifts-grid', swap: 'innerHTML'
            });
        }
        refreshTimeline();
    });

    connection.on('AssignmentsUpdated', function () {
        refreshTimeline();
        if (!document.getElementById('assignments-grid')) return;
        const filters = document.getElementById('assignment-filter-form');
        if (!filters) return;
        htmx.ajax('GET', endpoints.assignmentsUrl + '?' + new URLSearchParams(new FormData(filters)), {
            target: '#assignments-grid',
            swap:   'innerHTML'
        });
    });

    connection.start().then(joinSelectedLocation).catch(function (err) {
        console.error('SignalR connection error:', err);
    });
}());
