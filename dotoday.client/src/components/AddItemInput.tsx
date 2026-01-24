import { useState } from 'react';
import { Box, TextField, IconButton } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';

interface AddItemInputProps {
  placeholder: string;
  onAdd: (value: string) => void;
}

export function AddItemInput({ placeholder, onAdd }: AddItemInputProps) {
  const [value, setValue] = useState('');

  const handleAdd = () => {
    const trimmed = value.trim();
    if (trimmed) {
      onAdd(trimmed);
      setValue('');
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleAdd();
    }
  };

  return (
    <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
      <TextField
        size="small"
        placeholder={placeholder}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        onKeyDown={handleKeyDown}
        sx={{ flexGrow: 1 }}
      />
      <IconButton onClick={handleAdd} color="primary" disabled={!value.trim()}>
        <AddIcon />
      </IconButton>
    </Box>
  );
}
