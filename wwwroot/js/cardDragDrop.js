import {
        Draggable,
        Sortable,
        Droppable,
        Swappable,
    } from 'https://cdn.jsdelivr.net/npm/@shopify/draggable/build/esm/index.mjs';

const sortable = new Sortable(document.querySelectorAll('.CardDragDropContainer'), {
  draggable: '.CardDragItem--isDraggable',
});

sortable.on('sortable:start', () => console.log('sortable:start'));
sortable.on('sortable:sort', () => console.log('sortable:sort'));

sortable.on('sortable:stop', async (event) => {
    const cardId = event.data.dragEvent.source.dataset.cardId;
    const newSwimlaneId = event.data.newContainer.dataset.swimlaneId;
    const newPosition = event.data.newIndex;

    if (cardId && newSwimlaneId !== undefined && newPosition !== undefined) {
        try {
            const response = await fetch(`/Card/MoveCard?cardId=${cardId}&newSwimlaneId=${newSwimlaneId}&newPosition=${newPosition}`, {
                method: 'PATCH'
            });

            if (!response.ok) {
                console.error('Failed to update card position');
            }
        } catch (error) {
            console.error('Error while updating card position:', error);
        }
    }
});