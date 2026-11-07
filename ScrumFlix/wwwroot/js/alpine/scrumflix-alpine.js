/* ScrumFlix Alpine components — v1.0, 2026-09-26.
 * Register before the deferred @alpinejs/csp core script starts.
 * MVC keeps form ownership; these components only manage local UI state.
 */
document.addEventListener('alpine:init', function () {
    'use strict';

    function clampInteger(value, min, max) {
        var parsed = Number.parseInt(value, 10);
        return Math.max(min, Math.min(max, Number.isFinite(parsed) ? parsed : min));
    }

    Alpine.data('passwordVisibility', function () {
        return {
            shown: false,
            field: 'password',
            init: function () {
                this.field = this.$el.dataset.passwordLabel || 'password';
            },
            get inputType() { return this.shown ? 'text' : 'password'; },
            get label() { return (this.shown ? 'Hide ' : 'Show ') + this.field; },
            get iconClass() { return this.shown ? 'bi-eye-slash' : 'bi-eye'; },
            toggle: function () { this.shown = !this.shown; }
        };
    });

    Alpine.data('ticketQuantity', function () {
        return {
            qty: 1,
            min: 1,
            max: 20,
            price: 0,
            input: null,
            total: null,
            quantityBridge: null,
            init: function () {
                this.input = this.$el.querySelector('#qtyInput');
                this.total = this.$el.querySelector('#lineTotal');
                var priceElement = this.$el.querySelector('#priceDisplay');
                if (!this.input || !priceElement) return;
                this.min = Number.parseInt(this.input.min, 10) || 1;
                this.max = Number.parseInt(this.input.max, 10) || 20;
                this.price = Number.parseFloat(priceElement.dataset.price) || 0;
                var self = this;
                this.quantityBridge = function () { return self.qty; };
                window.getQty = this.quantityBridge; // seatPicker.js compatibility
                this.setQuantity(this.input.value);
            },
            setQuantity: function (value) {
                if (!this.input) return;
                this.qty = clampInteger(value, this.min, this.max);
                this.input.value = String(this.qty);
                if (this.total) this.total.textContent = '$' + (this.qty * this.price).toFixed(2);
                this.input.dispatchEvent(new CustomEvent('qtychanged', { bubbles: false }));
            },
            decrement: function () { this.setQuantity(this.qty - 1); },
            increment: function () { this.setQuantity(this.qty + 1); },
            destroy: function () {
                if (window.getQty === this.quantityBridge) delete window.getQty;
            }
        };
    });

    Alpine.data('cartQuantity', function () {
        return {
            form: null,
            input: null,
            lastValid: 1,
            submitting: false,
            init: function () {
                this.form = this.$el;
                this.input = this.form.querySelector('input[name="quantity"]');
                if (this.input) this.lastValid = clampInteger(this.input.value, 0, 20);
            },
            submitQuantity: function (value) {
                if (!this.input || this.submitting) return;
                var parsed = Number.parseInt(value, 10);
                var next = Number.isFinite(parsed) ? clampInteger(parsed, 0, 20) : this.lastValid;
                this.input.value = String(next);
                this.lastValid = next;
                this.submitting = true;
                this.form.requestSubmit();
            },
            decrement: function () {
                this.submitQuantity(this.lastValid - 1);
            },
            increment: function () {
                this.submitQuantity(this.lastValid + 1);
            },
            changed: function () { this.submitQuantity(this.input.value); }
        };
    });

    Alpine.data('concessionQuantity', function () {
        return {
            input: null,
            init: function () { this.input = this.$el.querySelector('input[name="quantity"]'); },
            setQuantity: function (value) {
                if (this.input && !this.input.disabled) {
                    this.input.value = String(clampInteger(value, 1, 20));
                }
            },
            decrement: function () {
                this.setQuantity((Number.parseInt(this.input.value, 10) || 1) - 1);
            },
            increment: function () {
                this.setQuantity((Number.parseInt(this.input.value, 10) || 1) + 1);
            },
            changed: function () { this.setQuantity(this.input.value); }
        };
    });

    Alpine.data('confirmation', function () {
        return {
            form: null,
            confirming: false,
            approved: false,
            opener: null,
            prompt: '',
            init: function () {
                this.form = this.$el;
                this.prompt = this.form.dataset.confirmPrompt || 'Are you sure?';
            },
            open: function (event) {
                this.opener = event.currentTarget;
                this.confirming = true;
                this.$nextTick(function () {
                    var yes = this.form.querySelector('[data-confirm-yes]');
                    if (yes) yes.focus();
                }.bind(this));
            },
            handleSubmit: function (event) {
                if (this.approved) {
                    this.approved = false;
                    this.confirming = false;
                    return;
                }
                event.preventDefault();
                // An Enter key or programmatic submission cannot bypass confirmation.
                this.open({ currentTarget: event.submitter || document.activeElement });
            },
            confirm: function () {
                this.approved = true;
                this.form.requestSubmit();
            },
            cancel: function () {
                if (!this.confirming) return;
                this.confirming = false;
                if (this.opener && this.opener.isConnected) {
                    this.$nextTick(function () { this.opener.focus(); }.bind(this));
                }
            }
        };
    });
});
