/* Telerik locations grid. Keep the server-rendered table visible until data has loaded. */
(function () {
    'use strict';
    document.addEventListener('DOMContentLoaded', function () {
        var element = document.getElementById('locations-grid');
        var fallback = document.getElementById('locations-fallback');
        var $ = window.jQuery;
        if (!element || !$ || !$.fn.kendoGrid || !window.kendo) return;
        var readUrl = element.dataset.readUrl;
        var editUrl = element.dataset.editUrl;
        var screensUrl = element.dataset.screensUrl;
        var encode = window.kendo.htmlEncode;
        $(element).kendoGrid({
            dataSource: {
                transport: { read: { url: readUrl, dataType: 'json', type: 'GET' } },
                schema: { model: { id: 'locationId', fields: {
                    locationId: { type: 'number' }, locationName: { type: 'string' },
                    locationAddress: { type: 'string' }, screenCount: { type: 'number' },
                    timeZoneId: { type: 'string' }, isActive: { type: 'boolean' }
                } } },
                pageSize: 10,
                error: function () { element.hidden = true; fallback.hidden = false; }
            },
            sortable: true,
            filterable: true,
            pageable: { refresh: true, pageSizes: [10, 25, 50], buttonCount: 5 },
            noRecords: { template: function () { return 'No locations found.'; } },
            columns: [
                { field: 'locationName', title: 'Name' },
                { field: 'locationAddress', title: 'Address', template: function (row) { return encode(row.locationAddress || '—'); } },
                { field: 'screenCount', title: 'Screens', width: 110, attributes: { 'class': 'text-end' } },
                { field: 'timeZoneId', title: 'Time Zone', template: function (row) { return encode(row.timeZoneId || '—'); } },
                { field: 'isActive', title: 'Status', width: 120, template: function (row) {
                    return row.isActive ? '<span class="badge sf-badge-ok">Active</span>' : '<span class="badge sf-badge-oos">Inactive</span>';
                } },
                { title: 'Actions', width: 180, sortable: false, filterable: false, template: function (row) {
                    var name = encode(row.locationName || '');
                    return '<a class="btn btn-sm sf-btn-outline me-2" href="' + encode(editUrl + '?id=' + encodeURIComponent(row.locationId)) + '" aria-label="Edit ' + name + '">Edit</a>' +
                        '<a class="btn btn-sm sf-btn-ghost" href="' + encode(screensUrl + '?locationId=' + encodeURIComponent(row.locationId)) + '" aria-label="Screens for ' + name + '">Screens</a>';
                } }
            ],
            dataBound: function () { element.hidden = false; fallback.hidden = true; }
        });
    });
}());
