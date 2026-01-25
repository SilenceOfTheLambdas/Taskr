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
sortable.on('sortable:sorted', () => console.log('sortable:sorted'));
sortable.on('sortable:stop', () => console.log('sortable:stop'));