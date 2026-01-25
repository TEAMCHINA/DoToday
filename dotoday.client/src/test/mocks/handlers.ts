import { http, HttpResponse } from 'msw';

// Default mock data
export const mockLists = [
  { id: 1, name: 'Shopping List' },
  { id: 2, name: 'Work Tasks' },
];

export const mockListWithTasks = {
  id: 1,
  name: 'Shopping List',
  tasks: [
    { id: 1, title: 'Buy milk', isCompleted: false, listId: 1 },
    { id: 2, title: 'Buy bread', isCompleted: true, listId: 1 },
  ],
};

let listsData = [...mockLists];
let listDetailData = { ...mockListWithTasks };
let nextListId = 3;
let nextTaskId = 3;

export function resetMockData() {
  listsData = [...mockLists];
  listDetailData = {
    id: 1,
    name: 'Shopping List',
    tasks: [
      { id: 1, title: 'Buy milk', isCompleted: false, listId: 1 },
      { id: 2, title: 'Buy bread', isCompleted: true, listId: 1 },
    ],
  };
  nextListId = 3;
  nextTaskId = 3;
}

export const handlers = [
  // GET /api/Lists - Get all lists
  http.get('/api/Lists', () => {
    return HttpResponse.json({ lists: listsData });
  }),

  // POST /api/Lists - Create a new list
  http.post('/api/Lists', async ({ request }) => {
    const body = (await request.json()) as { name: string };
    const newList = {
      id: nextListId++,
      name: body.name,
      tasks: [],
    };
    listsData.push({ id: newList.id, name: newList.name });
    return HttpResponse.json({ list: newList });
  }),

  // GET /api/Lists/:id - Get a specific list
  http.get('/api/Lists/:id', ({ params }) => {
    const id = parseInt(params.id as string, 10);
    if (id === listDetailData.id) {
      return HttpResponse.json({ list: listDetailData });
    }
    // Return null list for not found
    return HttpResponse.json({ list: null });
  }),

  // PUT /api/Lists/:id - Update a list
  http.put('/api/Lists/:id', async ({ params, request }) => {
    const id = parseInt(params.id as string, 10);
    const body = (await request.json()) as { name: string };
    const listIndex = listsData.findIndex((l) => l.id === id);
    if (listIndex !== -1) {
      listsData[listIndex].name = body.name;
    }
    if (id === listDetailData.id) {
      listDetailData.name = body.name;
    }
    return HttpResponse.json({ list: { id, name: body.name, tasks: [] } });
  }),

  // DELETE /api/Lists/:id - Delete a list
  http.delete('/api/Lists/:id', ({ params }) => {
    const id = parseInt(params.id as string, 10);
    listsData = listsData.filter((l) => l.id !== id);
    return new HttpResponse(null, { status: 200 });
  }),

  // POST /api/lists/:listId/Tasks - Create a task
  http.post('/api/lists/:listId/Tasks', async ({ params, request }) => {
    const listId = parseInt(params.listId as string, 10);
    const body = (await request.json()) as { title: string };
    const newTask = {
      id: nextTaskId++,
      title: body.title,
      isCompleted: false,
      listId,
    };
    if (listId === listDetailData.id) {
      listDetailData.tasks.push(newTask);
    }
    return HttpResponse.json({ task: newTask });
  }),

  // PUT /api/lists/:listId/Tasks/:taskId - Update a task
  http.put('/api/lists/:listId/Tasks/:taskId', async ({ params, request }) => {
    const listId = parseInt(params.listId as string, 10);
    const taskId = parseInt(params.taskId as string, 10);
    const body = (await request.json()) as { isCompleted: boolean };

    if (listId === listDetailData.id) {
      const task = listDetailData.tasks.find((t) => t.id === taskId);
      if (task) {
        task.isCompleted = body.isCompleted;
        return HttpResponse.json({ task });
      }
    }
    return HttpResponse.json({ task: { id: taskId, isCompleted: body.isCompleted, listId } });
  }),
];
