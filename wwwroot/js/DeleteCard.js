document.addEventListener('submit', async (e) => {

    if (e.target && e.target.classList.contains('delete-card-form'))
    {
        e.preventDefault();

        const form = e.target;
        const cardId = form.dataset.cardId;

        const response = await fetch(`Card/DeleteCard?cardId=${encodeURIComponent(cardId)}`, {
            method: 'DELETE'
        });

        if (response.status === 204)
        {
            window.location.href = '/';
        } else {
            console.error("Deletion of Card failed :/", response.status);
        }
    }
});