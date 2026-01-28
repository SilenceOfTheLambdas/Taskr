import {
    Draggable,
    Sortable,
    Droppable,
    Swappable,
} from 'https://cdn.jsdelivr.net/npm/@shopify/draggable/build/esm/index.mjs';

const sortable = new Sortable(document.querySelectorAll('.SwimlaneContainer'), {
    draggable: '.SwimlaneItem--isDraggable',
    handle: '.SwimlaneItem--handle',
    distance: 10 // Allows for button presses as a drag event will only trigger if the mouse is moved
});

sortable.on('sortable:start', () => console.log('sortable:start'));
sortable.on('sortable:sort', () => console.log('sortable:sort'));

sortable.on('sortable:stop', async (event) => {
    const swimlaneId = event.data.dragEvent.source.dataset.swimlaneId;
    const newPosition = event.data.newIndex;

    if (newPosition !== undefined) {
        try {
            const response = await
                fetch(`/Swimlane/UpdateSwimlanePosition?swimlaneId=${swimlaneId}&newPosition=${newPosition}`,
                    {
                        method: 'PATCH'
                    });

            if (!response.ok) {
                console.error('Failed to update swimlane position');
            }
        } catch (error) {
            console.error('Error while updating swimlane position:', error);
        }
    }
});