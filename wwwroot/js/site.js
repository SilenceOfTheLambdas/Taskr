// Show Alerts
function showAlert(message, type, persist = false) {
    if (persist) {
        localStorage.setItem('pendingAlert', JSON.stringify({message: message, type: type}));
        return;
    }

    const alertPlaceholder = document.getElementById('live-alert-placeholder')
    if (!alertPlaceholder) return;

    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible fade show" role="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
        '</div>'
    ].join('')

    alertPlaceholder.append(wrapper)
}

document.addEventListener('DOMContentLoaded', () => {
    const pendingAlert = localStorage.getItem('pendingAlert');
    if (pendingAlert) {
        const {message, type} = JSON.parse(pendingAlert);
        showAlert(message, type);
        localStorage.removeItem('pendingAlert');
    }
});

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

// Revert back to the + button if the user presses Escape
form.addEventListener('keydown', (e) => {
    if (e.code === "Escape") {
        form.classList.add('d-none');
        addBtn.classList.remove('d-none');
    }
});

// Handle new tag input and creation
const newTagButton = document.getElementById('new-tag-button');
const newTagForm = document.getElementById('add-tag-form');
const tagNameInput = document.getElementById('tag-name-input');

newTagButton.addEventListener('click', () => {
    newTagButton.classList.add('d-none');
    newTagForm.classList.remove('d-none');
    tagNameInput.focus();
})

newTagForm.addEventListener('submit', async (e) => {
    e.preventDefault()

    const boardId = newTagForm.dataset.boardId;
    
    // Form Data
    const formData = new FormData();
    const tagName = tagNameInput.value;

    formData.append('tagName', tagName);
    
    const response = await fetch(`/Board/CreateNewTag?boardId=${encodeURIComponent(boardId)}`, {
        method: 'POST',
        body: formData
    });

    if (response.status === 201) {
        // Redirect
        window.location.href = '/';
    } else {
        // An error has occurred
        console.error("Creation of Swimlane failed", response.status)
        showAlert("Failed to create new tag. Please try again.", "danger", true);
    }
});

// Revert back when the user presses Escape
newTagForm.addEventListener('keydown', (e) => {
    if (e.code === "Escape") {
        newTagForm.classList.add('d-none');
        newTagButton.classList.remove('d-none');
    }
});

// Close the tags panel when clicking outside it and make sure to reset form status
document.addEventListener('mouseup', function(e) {
    const panel = document.getElementById("tags-panel");
    if (!panel.contains(e.target)) {
        panel.hidden = true;
        newTagForm.classList.add('d-none');
        newTagButton.classList.remove('d-none');
    }
});
