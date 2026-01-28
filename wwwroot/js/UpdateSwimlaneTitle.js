document.addEventListener('click', async (e) => {
    const editBtn = e.target.closest('.edit-swimlane-btn');
    if (editBtn) {
        const swimlaneId = editBtn.dataset.swimlaneId;
        const titleElement = document.getElementById(`swimlane-title-${swimlaneId}`);

        // Enable editing
        titleElement.contentEditable = "true";
        titleElement.classList.add('form-control'); // Bootstrap specific class
        titleElement.focus();

        // Save on blur (clicking away)
        titleElement.addEventListener('blur', () => saveTitle(titleElement, swimlaneId), {once: true});

        // Save on Enter key press
        titleElement.addEventListener('keydown', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                titleElement.blur(); // Triggers the blur event above
            }
        });
    }
});

async function saveTitle(titleElement, swimlaneId) {
    const newTitle = titleElement.innerText.trim();
    titleElement.contentEditable = "false";
    titleElement.classList.remove('form-control');

    if (titleElement.innerText.trim().length < 1 || titleElement.innerText.trim().length > 20) {
        showAlert('Error updating swimlane title: title must be between 1 - 20 characters.', 'warning', true);
        window.location.reload(); // Reload to revert UI
        return;
    }

    try {
        const response = await fetch(`/Swimlane/UpdateSwimlaneTitle?swimlaneId=${swimlaneId}&newSwimlaneName=${encodeURIComponent(newTitle)}`, {
            method: 'PATCH'
        });

        if (!response.ok) {
            console.error("Failed to update swimlane title", response.status);
            window.location.reload(); // Reload to revert UI
        }
    } catch (error) {
        console.error("Failed to update swimlane title", error);
    }
}
