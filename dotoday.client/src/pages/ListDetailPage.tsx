import { useParams, Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Container,
  Typography,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Checkbox,
  Paper,
  Box,
  Button,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { AddItemInput } from '@/components/AddItemInput';
import { DoTodayApiClient, CreateTaskRequest, UpdateTaskRequest } from '@/api/DoTodayApiClient';

const apiClient = new DoTodayApiClient();

export function ListDetailPage() {
  const { listId } = useParams<{ listId: string }>();
  const queryClient = useQueryClient();
  const listIdNum = listId ? parseInt(listId, 10) : 0;

  const { data: list, isLoading } = useQuery({
    queryKey: ['list', listIdNum],
    queryFn: async () => {
      const response = await apiClient.getListById(listIdNum);
      return response.list ?? null;
    },
    enabled: !!listId,
  });

  const createTaskMutation = useMutation({
    mutationFn: (title: string) => {
      const request = new CreateTaskRequest({ title });
      return apiClient.createTask(listIdNum, request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['list', listIdNum] });
    },
    onError: (error) => {
      console.error('Failed to create task:', error);
    },
  });

  const updateTaskMutation = useMutation({
    mutationFn: ({ taskId, isCompleted }: { taskId: number; isCompleted: boolean }) => {
      const request = new UpdateTaskRequest({ isCompleted });
      return apiClient.updateTask(listIdNum, taskId, request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['list', listIdNum] });
    },
    onError: (error) => {
      console.error('Failed to update task:', error);
    },
  });

  if (isLoading) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography>Loading...</Typography>
      </Container>
    );
  }

  if (!list) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography>List not found</Typography>
        <Button component={Link} to="/" startIcon={<ArrowBackIcon />} sx={{ mt: 2 }}>
          Back to Lists
        </Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Button component={Link} to="/" startIcon={<ArrowBackIcon />} sx={{ mb: 2 }}>
        Back to Lists
      </Button>

      <Typography variant="h4" component="h1" gutterBottom>
        {list.name}
      </Typography>

      <Paper>
        <List>
          {(list.tasks?.length ?? 0) === 0 ? (
            <ListItem>
              <ListItemText primary="No tasks yet. Add one below!" />
            </ListItem>
          ) : (
            list.tasks!.map((task) => (
              <ListItem key={task.id} disablePadding>
                <ListItemButton onClick={() => updateTaskMutation.mutate({ taskId: task.id!, isCompleted: !task.isCompleted })}>
                  <ListItemIcon>
                    <Checkbox
                      edge="start"
                      checked={task.isCompleted}
                      tabIndex={-1}
                      disableRipple
                    />
                  </ListItemIcon>
                  <ListItemText
                    primary={task.title}
                    sx={{
                      textDecoration: task.isCompleted ? 'line-through' : 'none',
                      color: task.isCompleted ? 'text.secondary' : 'text.primary',
                    }}
                  />
                </ListItemButton>
              </ListItem>
            ))
          )}
        </List>
      </Paper>

      <Box sx={{ mt: 3 }}>
        <AddItemInput placeholder="New task" onAdd={(title) => createTaskMutation.mutate(title)} maxLength={500} />
      </Box>
    </Container>
  );
}
