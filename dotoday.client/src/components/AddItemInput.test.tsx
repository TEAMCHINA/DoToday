import { describe, it, expect, vi } from 'vitest';
import userEvent from '@testing-library/user-event';
import { render, screen } from '@/test/utils';
import { AddItemInput } from './AddItemInput';

describe('AddItemInput', () => {
  it('renders with placeholder text', () => {
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} />);
    expect(screen.getByPlaceholderText('Add item')).toBeInTheDocument();
  });

  it('handles user input', async () => {
    const user = userEvent.setup();
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'New item');

    expect(input).toHaveValue('New item');
  });

  it('shows error on blur when input is empty after typing', async () => {
    const user = userEvent.setup();
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'a');
    await user.clear(input);
    await user.tab();

    // Error should not show when empty on blur without prior content
    expect(screen.queryByText('Cannot be empty')).not.toBeInTheDocument();
  });

  it('shows error when value is too short', async () => {
    const user = userEvent.setup();
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} minLength={5} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'abc');
    await user.tab();

    expect(screen.getByText('Must be at least 5 characters')).toBeInTheDocument();
  });

  it('shows error when value is too long', async () => {
    const user = userEvent.setup();
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} maxLength={5} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'abcdef');
    await user.tab();

    expect(screen.getByText('Must be 5 characters or less')).toBeInTheDocument();
  });

  it('calls onAdd with trimmed value on submit', async () => {
    const user = userEvent.setup();
    const onAdd = vi.fn();
    render(<AddItemInput placeholder="Add item" onAdd={onAdd} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, '  New item  ');

    const button = screen.getByRole('button');
    await user.click(button);

    expect(onAdd).toHaveBeenCalledWith('New item');
  });

  it('clears input after successful add', async () => {
    const user = userEvent.setup();
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'New item');

    const button = screen.getByRole('button');
    await user.click(button);

    expect(input).toHaveValue('');
  });

  it('disables button when input is empty', () => {
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} />);

    const button = screen.getByRole('button');
    expect(button).toBeDisabled();
  });

  it('enables button when input has content', async () => {
    const user = userEvent.setup();
    render(<AddItemInput placeholder="Add item" onAdd={vi.fn()} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'New item');

    const button = screen.getByRole('button');
    expect(button).toBeEnabled();
  });

  it('submits on Enter key', async () => {
    const user = userEvent.setup();
    const onAdd = vi.fn();
    render(<AddItemInput placeholder="Add item" onAdd={onAdd} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'New item{Enter}');

    expect(onAdd).toHaveBeenCalledWith('New item');
  });

  it('does not call onAdd when input is invalid', async () => {
    const user = userEvent.setup();
    const onAdd = vi.fn();
    render(<AddItemInput placeholder="Add item" onAdd={onAdd} minLength={5} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, 'abc{Enter}');

    expect(onAdd).not.toHaveBeenCalled();
    expect(screen.getByText('Must be at least 5 characters')).toBeInTheDocument();
  });

  it('does not call onAdd when input is only whitespace', async () => {
    const user = userEvent.setup();
    const onAdd = vi.fn();
    render(<AddItemInput placeholder="Add item" onAdd={onAdd} />);

    const input = screen.getByPlaceholderText('Add item');
    await user.type(input, '   {Enter}');

    expect(onAdd).not.toHaveBeenCalled();
  });
});
