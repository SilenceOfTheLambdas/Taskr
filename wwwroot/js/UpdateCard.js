/** === Update Card Details === **/
document.addEventListener('submit', async (e) => {

    if (e.target && e.target.classList.contains('update-card-form')) {
        e.preventDefault();

        const form = e.target;

        const cardId = form.dataset.cardId;

        const formData = new FormData();
        const cardTitle = form.querySelector('#card-title').value;
        const cardDescription = form.querySelector('#card-description').value;

        formData.append('cardTitle', cardTitle);
        formData.append('cardDescription', cardDescription);

        const response = await fetch(`Card/UpdateCardDetails?cardId=${encodeURIComponent(cardId)}`, {
            method: 'PATCH',
            body: formData
        });

        if (response.status === 204) {
            window.location.href = '/';
        } else {
            console.error("Updating of Card failed :/", response.status);
        }
    }
});