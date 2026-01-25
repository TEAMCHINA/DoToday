import { Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Container,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Box,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import { AddItemInput } from '@/components/AddItemInput';
import { DoTodayApiClient, CreateTaskListRequest } from '@/api/DoTodayApiClient';

const apiClient = new DoTodayApiClient();

export function ListsPage() {
  const queryClient = useQueryClient();

  const { data: lists = [], isLoading } = useQuery({
    queryKey: ['lists'],
    queryFn: async () => {
      const response = await apiClient.getLists();
      return response.lists ?? [];
    },
  });

  const createListMutation = useMutation({
    mutationFn: (name: string) => {
      const request = new CreateTaskListRequest({ name });
      return apiClient.createList(request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lists'] });
    },
    onError: (error) => {
      console.error('Failed to create list:', error);
    },
  });

  const deleteListMutation = useMutation({
    mutationFn: (id: number) => apiClient.deleteList(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lists'] });
    },
    onError: (error) => {
      console.error('Failed to delete list:', error);
    },
  });

  if (isLoading) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography>Loading...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        My Lists
      </Typography>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {lists.length === 0 ? (
              <TableRow>
                <TableCell colSpan={2} align="center">
                  No lists yet. Create one below!
                </TableCell>
              </TableRow>
            ) : (
              lists.map((list) => (
                <TableRow key={list.id}>
                  <TableCell>
                    <Link to={`/lists/${list.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
                      {list.name}
                    </Link>
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      onClick={() => deleteListMutation.mutate(list.id!)}
                      color="error"
                      size="small"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Box sx={{ mt: 3 }}>
        <AddItemInput placeholder="New list name" onAdd={(name) => createListMutation.mutate(name)} maxLength={200} />
      </Box>
    </Container>
  );
}
