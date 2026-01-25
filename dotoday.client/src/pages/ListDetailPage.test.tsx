import { describe, it, expect, beforeEach } from 'vitest';
import userEvent from '@testing-library/user-event';
import { render, screen, waitFor } from '@/test/utils';
import { Routes, Route } from 'react-router-dom';
import { http, HttpResponse } from 'msw';
import { server } from '@/test/mocks/server';
import { resetMockData } from '@/test/mocks/handlers';
import { ListDetailPage } from './ListDetailPage';

function renderWithRoute(listId: string = '1') {
  return render(
    <Routes>
      <Route path="/lists/:listId" element={<ListDetailPage />} />
    </Routes>,
    {
      routerProps: {
        initialEntries: [`/lists/${listId}`],
      },
    }
  );
}

describe('ListDetailPage', () => {
  beforeEach(() => {
    resetMockData();
  });

  it('shows loading state initially', () => {
    renderWithRoute();
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('renders list name and tasks', async () => {
    renderWithRoute();

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Shopping List' })).toBeInTheDocument();
    });

    expect(screen.getByText('Buy milk')).toBeInTheDocument();
    expect(screen.getByText('Buy bread')).toBeInTheDocument();
  });

  it('handles "not found" state', async () => {
    server.use(
      http.get('/api/Lists/:id', () => {
        return HttpResponse.json({ list: null });
      })
    );

    renderWithRoute('999');

    await waitFor(() => {
      expect(screen.getByText('List not found')).toBeInTheDocument();
    });

    expect(screen.getByRole('link', { name: /Back to Lists/i })).toBeInTheDocument();
  });

  it('adds new task via AddItemInput', async () => {
    const user = userEvent.setup();
    renderWithRoute();

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Shopping List' })).toBeInTheDocument();
    });

    const input = screen.getByPlaceholderText('New task');
    await user.type(input, 'Buy eggs');

    const addButton = screen.getByRole('button', { name: '' });
    await user.click(addButton);

    await waitFor(() => {
      expect(screen.getByText('Buy eggs')).toBeInTheDocument();
    });
  });

  it('toggles task completion on checkbox click', async () => {
    const user = userEvent.setup();
    renderWithRoute();

    await waitFor(() => {
      expect(screen.getByText('Buy milk')).toBeInTheDocument();
    });

    // Find the checkbox for "Buy milk" (first unchecked item)
    const checkboxes = screen.getAllByRole('checkbox');
    const uncheckedCheckbox = checkboxes.find(
      (checkbox) => !(checkbox as HTMLInputElement).checked
    );
    expect(uncheckedCheckbox).toBeDefined();

    // Click the list item button to toggle
    const buyMilkItem = screen.getByText('Buy milk').closest('li');
    const button = buyMilkItem?.querySelector('div[role="button"]');
    if (button) {
      await user.click(button);
    }

    await waitFor(() => {
      const updatedCheckboxes = screen.getAllByRole('checkbox');
      // First checkbox should now be checked
      expect((updatedCheckboxes[0] as HTMLInputElement).checked).toBe(true);
    });
  });

  it('back button navigates to lists page', async () => {
    renderWithRoute();

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Shopping List' })).toBeInTheDocument();
    });

    const backButton = screen.getByRole('link', { name: /Back to Lists/i });
    expect(backButton).toHaveAttribute('href', '/');
  });

  it('handles API errors gracefully', async () => {
    server.use(
      http.get('/api/Lists/:id', () => {
        return new HttpResponse(null, { status: 500 });
      })
    );

    renderWithRoute();

    // Should show loading initially
    expect(screen.getByText('Loading...')).toBeInTheDocument();

    // When query fails, it returns undefined/null data, showing "not found"
    await waitFor(() => {
      expect(screen.getByText('List not found')).toBeInTheDocument();
    });
  });

  it('shows empty tasks message when list has no tasks', async () => {
    server.use(
      http.get('/api/Lists/:id', () => {
        return HttpResponse.json({
          list: {
            id: 1,
            name: 'Empty List',
            tasks: [],
          },
        });
      })
    );

    renderWithRoute();

    await waitFor(() => {
      expect(screen.getByText('No tasks yet. Add one below!')).toBeInTheDocument();
    });
  });

  it('displays completed tasks with checked checkbox', async () => {
    renderWithRoute();

    await waitFor(() => {
      expect(screen.getByText('Buy bread')).toBeInTheDocument();
    });

    // Buy bread is marked as completed in mock data
    const checkboxes = screen.getAllByRole('checkbox');
    // Find the checked checkbox (Buy bread is completed)
    const completedCheckbox = checkboxes.find(
      (checkbox) => (checkbox as HTMLInputElement).checked
    );
    expect(completedCheckbox).toBeDefined();

    // Verify uncompleted task has unchecked checkbox
    const uncompletedCheckbox = checkboxes.find(
      (checkbox) => !(checkbox as HTMLInputElement).checked
    );
    expect(uncompletedCheckbox).toBeDefined();
  });
});
