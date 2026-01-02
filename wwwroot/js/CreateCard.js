/** === Create New Card === **/
document.addEventListener('submit', async (e) => {
    
    if (e.target && e.target.classList.contains('create-card-form'))
    {
        e.preventDefault();

        const form = e.target;
        
        const swimlaneId = form.dataset.swimlaneId;
        
        const formData = new FormData();
        const cardTitle = form.querySelector('#card-title').value;
        const cardDescription = form.querySelector('#card-description').value;

        formData.append('cardTitle', cardTitle);
        formData.append('cardDescription', cardDescription);

        const response = await fetch(`Card/CreateNewCard?swimlaneId=${encodeURIComponent(swimlaneId)}`, {
            method: 'POST',
            body: formData
        });

        if (response.status === 201)
        {
            window.location.href = '/';
        } else {
            console.error("Creation of Card failed :/", response.status);
        }
    }
});