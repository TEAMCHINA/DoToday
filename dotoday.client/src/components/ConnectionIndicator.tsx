import { Box, Tooltip } from '@mui/material';
import { useConnectionStatus } from '@/hooks/useSignalR';
import type { ConnectionStatus } from '@/hooks/useSignalR';

const statusConfig: Record<ConnectionStatus, { color: string; label: string }> = {
  connected: { color: '#4caf50', label: 'Connected' },
  connecting: { color: '#ff9800', label: 'Connecting...' },
  disconnected: { color: '#f44336', label: 'Disconnected' },
};

export function ConnectionIndicator() {
  const status = useConnectionStatus();
  const { color, label } = statusConfig[status];

  return (
    <Tooltip title={label}>
      <Box
        component="span"
        sx={{
          display: 'inline-block',
          width: 12,
          height: 12,
          borderRadius: '50%',
          backgroundColor: color,
          ml: 1,
          verticalAlign: 'middle',
        }}
      />
    </Tooltip>
  );
}
