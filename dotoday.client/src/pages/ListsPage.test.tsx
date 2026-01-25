import { describe, it, expect, beforeEach } from 'vitest';
import userEvent from '@testing-library/user-event';
import { render, screen, waitFor } from '@/test/utils';
import { http, HttpResponse } from 'msw';
import { server } from '@/test/mocks/server';
import { resetMockData } from '@/test/mocks/handlers';
import { ListsPage } from './ListsPage';

describe('ListsPage', () => {
  beforeEach(() => {
    resetMockData();
  });

  it('shows loading state initially', () => {
    render(<ListsPage />);
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('renders list of tasks after fetch', async () => {
    render(<ListsPage />);

    await waitFor(() => {
      expect(screen.getByText('Shopping List')).toBeInTheDocument();
    });
    expect(screen.getByText('Work Tasks')).toBeInTheDocument();
  });

  it('handles empty list state', async () => {
    server.use(
      http.get('/api/Lists', () => {
        return HttpResponse.json({ lists: [] });
      })
    );

    render(<ListsPage />);

    await waitFor(() => {
      expect(screen.getByText('No lists yet. Create one below!')).toBeInTheDocument();
    });
  });

  it('creates new list via AddItemInput', async () => {
    const user = userEvent.setup();
    render(<ListsPage />);

    await waitFor(() => {
      expect(screen.getByText('Shopping List')).toBeInTheDocument();
    });

    const input = screen.getByPlaceholderText('New list name');
    await user.type(input, 'Groceries');

    const addButton = screen.getByTestId('AddIcon').closest('button')!;
    await user.click(addButton);

    await waitFor(() => {
      expect(screen.getByText('Groceries')).toBeInTheDocument();
    });
  });

  it('deletes list on delete button click', async () => {
    const user = userEvent.setup();
    render(<ListsPage />);

    await waitFor(() => {
      expect(screen.getByText('Shopping List')).toBeInTheDocument();
    });

    // Find the delete buttons
    const deleteButtons = screen.getAllByRole('button', { name: '' });
    // First delete button corresponds to first list
    await user.click(deleteButtons[0]);

    await waitFor(() => {
      expect(screen.queryByText('Shopping List')).not.toBeInTheDocument();
    });
  });

  it('has links to list detail pages', async () => {
    render(<ListsPage />);

    await waitFor(() => {
      expect(screen.getByText('Shopping List')).toBeInTheDocument();
    });

    const link = screen.getByRole('link', { name: 'Shopping List' });
    expect(link).toHaveAttribute('href', '/lists/1');
  });

  it('handles API errors gracefully', async () => {
    server.use(
      http.get('/api/Lists', () => {
        return new HttpResponse(null, { status: 500 });
      })
    );

    render(<ListsPage />);

    // Should show loading initially then recover
    expect(screen.getByText('Loading...')).toBeInTheDocument();

    // Wait for query to settle and show empty state (default value)
    await waitFor(() => {
      expect(screen.queryByText('Loading...')).not.toBeInTheDocument();
    });
  });

  it('displays page title', async () => {
    render(<ListsPage />);

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'My Lists' })).toBeInTheDocument();
    });
  });
});
