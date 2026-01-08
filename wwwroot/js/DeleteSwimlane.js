document.addEventListener('submit', async (e) => {

    if (e.target && e.target.classList.contains('delete-swimlane-form'))
    {
        e.preventDefault();

        const form = e.target;
        const swimlaneId = form.dataset.swimlaneId;

        try {
            const response = await fetch(`Swimlane/DeleteSwimlane?swimlaneId=${encodeURIComponent(swimlaneId)}`, {
                method: 'DELETE'
            });

            if (response.status === 204)
            {
                window.location.href = '/';
            } else {
                console.error("Deletion of swimlane failed :/", response.status);
            }
        } catch (error) {
            console.error("Deletion of swimlane failed :/", error);
        }
        
    }
});