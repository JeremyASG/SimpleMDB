import { $, apiFetch, renderStatus, captureMovieForm } from '/scripts/common.js';

(async function initMovieAdd() {
    const form = $('#movie-form');
    const statusEl = $('#status');

    renderStatus(statusEl, 'ok', 'New movie. You can edit and save.');

    form.addEventListener('submit', async (ev) => {

        ev.preventDefault();
        const payload = captureMovieForm(form);
        
        // Input validation
        if (!payload.title || payload.title.trim().length === 0) {
            renderStatus(statusEl, 'err', 'Title is required.');
            return;
        }
        
        if (payload.title.length > 50) {
            renderStatus(statusEl, 'err', 'Title should not be longer than 50 characters.');
            return;
        }
        
        if (!payload.year || payload.year < 1888 || payload.year > new Date().getFullYear()) {
            renderStatus(statusEl, 'err', `Year must be between 1888 and ${new Date().getFullYear()}.`);
            return;
        }
        
        if (payload.description && payload.description.length > 100) {
            renderStatus(statusEl, 'err', 'Description should not be longer than 100 characters.');
            return;
        }

        try {
            const created = await apiFetch(
                '/movies', { method: 'POST', body: JSON.stringify(payload) });
            renderStatus(statusEl, 'ok',
                `Created movie #${created.id} "${created.title}" (${created.year}).`);
            form.reset();
        } catch (err) {
            renderStatus(statusEl, 'err', `Create failed: ${err.message}`);
        }
    });
})();