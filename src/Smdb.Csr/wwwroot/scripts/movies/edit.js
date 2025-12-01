
import { $, apiFetch, renderStatus, getQueryParam, captureMovieForm } from
    '/scripts/common.js';
(async function initMovieEdit() {
    const id = getQueryParam('id');
    const form = $('#movie-form');
    const statusEl = $('#status');

    // Disables form fields and do not allow editing if movie id is missing. 

    if (!id) {
        renderStatus(statusEl, 'err', 'Missing ?id in URL.');
        form.querySelectorAll('input,textarea,button,select').forEach(
            el => el.disabled = true);
        return;
    }

    // Populates form with data from movie (id) fetched from the API server. 

    try {
        const m = await apiFetch(`/movies/${encodeURIComponent(id)}`);
        form.title.value = m.title ?? '';
        form.year.value = m.year ?? '';
        form.description.value = m.description ?? '';
        renderStatus(statusEl, 'ok', 'Loaded movie. You can edit and save.');
    } catch (err) {
        const message = err.message.includes('404') 
            ? `Movie with ID ${id} does not exist.`
            : `Failed to load data: ${err.message}`;
        renderStatus(statusEl, 'err', message);
        form.querySelectorAll('input,textarea,button,select').forEach(
            el => el.disabled = true);
        return;
    }

    // Executes the given function whenever the form 'submit' event is triggered. 

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
            const updated = await apiFetch(`/movies/${encodeURIComponent(id)}`, {
                method: 'PUT',
                body: JSON.stringify(payload),
            });
            renderStatus(statusEl, 'ok',
                `Updated movie #${updated.id} "${updated.title}" (${updated.year})).`);
        } catch (err) {
            renderStatus(statusEl, 'err', `Update failed: ${err.message}`);
        }
    });
})(); 