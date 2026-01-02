const addBtn = document.getElementById('add-swimlane-btn');
const form = document.getElementById('add-swimlane-form');
const input = document.getElementById('swimlane-name-input');

// Show input when the + button is pressed
addBtn.addEventListener('click', () => {
    addBtn.classList.add('d-none');
    form.classList.remove('d-none');
    input.focus();
})

form.addEventListener('submit', async (e) => {
    e.preventDefault()

    const swimlaneName = input.value;
    const response = await fetch(`/Swimlane/CreateNewSwimlane?swimlaneName=${encodeURIComponent(swimlaneName)}`, {
        method: 'POST'
    });

    if (response.status === 201) {
        // Redirect
        window.location.href = '/';
    } else {
        // An error has occurred
        console.error("Creation of Swimlane failed", response.status)
    }
});

// Revert back to + button if user presses Escape
form.addEventListener('keydown', (e) => {
    if (e.code === "Escape") {
        form.classList.add('d-none');
        addBtn.classList.remove('d-none');
    }
});

// CREATE CARD FORM
