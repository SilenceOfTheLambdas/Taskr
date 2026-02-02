function renderMarkdown(rawContent, targetElement) {
    if (!targetElement) return;
    targetElement.innerHTML = DOMPurify.sanitize(marked.parse(rawContent || ''));
}

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.markdown-content').forEach(el => {
        renderMarkdown(el.textContent, el);
    });
    
    document.addEventListener('input', e => {
        if (e.target.classList.contains('markdown-input')) {
            const form = e.target.closest('form');
            const preview = form.querySelector('.markdown-preview');
            renderMarkdown(e.target.value, preview);
        }
    });
    
    document.addEventListener('show.bs.modal', (e) => {
        const modal = e.target;
        const textarea = modal.querySelector('.markdown-input');
        const preview = modal.querySelector('.markdown-preview');
        if (textarea && preview)
            renderMarkdown(textarea.value, preview);
    })
})