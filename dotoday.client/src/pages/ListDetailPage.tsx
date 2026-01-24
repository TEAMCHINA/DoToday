import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
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
import { DoTodayApiClient, TaskListDto, CreateTaskRequest, UpdateTaskRequest } from '@/api/DoTodayApiClient';

const apiClient = new DoTodayApiClient();

export function ListDetailPage() {
  const { listId } = useParams<{ listId: string }>();
  const [list, setList] = useState<TaskListDto | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchList = async () => {
    if (!listId) return;
    try {
      const response = await apiClient.getListById(parseInt(listId, 10));
      setList(response.list ?? null);
    } catch (error) {
      console.error('Failed to fetch list:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchList();
  }, [listId]);

  const handleAddTask = async (title: string) => {
    if (!listId) return;
    try {
      const request = new CreateTaskRequest({ title });
      await apiClient.createTask(parseInt(listId, 10), request);
      await fetchList();
    } catch (error) {
      console.error('Failed to create task:', error);
    }
  };

  const handleToggleTask = async (taskId: number, currentStatus: boolean) => {
    if (!listId) return;
    try {
      const request = new UpdateTaskRequest({ isCompleted: !currentStatus });
      await apiClient.updateTask(parseInt(listId, 10), taskId, request);
      await fetchList();
    } catch (error) {
      console.error('Failed to update task:', error);
    }
  };

  if (loading) {
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

      <Box sx={{ mb: 3 }}>
        <AddItemInput placeholder="New task" onAdd={handleAddTask} />
      </Box>

      <Paper>
        <List>
          {(list.tasks?.length ?? 0) === 0 ? (
            <ListItem>
              <ListItemText primary="No tasks yet. Add one above!" />
            </ListItem>
          ) : (
            list.tasks!.map((task) => (
              <ListItem key={task.id} disablePadding>
                <ListItemButton onClick={() => handleToggleTask(task.id!, task.isCompleted!)}>
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
    </Container>
  );
}
