import { useState } from 'react';
import { Box, TextField, IconButton } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';

interface AddItemInputProps {
  placeholder: string;
  onAdd: (value: string) => void;
  minLength?: number;
  maxLength?: number;
}

export function AddItemInput({
  placeholder,
  onAdd,
  minLength = 1,
  maxLength = 100
}: AddItemInputProps) {
  const [value, setValue] = useState('');
  const [touched, setTouched] = useState(false);

  const trimmed = value.trim();
  const tooShort = trimmed.length < minLength;
  const tooLong = trimmed.length > maxLength;
  const isValid = !tooShort && !tooLong;

  const getErrorMessage = (): string | undefined => {
    if (!touched) return undefined;
    if (tooShort && trimmed.length === 0) return 'Cannot be empty';
    if (tooShort) return `Must be at least ${minLength} characters`;
    if (tooLong) return `Must be ${maxLength} characters or less`;
    return undefined;
  };

  const handleAdd = () => {
    setTouched(true);
    if (isValid) {
      onAdd(trimmed);
      setValue('');
      setTouched(false);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleAdd();
    }
  };

  const handleBlur = () => {
    if (value) {
      setTouched(true);
    }
  };

  const errorMessage = getErrorMessage();

  return (
    <Box sx={{ display: 'flex', gap: 1, alignItems: 'flex-start' }}>
      <TextField
        size="small"
        placeholder={placeholder}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        onKeyDown={handleKeyDown}
        onBlur={handleBlur}
        error={!!errorMessage}
        helperText={errorMessage}
        inputProps={{ maxLength: maxLength + 10 }}
        sx={{ flexGrow: 1 }}
      />
      <IconButton
        onClick={handleAdd}
        color="primary"
        disabled={!trimmed}
        sx={{ mt: '4px' }}
      >
        <AddIcon />
      </IconButton>
    </Box>
  );
}
