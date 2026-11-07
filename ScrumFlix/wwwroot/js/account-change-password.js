/*
 * File: /wwwroot/js/account-change-password.js
 * Description:
 *     Handles validation summary visibility for the ChangePassword view.
 *     Alpine owns password visibility; keep this external file for the
 *     existing invalid-POST summary behavior.
 */

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {

        // Validation summary visibility
        var summary = document.querySelector('[data-valmsg-summary]');

        if (summary && summary.querySelectorAll('li').length > 0) {
            summary.style.removeProperty('display');
        }
    });
})();
