/*
 * File: /ScrumFlix/wwwroot/js/scrumflix.js
 * Description: Client-side JavaScript for ScrumFlix — cart badge updates, seat hover effects,
 *              and general UX enhancements.
 */

'use strict';

document.addEventListener('DOMContentLoaded', function () {

    // ── Auto-dismiss flash banners ────────────────────────────────────────────
    const flash = document.querySelector('.sf-flash-banner');
    if (flash) {
        setTimeout(() => {
            flash.style.transition = 'opacity 0.5s ease';
            flash.style.opacity = '0';
            setTimeout(() => flash.remove(), 500);
        }, 4000);
    }

    // ── Seat hover glow ───────────────────────────────────────────────────────
    document.querySelectorAll('.sf-seat-open').forEach(seat => {
        seat.addEventListener('mouseenter', function () {
            this.style.background = 'var(--sf-gold)';
            this.style.borderColor = 'var(--sf-gold-dim)';
        });
        seat.addEventListener('mouseleave', function () {
            this.style.background = '';
            this.style.borderColor = '';
        });
    });

    // ── Navbar active link highlight ──────────────────────────────────────────
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sf-nav-link').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase() || '';
        if (href && currentPath.startsWith(href) && href !== '/') {
            link.classList.add('active');
        }
    });

    // ── Smooth scroll for on-page anchors ────────────────────────────────────
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });

    // ── Concession card qty keyboard support ─────────────────────────────────
    document.querySelectorAll('.sf-qty-input').forEach(input => {
        input.addEventListener('keydown', function (e) {
            if (e.key === 'ArrowUp') { e.preventDefault(); this.value = Math.min(20, parseInt(this.value || 1) + 1); }
            if (e.key === 'ArrowDown') { e.preventDefault(); this.value = Math.max(1, parseInt(this.value || 1) - 1); }
        });
    });

});
