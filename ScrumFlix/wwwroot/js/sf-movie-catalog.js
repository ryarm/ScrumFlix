/* Catalog filters: native GET form fallback with Telerik search and cascade when available. */
(function () {
    'use strict';
    document.addEventListener('DOMContentLoaded', function () {
        var root = document.getElementById('movieCatalog');
        var form = document.getElementById('cascadeForm');
        var genre = document.getElementById('genreDropdown');
        var movie = document.getElementById('movieDropdown');
        var search = document.getElementById('titleSearchInput');
        if (!root || !form || !genre || !movie) return;
        var $ = window.jQuery;
        var kendoReady = !!($ && $.fn.kendoDropDownList && $.fn.kendoComboBox && $.fn.kendoAutoComplete);
        var selectedMovie = movie.dataset.selectedMovie || '';
        var movieWidget;
        function setMovies(rows, selected) {
            if (movieWidget) {
                // Kendo owns the select once initialized; replacing its options breaks its popup.
                movieWidget.setDataSource(new window.kendo.data.DataSource({ data: rows.map(function (row) {
                    return { id: String(row.id), name: row.name };
                }) }));
                movieWidget.enable(!!genre.value);
                movieWidget.value(selected || '');
                return;
            }
            movie.replaceChildren();
            var placeholder = document.createElement('option');
            placeholder.value = '';
            placeholder.textContent = genre.value ? '— Select a Movie —' : '— Select Genre First —';
            movie.appendChild(placeholder);
            rows.forEach(function (row) {
                var option = document.createElement('option');
                option.value = String(row.id);
                option.textContent = row.name;
                movie.appendChild(option);
            });
            movie.disabled = !genre.value;
            movie.value = selected || '';
        }
        function loadMovies(selected) {
            if (!genre.value) { setMovies([], ''); return; }
            fetch(root.dataset.moviesUrl + '?genre=' + encodeURIComponent(genre.value))
                .then(function (response) { if (!response.ok) throw new Error('Movie list unavailable'); return response.json(); })
                .then(function (rows) { setMovies(rows, selected); })
                .catch(function () { setMovies([], ''); });
        }
        if (kendoReady) {
            genre.classList.remove('sf-select', 'sf-cascade-select');
            movie.classList.remove('sf-select', 'sf-cascade-select');
            $(genre).kendoDropDownList({ filter: 'contains', change: function () { loadMovies(''); } });
            $(movie).kendoComboBox({ dataTextField: 'name', dataValueField: 'id', placeholder: '— Select a Movie —', filter: 'contains', suggest: true,
                select: function (event) {
                    var selected = this.dataItem(event.item.index());
                    var id = selected && String(selected.id);
                    if (id && /^[1-9]\d*$/.test(id)) {
                        window.location.assign(root.dataset.detailUrl + '?id=' + encodeURIComponent(id));
                    }
                },
                change: function () {
                // Reject arbitrary typed text: only an actual movie ID can navigate.
                if (!this.value() || !/^\d+$/.test(this.value())) this.value('');
            } });
            movieWidget = $(movie).data('kendoComboBox');
            movieWidget.enable(!!genre.value);
            if (search) {
                $(search).kendoAutoComplete({
                    minLength: 2, filter: 'contains', delay: 250,
                    dataTextField: 'title',
                    select: function (event) {
                        var title = this.dataItem(event.item.index());
                        if (title && Number.isInteger(title.id) && title.id > 0) {
                            window.location.assign(root.dataset.detailUrl + '?id=' + encodeURIComponent(title.id));
                        }
                    },
                    dataSource: { serverFiltering: true, transport: { read: {
                        url: root.dataset.suggestionsUrl, dataType: 'json',
                        data: function () { return { term: $(search).data('kendoAutoComplete').value(), genre: genre.value }; }
                    } } }
                });
            }
        } else {
            genre.addEventListener('change', function () { loadMovies(''); });
        }
        form.addEventListener('submit', function (event) {
            if (movieWidget) {
                var value = movieWidget.value();
                if (value && !/^\d+$/.test(value)) { event.preventDefault(); movieWidget.value(''); return; }
                movie.disabled = !value;
                movie.value = value;
            }
        });
        loadMovies(selectedMovie);
    });
}());
